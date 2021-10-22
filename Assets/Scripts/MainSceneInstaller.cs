using UnityEngine;
using Zenject;

namespace Checkers
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField]
        private GameController _gameController;

        public override void InstallBindings()
        {
            Container.Bind<IGameObservable>().To<GameController>().FromInstance(_gameController).AsSingle();
        }
    }
}