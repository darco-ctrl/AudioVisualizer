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
	public readonly float MAX_SPECTRUM_MAGNITUDE = 10f;

	private readonly object LockObject = new object();

	private float[] samples;
	private float[] frame;

	private double[] _window;

	public WasapiLoopbackCapture Capture;

	private float[] spectrum;
	public float[] LeftSpectrum;
	public float[] RightSpectrum;

	private Complex[] fftBuffer;

	private int writeIndex = 0;
	private int framePos = 0;

	private bool queuedFFT = false;



	public override void _Ready()
	{
		_window =  NWindow.Hann(RING_BUFFER);

		samples = new float[RING_BUFFER];

		frame = new float[FFT_SIZE];

		spectrum = new float[FFT_SIZE / 2];

		LeftSpectrum = new float[FFT_SIZE / 4];
		RightSpectrum = new float[FFT_SIZE / 4];

		fftBuffer = new Complex[FFT_SIZE];

		Capture = new WasapiLoopbackCapture();
		Capture.DataAvailable += Capture_DataAvailable;
		Capture.StartRecording();
	}

	private void Capture_DataAvailable(Object _sender, WaveInEventArgs _eventArgs)
	{
		int sampleCount = _eventArgs.BytesRecorded / 4;

		lock (LockObject) {
			for (int i = 0; i < sampleCount; i++)
			{
				float sample = BitConverter.ToSingle(_eventArgs.Buffer, i * 4);

				samples[writeIndex] = sample;

				writeIndex = (writeIndex + 1) % RING_BUFFER;

				frame[framePos++] = sample;

				if (framePos >= FFT_SIZE)
				{
					queuedFFT = true;
					framePos = 0;	
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
		
		bool isValueLeft = true;
		int leftSpectrumIndex = 0;
		int rightSpectrumIndex = 0;

		double[] window = _window;

		for (int i = 0; i < FFT_SIZE; i++)
		{
			frame[i] = frame[i] * (float)window[i];

			fftBuffer[i] = new Complex(frame[i], 0);

		}

		NFourier.Forward(fftBuffer, FourierOptions.Matlab);
		
		for (int i = 0; i < FFT_SIZE / 2; i++)
		{
			float mag = NormalizeMagnitude((float)fftBuffer[i].Magnitude);

			spectrum[i] = mag;

			if (isValueLeft)
			{
				LeftSpectrum[leftSpectrumIndex++] = mag;
			} else
			{
				RightSpectrum[rightSpectrumIndex++] = mag;
			}

			isValueLeft = !isValueLeft;
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
			return samples.ToArray();
		}
	}

	public float[] GetSpectrum()
	{
		lock (LockObject)
		{
			return spectrum.ToArray();
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
