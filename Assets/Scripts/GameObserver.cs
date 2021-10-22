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
        [SerializeField]
        private GameController _gameManager;

        private void Awake()
        {   
            _gameManager.OnEatChip += OnEatChip;
            _gameManager.OnSelectChip +=OnSelectChip;
            _gameManager.OnStep += OnStep;
        }

        private void OnStep(object sender, GameController.OnStepArgs args) {
            Debug.Log("Step of " + args.Side.ToString() + "\nFrom: " + args.From.Item1 + ", " + args.From.Item2 + "\nTo: " + args.To.Item1 + ", " + args.To.Item2);
        }

        private void OnSelectChip(object sender, GameController.CellCoordsEventArgs args) {
            Debug.Log("SelectChip: " + args.x + ", " + args.z);
        }

        private void OnEatChip(object sender, GameController.CellCoordsEventArgs args) {
            Debug.Log("EatChip: " + args.x + ", " + args.z);
        }
    }
}