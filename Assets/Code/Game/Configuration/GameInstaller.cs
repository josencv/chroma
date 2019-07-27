using System;
using Chroma.Assets.Code.Game;
using Chroma.Game.LevelManagement;
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
            Container.Bind<LevelLoader>().AsSingle();
            Container.Bind<GameLoop>().FromComponentInNewPrefab(settings.GameLoopPrefab).AsSingle().NonLazy();
            Container.Bind<InputManager>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public GameLoop GameLoopPrefab;
        }
    }
}
