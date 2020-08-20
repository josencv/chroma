using System;
using Chroma.Behaviour.Attack;
using UnityEngine;
using Zenject;

namespace Chroma.Game.Configuration
{
    /// <summary>
    /// This is the 'per-scene' DI installer of the game, meant to be used in a zenject scene context
    /// </summary>
    public class CombatInstaller : MonoInstaller
    {
        [SerializeField]
        private Settings settings;

        public override void InstallBindings()
        {
            Container.Bind<Weapon>().FromComponentInNewPrefab(settings.stickPrefab).AsTransient();
        }

        [Serializable]
        public class Settings
        {
            public Weapon stickPrefab;
        }
    }
}
