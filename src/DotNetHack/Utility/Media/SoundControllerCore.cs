﻿using System;
using NAudio.Wave;
using System.Collections.Generic;
using System.Threading;

namespace DotNetHack.Utility.Media
{
    /// <summary>
    /// SoundController, core
    ///     partial
    /// <seealso cref="http://opensebj.blogspot.com/2009/02/naudio-tutorial-2-mixing-multiple-wave.html"/>
    /// </summary>
    public partial class SoundController : BaseSoundController
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public override void Initialize()
        {
            try
            {
                // Call down to base to fire up events.
                base.Initialize();

                // Setup mixer
                Mixer = new WaveMixerStream32();
                Mixer.AutoStop = true;

                // Create a new wave-out device.
                WaveOutDevice = new WaveOut();
                WaveOutDevice.Init(Mixer);

                // Initialize the lazy loading hash
                SoundCache = new Dictionary<string, CoreSample>();
            }
            catch (Exception sound_ex) { }
        }

        /// <summary>
        /// PlaySound
        /// 
        /// <remarks>do not pass a fully qualified path, just a filename</remarks>
        /// </summary>
        /// <param name="aSoundFileName">The name of the sound file to play.</param>
        public override void PlaySound(string aSoundFileName)
        {
            if (!SoundCache.ContainsKey(aSoundFileName))
            {
                WaveFileReader tmpWaveFileReader = new WaveFileReader(aSoundFileName);
                WaveOffsetStream tmpWaveOffsetStream = new WaveOffsetStream(tmpWaveFileReader);
                WaveChannel32 tmpWaveChannel32 = new WaveChannel32(tmpWaveFileReader);

                SoundCache.Add(aSoundFileName, new CoreSample()
                {
                    WaveFileReader = tmpWaveFileReader,
                    WaveOffsetStream = tmpWaveOffsetStream,
                    WaveChannel32 = tmpWaveChannel32,
                });

                Mixer.AddInputStream(SoundCache[aSoundFileName].WaveChannel32);
            }

            SoundCache[aSoundFileName].WaveChannel32.Position = 0x00;

            WaveOutDevice.Play();
        }

        /// <summary>
        /// WaveOutDevice, interface(ed)
        /// </summary>
        static  IWavePlayer WaveOutDevice = null;

        /// <summary>
        /// The <c>Mixer</c>
        /// </summary>
        static WaveMixerStream32 Mixer = null;

        /// <summary>
        /// LazyCoreShot
        /// </summary>
        static Dictionary<string, CoreSample> SoundCache { get; set; }

        /// <summary>
        /// Aggregated objects required for playback
        /// </summary>
        struct CoreSample
        {
            public WaveFileReader WaveFileReader;
            public WaveChannel32 WaveChannel32;
            public WaveOffsetStream WaveOffsetStream;
        }
    }
}