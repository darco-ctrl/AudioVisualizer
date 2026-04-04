using Godot;
using System;
using NAudio.Wave;
using System.Linq;

public partial class SampleData : Node
{

	public int RingBuffer = 1024;

	private int WriteIndex = 0;

	private readonly object LockObject = new object();

	public float[] Samples;

	private WaveInEvent WaveIn;

	public override void _Ready()
	{
		Samples = new float[RingBuffer];

		WaveIn = new WaveInEvent(); // make new listner

		WaveIn.WaveFormat = new WaveFormat(44099, 1); // tell it to recored at 44099 samples per second

		WaveIn.DataAvailable += Capture_DataAvailable; // Says to call Capture DataAvailable when new one aavaiable

		//Starts the recording 
		WaveIn.StartRecording();
	}

	private void Capture_DataAvailable(Object _sender, WaveInEventArgs _eventArgs)
	{
		int sampleCount = _eventArgs.BytesRecorded / 4;

		lock (LockObject) {
			for (int i = 0; i < sampleCount; i++)
			{
				float sample = BitConverter.ToSingle(_eventArgs.Buffer, i * 4);

				Samples[WriteIndex] = sample;

				WriteIndex = (WriteIndex + 1) % RingBuffer;
			}
		}
	}	

	public float[] GetSamples()
	{
		lock (LockObject)
		{
			return Samples.ToArray();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _EnterTree()
    {
        GD.Print("Exiting . . .");

		WaveIn.StopRecording();
		WaveIn.Dispose();
    }

}
