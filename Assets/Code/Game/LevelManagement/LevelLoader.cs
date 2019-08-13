using System.Collections;
using Chroma.Game.LoadingScreens;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Chroma.Game.LevelManagement
{
    public class LevelLoader : MonoBehaviour
    {
        private LoadingScreen loadingScreen;

        [Inject]
        private void Inject(LoadingScreen loadingScreen)
        {
            this.loadingScreen = loadingScreen;
        }

        public void LoadLevel(string levelName)
        {
            // If the scene is already loaded we skip the loading step
            if(SceneManager.GetActiveScene().name != levelName)
            {
                // TODO: show loading loading scren
                StartCoroutine(LoadYourAsyncScene(levelName));
            }
        }

        IEnumerator LoadYourAsyncScene(string levelName)
        {
            loadingScreen.gameObject.SetActive(true);
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

            // Wait until the asynchronous scene fully loads
            while(!asyncOperation.isDone)
            {
                yield return null;
            }

            loadingScreen.gameObject.SetActive(false);
        }
    }
}
