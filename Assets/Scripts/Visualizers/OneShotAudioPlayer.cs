using UnityEngine;

public class OneShotAudioPlayer : MonoBehaviour {

    public AudioSource AudioSource;

    public void PlayOneShot()
    {
        AudioSource.Play();
    }
}
