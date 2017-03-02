using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace _8beatMap
{
    static class Sound
    {
        static WaveOutEvent WaveOut = new WaveOutEvent { DesiredLatency = 100, NumberOfBuffers = 16 };
        static public WaveFileReader MusicReader;

        static WaveOutEvent NoteSoundWaveOut = new WaveOutEvent { DesiredLatency = 110, NumberOfBuffers = 4 };
        public static NAudio.Wave.SampleProviders.SignalGenerator NoteSoundSig = new NAudio.Wave.SampleProviders.SignalGenerator { Frequency = 1000, Gain = 0.5, Type = NAudio.Wave.SampleProviders.SignalGeneratorType.Square };
        public static NAudio.Wave.SampleProviders.OffsetSampleProvider NoteSoundSigTrim;

        static public CachedSound NoteSoundWave;
        static public CachedSound NoteSoundWave_Swipe;

        static NAudio.Wave.SampleProviders.MixingSampleProvider NoteSoundMixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2)) { ReadFully = true };

        static System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());

        public class CachedSound
        {
            public float[] AudioData { get; private set; }
            public WaveFormat WaveFormat { get; private set; }
            public CachedSound(string audioFileName)
            {
                using (var audioFileReader = new AudioFileReader(audioFileName))
                {
                    // TODO: could add resampling in here if required
                    WaveFormat = audioFileReader.WaveFormat;
                    var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                    var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                    int samplesRead;
                    while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        wholeFile.AddRange(readBuffer.Take(samplesRead));
                    }
                    AudioData = wholeFile.ToArray();
                }
            }
        }

        class CachedSoundSampleProvider : ISampleProvider
        {
            private readonly CachedSound cachedSound;
            private long position;

            public CachedSoundSampleProvider(CachedSound cachedSound)
            {
                this.cachedSound = cachedSound;
            }

            public int Read(float[] buffer, int offset, int count)
            {
                var availableSamples = cachedSound.AudioData.Length - position;
                var samplesToCopy = Math.Min(availableSamples, count);
                Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
                position += samplesToCopy;
                return (int)samplesToCopy;
            }

            public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
        }


        static public void PlayMusic()
        {
            WaveOut.Play();
        }
        static public void PauseMusic()
        {
            WaveOut.Pause();
        }
        static public void StopMusic()
        {
            WaveOut.Stop();
        }
        static public void LoadMusic(string path)
        {
            if (path.Length > 0)
            {
                WaveOut.Stop();

                try
                {
                    MusicReader = new WaveFileReader(path);
                }
                catch { System.Windows.Forms.MessageBox.Show(DialogResMgr.GetString("MusicLoadError")); return; }

                WaveOut.Init(MusicReader);
            }
        }

        static public void SetNoteSoundLatency(int ms)
        {
            NoteSoundWaveOut.DesiredLatency = ms;
        }

        static public void InitNoteSounds()
        {
            NoteSoundWaveOut.Init(NoteSoundMixer);
            NoteSoundWaveOut.Play();
        }

        static public void PlayNoteSound(CachedSound sound)
        {
            NoteSoundMixer.AddMixerInput(new CachedSoundSampleProvider(sound));
        }
        static public void PlayNoteSound(ISampleProvider sound)
        {
            NoteSoundMixer.AddMixerInput(sound);
        }
    }
}
