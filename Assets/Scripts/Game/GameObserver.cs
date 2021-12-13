using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Zenject;

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
        [Inject]
        private IGameObservable _gameObservable;
        [Inject]
        private IGameController _gameController;

        private string _gameLogPath
        {
            get => Application.persistentDataPath + "/Game";
        }

        private void Awake()
        {
            _gameObservable.OnEatChip += OnEatChip;
            _gameObservable.OnSelectChip += OnSelectChip;
            _gameObservable.OnStep += OnStep;
            _gameObservable.OnStartGame += OnStartGame;
            _gameObservable.OnExitGame += OnExitGame;

            if (_mode == ObserverMode.Observe)
            {
                if (File.Exists(_gameLogPath))
                    File.Delete(_gameLogPath);
            }
            else
            {
                if (File.Exists(_gameLogPath))
                {
                    StopAllCoroutines();
                    StartCoroutine(PlayGameFromFile());
                }
                else
                {
                    Debug.Log("File not found");
                }
            }
        }

        private IEnumerator PlayGameFromFile()
        {
            yield return new WaitForSeconds(1f);
            foreach (string line in File.ReadAllLines(_gameLogPath))
            {
                if (!line.StartsWith("["))
                {
                    var step = new OnStepArgs(line);
                    _gameController.MakeStep(
                        step.From.Item1,
                        step.From.Item2,
                        step.To.Item1,
                        step.To.Item2
                    );
                    yield return new WaitForSeconds(1.5f);
                }
            }
        }

        private void OnStep(object sender, OnStepArgs args)
        {
            Debug.Log("Step of " + args.Side.ToString() + "\nFrom: " + args.From.Item1 + ", " + args.From.Item2 + "\nTo: " + args.To.Item1 + ", " + args.To.Item2);
            var stepString = args.toSerializedString();
            AddToFile(stepString);
        }

        private void OnSelectChip(object sender, CellCoordsEventArgs args)
        {
            Debug.Log("SelectChip: " + args.x + ", " + args.z);
            string json = JsonUtility.ToJson(args);
        }

        private void OnEatChip(object sender, CellCoordsEventArgs args)
        {
            Debug.Log("EatChip: " + args.x + ", " + args.z);
            string json = JsonUtility.ToJson(args);
        }

        private void OnStartGame()
        {
            var time = DateTime.Now.ToLongTimeString();
            var logString = "[" + time + "] Start Game";
            Debug.Log(logString);
            AddToFile(logString);
        }

        private void OnExitGame()
        {
            var time = DateTime.Now.ToLongTimeString();
            var logString = "[" + time + "] Exit Game";
            Debug.Log(logString);
            AddToFile(logString);
        }

        private void AddToFile(string content)
        {
            if (File.Exists(_gameLogPath))
                using (StreamWriter sw = File.AppendText(_gameLogPath))
                {
                    sw.WriteLine(content);
                }
            else
                File.WriteAllText(_gameLogPath, content + "\n");

            Debug.Log("Written to file");
        }
    }
}