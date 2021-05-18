using System;
using System.Collections.Generic;
using Chroma.Game.Commands;
using Chroma.Game.LevelManagement;
using Chroma.Game.LoadingScreens;
using Chroma.Infrastructure.DI;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Chroma.Game.Configuration
{
    /// <summary>
    /// This is the 'global' DI installer of the game, meant to be used in a Zenject 'ProjectContext'
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private Settings settings;

        public override void InstallBindings()
        {
            Container.Bind<GameManager>().AsSingle().NonLazy();
            Container.Bind<LevelLoader>().FromComponentInNewPrefab(settings.LevelLoaderPrefab).AsSingle();
            Container.Bind<LoadingScreen>().FromComponentInNewPrefab(settings.LoadingScreenPrefab).AsSingle().NonLazy();
            Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<PlayerInput>().FromComponentInNewPrefab(settings.PlayerInputPrefab).AsSingle();
            Container.Bind<InputCommandIssuer>().AsSingle().NonLazy();
        }

        [Serializable]
        public class Settings
        {
            public PlayerInput PlayerInputPrefab;
            public LevelLoader LevelLoaderPrefab;
            public LoadingScreen LoadingScreenPrefab;

            public void CheckSettings()
            {
                List<string> errors = new List<string>();
                if(PlayerInputPrefab == null)
                {
                    errors.Add("'PlayerInputPrefab' is not set");
                }

                if(LoadingScreenPrefab == null)
                {
                    errors.Add("'LoadingScreenPrefab' is not set");
                }

                if(LevelLoaderPrefab == null)
                {
                    errors.Add("'LevelLoaderPrefab' is not set");
                }

                if(errors.Count > 0)
                {
                    throw new ApplicationException("There are missing settings in GameInstaller:\n" + string.Join("\n", errors.ToArray()));
                }
            }
        }
    }
}
