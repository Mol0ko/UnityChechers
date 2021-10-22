using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Checkers
{
    public class GameObserver : MonoBehaviour
    {
        public enum ObserverMode
        {
            Observe,
            Play
        }

        [SerializeField]
        private ObserverMode _mode = ObserverMode.Observe;
        private IGameObservable _gameController;
        private readonly string _gameLogPath = Application.persistentDataPath + "/Game";

        private void Awake()
        {
            _gameController.OnEatChip += OnEatChip;
            _gameController.OnSelectChip += OnSelectChip;
            _gameController.OnStep += OnStep;

            if (_mode == ObserverMode.Observe)
            {
                if (File.Exists(_gameLogPath))
                    File.Delete(_gameLogPath);
            }
            else
            {

            }
        }

        private void OnStep(object sender, OnStepArgs args)
        {
            Debug.Log("Step of " + args.Side.ToString() + "\nFrom: " + args.From.Item1 + ", " + args.From.Item2 + "\nTo: " + args.To.Item1 + ", " + args.To.Item2);
        }

        private void OnSelectChip(object sender, CellCoordsEventArgs args)
        {
            Debug.Log("SelectChip: " + args.x + ", " + args.z);
        }

        private void OnEatChip(object sender, CellCoordsEventArgs args)
        {
            Debug.Log("EatChip: " + args.x + ", " + args.z);
        }

        private void WriteToFile(IEnumerable<string> contents)
        {
            File.WriteAllLines(_gameLogPath, contents);
        }
    }
}