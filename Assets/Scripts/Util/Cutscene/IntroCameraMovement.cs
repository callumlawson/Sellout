using DG.Tweening;
using UnityEngine;

public class IntroCameraMovement : MonoBehaviour
{
    public float durationInSeconds;
    public float distance;

	// Use this for initialization
	void Start ()
	{
	    gameObject.transform.DOMoveX(distance, durationInSeconds, false).SetRelative().SetEase(Ease.InOutSine);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
