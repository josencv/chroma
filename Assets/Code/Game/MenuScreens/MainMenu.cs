using System;
using Chroma.Infrastructure.Menu;
using Zenject;

namespace Chroma.Game.MenuScreens
{

    public class MainMenu : MenuScreen
    {
        private GameManager gameManager;

        [Inject]
        private void Inject(GameManager gameManager)
        {
            // JIN: I don't like injecting the game manager object directly in other instances,
            // but I haven't found a straightforward way to find the instance of the main menu in the hierarchy
            // using Zenject and then injecting it into the game manager.
            // Another option would be to instantiate the MainMenu from a prefab in the GameInstaller, but then I'll to do
            // more non-straightforward magic to get the menu displayed in the scene.
            this.gameManager = gameManager;
        }

        private void Awake()
        {
            name = "MainMenu";
        }

        public void OnPlayButtonPressed()
        {
            gameManager.StartGame();
            gameObject.SetActive(false);
        }

        public void OnQuitButtonPressed()
        {
            gameManager.QuitGame();
        }
    }
}
