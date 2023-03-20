using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace HB.Core.Modules.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioData _audioData;
        [SerializeField] private AudioSource _musicSource, _sfxSource;
        private bool _isMusicOn, _isSfxOn;
        public AudioMixer Mixer => _musicSource.outputAudioMixerGroup.audioMixer;

        public void SetSoundSetting(bool isMusicOn, bool isSfxOn)
        {
            _isMusicOn = isMusicOn;
            _isSfxOn = isSfxOn;
            _musicSource.mute = !isMusicOn;
            _sfxSource.mute = !isSfxOn;
        }

        public bool[] GetSoundSetting()
        {
            return new[] { _isMusicOn, _isSfxOn };
        }

        public void Stop()
        {
            _musicSource.Stop();
            _sfxSource.Stop();
        }

        public void PlayClip(string clipName, float delay = 0f)
        {
          if(string.IsNullOrEmpty(clipName))return;
            var c = _audioData.Clips.Find(i => i.ClipName == clipName);
            if (c == null) return;
            if (c.type == AudioClipType.Music)
            {
                if (_musicSource.clip == c.Clip && _musicSource.isPlaying) return;
                if (clipName == "HomeEditorMusic" || clipName == "Match3Music")
                {
                    _musicSource.loop = true;

                }
                else
                {
                    _musicSource.loop = false;
                }

                _musicSource.clip = c.Clip;
                _musicSource.PlayDelayed(delay);
            }
            else
            {

                StartCoroutine(PlaySfx(c.Clip, delay));
            }

        }



        public IEnumerator PlaySfx(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            _sfxSource.PlayOneShot(clip, 1);
        }
    }
}