using DG.Tweening;
using UnityEngine;

public class SpriteFader : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public float delayInSeconds;
    public float duration;
    public bool fadeout;

	// Use this for initialization
	void Start ()
	{
	    spriteRenderer.DOFade(1.0f, duration).SetDelay(delayInSeconds);
	    if (fadeout)
	    {
            spriteRenderer.DOFade(0.0f, duration).SetDelay(delayInSeconds + duration);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
