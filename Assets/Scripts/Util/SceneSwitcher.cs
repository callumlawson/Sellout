using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    public string DestinationScene;

	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(DestinationScene, LoadSceneMode.Single);
        }
	}
}
