using System;

using Garage.Core.DI;
using HB.Core.DI;
using UnityEngine;
using UnityEngine.Audio;
using IModule = HB.Match3.Cells.IModule;

namespace HB.Core.Modules.Audio
{
    public class AudioModule : MonoModule, ILoadable
    {
        [Range(0.0001f, 1)]
        private float _volume = 0;

        private AudioMixer _mixer;
        private AudioPlayer _audioPlayer;

        private const string MusicVolumeParameter = "MusicVolume";
        private const string SfxVolumeParameter = "SfxVolume";



        public static void MuteAll()
        {
            AudioListener.pause = true;
        }

        public static void UnMuteAll()
        {
            AudioListener.pause = false;
        }

        public void SetMusicVolume(float vol)
        {
            _volume = vol;
            _volume = Mathf.Clamp(vol, 0.0001f, 1);
            _mixer.SetFloat(MusicVolumeParameter, Mathf.Log10(_volume) * 20);

        }

        public void SetSfxVolume(float vol)
        {
            _volume = vol;
            _volume = Mathf.Clamp(vol, 0.0001f, 1);
            _mixer.SetFloat(SfxVolumeParameter, Mathf.Log10(_volume) * 20);
        }

        //private AssetLoader _loader;
        public override void OnRegister(IContext context)
        {
            //_loader = context.Get<AssetLoader>();

        }

        public async void Load(Action<IModule> onLoaded)
        {
            if (_audioPlayer != null)
            {
                onLoaded?.Invoke(this);
                return;
            }
            
            GameObject go = Resources.Load<GameObject>("AudioPlayer");
            _audioPlayer = Instantiate(go).GetComponent<AudioPlayer>();
            DontDestroyOnLoad(_audioPlayer);

            _mixer = _audioPlayer.Mixer;
            onLoaded?.Invoke(this);
        }

        public AudioPlayer GetAudioPlayer()
        {
        
            return _audioPlayer;
        }

    }


    public enum AudioClipType
    {
        Music,
        Sfx
    }
}