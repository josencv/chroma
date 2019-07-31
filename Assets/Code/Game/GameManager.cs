using Chroma.Game.LevelManagement;
using Chroma.Game.MenuScreens;
using UnityEditor;

namespace Chroma.Game
{
    public class GameManager
    {
        private GameState currentState;
        private LevelLoader levelLoader;

        public GameManager(LevelLoader levelLoader)
        {
            currentState = GameState.InMenus;
            this.levelLoader = levelLoader;
        }

        public void StartGame()
        {
            levelLoader.LoadLevel(LevelNames.MainLevel);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        private void OnMainMenuPlayButtonPressed()
        {
            if (currentState != GameState.InGame)
            {
                StartGame();
            }
        }

        private void OnMainMenuQuitButtonPressed()
        {
            QuitGame();
        }
    }
}
