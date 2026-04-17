using Godot;
using System;
using NAudio.Wave;
using System.Linq;
using System.Collections;
using System.Numerics;

using NWindow = MathNet.Numerics.Window;
using NFourier = MathNet.Numerics.IntegralTransforms.Fourier;
using MathNet.Numerics.IntegralTransforms;
using NAudio.MediaFoundation;

public partial class SampleData : Node
{

	public readonly int RING_BUFFER = 1024;
	public readonly int FFT_SIZE = 1024;
	public readonly float MAX_SPECTRUM_MAGNITUDE = 50f;
	public int SINGLE_CHANNEL_LENGTH;

	private readonly object LockObject = new object();

	public float[] Samples;
	private float[] frameLeft;
	private float[] frameRight;

	private double[] _window;

	public WasapiLoopbackCapture Capture;

	public float[] LeftSpectrum;
	public float[] RightSpectrum;

	private Complex[] fftBufferLeft;
	private Complex[] fftBufferRight;

	private int writeIndex = 0;
	private int framePosLeft = 0;
	private int framePosRight = 0;

	private bool queuedFFT = false;

	public override void _Ready()
	{
		SINGLE_CHANNEL_LENGTH = FFT_SIZE / 2;

		_window = NWindow.Hann(FFT_SIZE);

		Samples = new float[RING_BUFFER];

		frameLeft = new float[FFT_SIZE];
		frameRight = new float[FFT_SIZE];

		LeftSpectrum = new float[SINGLE_CHANNEL_LENGTH];
		RightSpectrum = new float[SINGLE_CHANNEL_LENGTH];

		fftBufferLeft = new Complex[FFT_SIZE];
		fftBufferRight = new Complex[FFT_SIZE];

		Capture = new WasapiLoopbackCapture();
		Capture.DataAvailable += Capture_DataAvailable;
		Capture.StartRecording();
	}

	private void Capture_DataAvailable(Object _sender, WaveInEventArgs _eventArgs)
	{
		int sampleCount = _eventArgs.BytesRecorded / 4;

		lock (LockObject) {
			for (int i = 0; i < sampleCount; i += 2)
			{
				float left  = BitConverter.ToSingle(_eventArgs.Buffer, i * 4);
				float right = BitConverter.ToSingle(_eventArgs.Buffer, (i + 1) * 4);

				// ring buffer gets mono mix for waveform display
				Samples[writeIndex] = (left + right) * 0.5f;
				writeIndex = (writeIndex + 1) % RING_BUFFER;

				frameLeft[framePosLeft++]   = left;
				frameRight[framePosRight++] = right;

				if (framePosLeft >= FFT_SIZE)
				{
					queuedFFT = true;
					framePosLeft  = 0;
					framePosRight = 0;
				}
			}

			if (queuedFFT)
			{
				FFTProcess();
			}
		}
	}

	private void FFTProcess()
	{
		queuedFFT = false;

		double[] window = _window;

		for (int i = 0; i < FFT_SIZE; i++)
		{
			fftBufferLeft[i]  = new Complex(frameLeft[i]  * window[i], 0);
			fftBufferRight[i] = new Complex(frameRight[i] * window[i], 0);
		}

		NFourier.Forward(fftBufferLeft,  FourierOptions.Matlab);
		NFourier.Forward(fftBufferRight, FourierOptions.Matlab);

		for (int i = 0; i < FFT_SIZE / 2; i++)
		{
			LeftSpectrum[i]  = NormalizeMagnitude((float)fftBufferLeft[i].Magnitude);
			RightSpectrum[i] = NormalizeMagnitude((float)fftBufferRight[i].Magnitude);
		}
	}

	private float NormalizeMagnitude(float magnitude)
	{
		return Mathf.Clamp(magnitude / MAX_SPECTRUM_MAGNITUDE, 0f, 1f);
	}

	public float[] GetSamples()
	{
		lock (LockObject)
		{
			return Samples.ToArray();
		}
	}

	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		GD.Print("Exiting . . .");

		if (Capture != null)
		{
			Capture.StopRecording();
			Capture.Dispose();
		}
	}
}