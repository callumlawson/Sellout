using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    public string DestinationScene;

    private AsyncOperation gameLoading;

    [UsedImplicitly]
    public void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        gameLoading = SceneManager.LoadSceneAsync(DestinationScene);
        gameLoading.allowSceneActivation = false;

        Debug.Log("start loading");

        yield return gameLoading;
    }

    [UsedImplicitly]
	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
		    gameLoading.allowSceneActivation = true;
		}
	}
}
