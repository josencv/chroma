using System;
using Chroma.Assets.Code.Game;
using Chroma.Game.LevelManagement;
using Chroma.Game.LoadingScreens;
using Chroma.Infrastructure.Input;
using UnityEngine;
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
            Container.Bind<GameLoop>().FromComponentInNewPrefab(settings.GameLoopPrefab).AsSingle().NonLazy();
            Container.Bind<InputManager>().AsSingle();
            Container.Bind<LoadingScreen>().FromComponentInNewPrefab(settings.LoadingScreenPrefab).AsSingle().NonLazy();
        }

        [Serializable]
        public class Settings
        {
            public GameLoop GameLoopPrefab;
            public LevelLoader LevelLoaderPrefab;
            public LoadingScreen LoadingScreenPrefab;
        }
    }
}
