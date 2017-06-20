using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameSceneLoader : MonoBehaviour
    {
        public const string UISceneName = "UIScene";

        [UsedImplicitly]
        public void Awake()
        {
            SceneManager.LoadScene(UISceneName, LoadSceneMode.Additive);
            SceneManager.sceneLoaded += TryStartGame;
        }

        private void TryStartGame(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == UISceneName)
            {
                StartCoroutine(StartGame());
                
            }
        }

        private IEnumerator StartGame()
        {
            yield return null;
            GameRunner.Instance.StartGame();
        }
    }
}