using Godot;
using System;
using NAudio.Wave;
using System.Linq;
using System.Collections;
using System.Numerics;

using NWindow = MathNet.Numerics.Window;
using NFourier = MathNet.Numerics.IntegralTransforms.Fourier;
using MathNet.Numerics.IntegralTransforms;

public partial class SampleData : Node
{

	public const int RING_BUFFER = 1024;
	public const int FFT_SIZE = 2048;

	private readonly object LockObject = new object();

	private float[] samples;
	private float[] frame;

	readonly double[] _window = NWindow.Hann(FFT_SIZE);

	private WasapiLoopbackCapture Capture;

	private int writeIndex = 0;
	private int framePos = 0;

	private bool queuedFFT = false;

	public override void _Ready()
	{
		samples = new float[RING_BUFFER];
		frame = new float[FFT_SIZE];

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

		double[] window = _window;
		var fftBuffer = new Complex[FFT_SIZE];

		for (int i = 0; i < FFT_SIZE; i++)
		{
			frame[i] = frame[i] * (float)window[i];

			fftBuffer[i] = new Complex(frame[i], 0);

		}

		NFourier.Forward(fftBuffer, FourierOptions.Matlab);
		
	}

	public float[] GetSamples()
	{
		lock (LockObject)
		{
			return samples.ToArray();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
