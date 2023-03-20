using System;
using System.Collections.Generic;
using UnityEngine;

namespace HB.Core.Modules.Audio
{
    [CreateAssetMenu]
    public class AudioData : ScriptableObject
    {
        public List<AudioFile> Clips;

    }
    
    [Serializable]
    public class AudioFile
    {
        public string ClipName;
        public AudioClip Clip;
        public AudioClipType type;

    }
}
