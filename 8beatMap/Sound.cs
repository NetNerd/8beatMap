﻿using System;
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
        static private void LoadMusicInt(string path)
        {
            if (path.Length > 0)
            {
                WaveMixer.RemoveMixerInput(MusicResamp);

                try
                {
                    MusicReader = new AudioFileReader(path);
                    MusicResamp = new NAudio.Wave.SampleProviders.WdlResamplingSampleProvider(MusicReader, 44100);
                }
                catch (Exception ex)
                {
                    MusicReader = null;
                    MusicResamp = null;
                    throw ex;
                    //System.Windows.Forms.MessageBox.Show(DialogResMgr.GetString("MusicLoadError"));
                    //return;
                }
            }
        }
        static public void LoadMusic(string path)
        {
            try
            {
                LoadMusicInt(path);
            }
            catch
            {
                SkinnedMessageBox.Show(DialogResMgr.GetString("MusicLoadError"), "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        static public void LoadMusic(string path, Skinning.Skin errorskin)
        {
            try
            {
                LoadMusicInt(path);
            }
            catch
            {
                SkinnedMessageBox.Show(errorskin, DialogResMgr.GetString("MusicLoadError"), "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        static public void InitWaveOut()
        {
            WaveOut.Init(VolMixer);
            WaveOut.Play();
        }

        static public void PlayNoteSound(CachedSound snd, int skipMs = 0, int takeMs = 0)
        {
            PlayNoteSound(new CachedSoundSampleProvider(snd), skipMs, takeMs);
        }
        static public void PlayNoteSound(ISampleProvider snd, int skipMs = 0, int takeMs = 0)
        {
            NAudio.Wave.SampleProviders.OffsetSampleProvider NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(snd);

            if (skipMs < 0)
                NoteSoundTrim.DelayBy = TimeSpan.FromMilliseconds(-skipMs);
            else
                NoteSoundTrim.SkipOver = TimeSpan.FromMilliseconds(skipMs);

            if (takeMs > 0)
                NoteSoundTrim.Take = TimeSpan.FromMilliseconds(takeMs);

            WaveMixer.AddMixerInput(NoteSoundTrim);
        }

        static public void SetVolume(float vol)
        {
            VolMixer.Volume = vol;
        }

        static public int Latency
        {
            get
            {
                return WaveOut.DesiredLatency;
            }
            set
            {
                WaveOut.DesiredLatency = value;
            }
        }

        static public int TryGetMp3FileStartDelay(string path)
        {
            int ms = 0;
            const int decode_delay = 529;

            try
            {
                Mp3FileReader reader = new Mp3FileReader(path);
                if (reader.XingHeader != null && reader.XingHeader.Mp3Frame.FrameLength >= 0xb2)
                {
                    if (
                         (reader.XingHeader.Mp3Frame.RawData[0x9c] == 'L' &&
                          reader.XingHeader.Mp3Frame.RawData[0x9d] == 'A' &&
                          reader.XingHeader.Mp3Frame.RawData[0x9e] == 'M' &&
                          reader.XingHeader.Mp3Frame.RawData[0x9f] == 'E'
                         ) ||
                         (reader.XingHeader.Mp3Frame.RawData[0x9c] == 'L' &&
                          reader.XingHeader.Mp3Frame.RawData[0x9d] == 'a' &&
                          reader.XingHeader.Mp3Frame.RawData[0x9e] == 'v' &&
                          reader.XingHeader.Mp3Frame.RawData[0x9f] == 'c'
                         )
                       ) // check for LAME or Lavc
                    {
                        // see http://gabriel.mp3-tech.org/mp3infotag.html#delays for info about this header info
                        int samples = reader.XingHeader.Mp3Frame.RawData[0xb1] << 4 + ((reader.XingHeader.Mp3Frame.RawData[0xb2] & 0xF0) >> 4);
                        samples += decode_delay; // decoder delay
                        ms = 1000 * samples / reader.Mp3WaveFormat.SampleRate;
                    }
                }
                
                if (ms == 0) // assume no header present
                {
                    int samples = 700 + decode_delay; // assuming ~700 samples of encoder delay should be safe http://mp3decoders.mp3-tech.org/decoders_lame.html
                    ms = 1000 * samples / reader.Mp3WaveFormat.SampleRate;
                }
            }
            catch
            { }

            return ms;
        }
    }
}
