using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers
{
    public class StandardSoundPlayer : MonoBehaviour
    {
        public static StandardSoundPlayer Instance;

        [UsedImplicitly] public AudioSource AudioSource;
        [UsedImplicitly] public AudioClip Click;

        [UsedImplicitly]
        public void Awake()
        {
            Instance = this;
        }

        public void PlayClick()
        {
            AudioSource.PlayOneShot(Click);
        }
    }
}
