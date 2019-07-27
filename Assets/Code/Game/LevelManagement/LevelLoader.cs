using UnityEngine.SceneManagement;

namespace Chroma.Game.LevelManagement
{
    public class LevelLoader
    {
        public void LoadLevel(string levelName)
        {
            // If the scene is already loaded we skip the loading step
            if(SceneManager.GetActiveScene().name != levelName)
            {
                SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
            }
        }
    }
}
