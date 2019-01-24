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
        static WaveOutEvent WaveOut = new WaveOutEvent { DesiredLatency = 160, NumberOfBuffers = 20 };
        //static WasapiOut WaveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, true, 100);
        static NAudio.Wave.SampleProviders.MixingSampleProvider WaveMixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2)) { ReadFully = true };
        static NAudio.Wave.SampleProviders.VolumeSampleProvider VolMixer = new NAudio.Wave.SampleProviders.VolumeSampleProvider(WaveMixer) { Volume = 0.666f };
        static NAudio.Wave.SampleProviders.WdlResamplingSampleProvider MusicResamp;
        static public AudioFileReader MusicReader = null;
        
        public static NAudio.Wave.SampleProviders.SignalGenerator NoteSoundSig = new NAudio.Wave.SampleProviders.SignalGenerator { Frequency = 1000, Gain = 0.5, Type = NAudio.Wave.SampleProviders.SignalGeneratorType.Square };

        static public CachedSound NoteSoundWave;
        static public CachedSound NoteSoundWave_Swipe;

        public static NAudio.Wave.SampleProviders.OffsetSampleProvider NoteSoundTrim;


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

        public class CachedSoundSampleProvider : ISampleProvider
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
            WaveMixer.RemoveMixerInput(MusicResamp);
            if (MusicResamp != null )
                WaveMixer.AddMixerInput(MusicResamp);
        }
        static public void StopMusic()
        {
            WaveMixer.RemoveMixerInput(MusicResamp);
        }
        static public void LoadMusic(string path)
        {
            if (path.Length > 0)
            {
                WaveMixer.RemoveMixerInput(MusicResamp);

                try
                {
                    MusicReader = new AudioFileReader(path);
                    MusicResamp = new NAudio.Wave.SampleProviders.WdlResamplingSampleProvider(MusicReader, 44100);
                }
                catch
                {
                    MusicReader = null;
                    MusicResamp = null;
                    System.Windows.Forms.MessageBox.Show(DialogResMgr.GetString("MusicLoadError"));
                    return;
                }
            }
        }
        static public void InitWaveOut()
        {
            WaveOut.Init(VolMixer);
            WaveOut.Play();
        }

        static public void PlayNoteSound(CachedSound sound)
        {
            WaveMixer.AddMixerInput(new CachedSoundSampleProvider(sound));
        }
        static public void PlayNoteSound(ISampleProvider sound)
        {
            WaveMixer.AddMixerInput(sound);
        }
    }
}
