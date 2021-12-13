using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Checkers
{
    public class GameController : MonoBehaviour, IGameController, IGameObservable
    {

        #region Serializable

        [Serializable]
        public class FieldRow
        {
            public ChipComponent[] Chips;
        }

        [SerializeField]
        private FieldRow[] _chipsOnField;
        [SerializeField]
        private CellComponent[] _cells;
        [SerializeField]
        private Material _focusedMaterial;
        [SerializeField]
        private CameraMove _cameraAxis;

        #endregion

        #region Events

        public event OnStepEventHandler OnStep;
        public event OnEatChipEventHandler OnEatChip;
        public event OnSelectChipEventHandler OnSelectChip;
        public event Action OnStartGame;
        public event Action OnExitGame;

        #endregion

        private CellComponent _focusedCell;
        private ChipComponent _selectedChip;
        private Tuple<int, int> _selectedChipPosition;
        private ColorType _turnSide = ColorType.White;

        #region Lifecycle

        private void Start()
        {
            foreach (var cell in _cells)
            {
                cell.OnFocusEventHandler += OnFocusEventHandler;
                cell.OnClickEventHandler += OnCellClickEventHandler;
            }
            OnStartGame?.Invoke();
        }

        private void Update()
        {
            UpdateFocusedCell();
            UpdateChipSelection();
            UpdateExitGame();
        }

        #endregion

        #region Private methods

        private void OnFocusEventHandler(CellComponent cell, bool focused)
        {
            if (focused)
                cell.AddAdditionalMaterial(_focusedMaterial);
            else
                cell.RemoveAdditionalMaterial();
        }

        private void OnCellClickEventHandler(BaseClickComponent cell)
        {
            var x = (int)cell.transform.position.x;
            var z = -(int)cell.transform.position.z;
            Debug.Log($"OnClick cell: {x} , {z}");

            var chip = _chipsOnField[z]?.Chips[x];
            if (chip != null && chip.GetColor == _turnSide)
            {
                SelectChip(chip);
                _selectedChipPosition = new Tuple<int, int>(x, z);
                var onSelectChipArgs = new CellCoordsEventArgs(x, z);
                OnSelectChip?.Invoke(this, onSelectChipArgs);
            }
            else if (_selectedChip != null)
            {
                var cellIsEmpty = _chipsOnField[z].Chips[x] == null;
                var isDiagonalStep = Math.Abs(x - _selectedChipPosition.Item1) == Math.Abs(z - _selectedChipPosition.Item2);
                var canMakeStep = cellIsEmpty && isDiagonalStep && _focusedCell.GetColor == ColorType.Black && _selectedChip.GetColor == _turnSide;
                if (canMakeStep)
                {

                    MakeStep(_selectedChipPosition.Item1, _selectedChipPosition.Item2, x, z);
                    string winMessage;
                    if (CheckWin(out winMessage))
                        Debug.Log(winMessage);
                }
                else
                {
                    var message = "Step is invalid";
                    if (!cellIsEmpty)
                        message = "Target cell is not empty";
                    else if (!isDiagonalStep)
                        message = "Step should be diagonal";
                    else if (_focusedCell.GetColor != ColorType.Black)
                        message = "Target cell should be black";
                    else if (_selectedChip.GetColor == _turnSide)
                        message = "Turn to " + _turnSide.ToString();
                    Debug.Log(message);
                }
            }
        }

        private void UpdateFocusedCell()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            var hittingCell = hit.transform?.GetComponent<CellComponent>();
            if (hittingCell != null && _focusedCell != hittingCell)
            {
                if (_focusedCell != null)
                {
                    _focusedCell.OnPointerExit();
                    _focusedCell = null;
                }
                hittingCell.OnPointerEnter();
                _focusedCell = hittingCell;
            }
        }

        private void UpdateChipSelection()
        {
            var selectCellCondition = false;

#if UNITY_EDITOR
            selectCellCondition = Input.GetMouseButtonDown(0);
#else
            selectCellCondition = Input.GetMouseButtonDown(1);
#endif

            if (selectCellCondition)
                _focusedCell?.OnPointerClick();
        }


        private void UpdateExitGame()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                OnExitGame?.Invoke();
            }
        }

        private void SelectChip(ChipComponent chip)
        {
            if (_selectedChip != null)
                _selectedChip.RemoveAdditionalMaterial();

            _selectedChip = chip;
            chip.AddAdditionalMaterial(_focusedMaterial);
        }

        public void MakeStep(int xFrom, int zFrom, int xTo, int zTo)
        {
            if (_selectedChip == null)
            {
                _selectedChip = _chipsOnField[zFrom].Chips[xFrom];
            }
            var eatenEnemy = EatenEnemyForStep(xFrom, zFrom, xTo, zTo);
            if (eatenEnemy != null)
            {
                var eatenChip = _chipsOnField[eatenEnemy.Item2].Chips[eatenEnemy.Item1];
                Destroy(eatenChip.gameObject);

                var onEatChipArgs = new CellCoordsEventArgs(eatenEnemy.Item1, eatenEnemy.Item2);
                OnEatChip?.Invoke(this, onEatChipArgs);

                _chipsOnField[eatenEnemy.Item2].Chips[eatenEnemy.Item1] = null;
            }

            _chipsOnField[zTo].Chips[xTo] = _selectedChip;
            _chipsOnField[zFrom].Chips[xFrom] = null;
            _selectedChip.GetComponent<ChipStepMove>().MoveToPosition(xTo, -zTo);
            _cameraAxis.MoveToAnotherSide();
            _selectedChip.RemoveAdditionalMaterial();
            _selectedChip = null;
            _selectedChipPosition = null;

            var onStepArgs = new OnStepArgs(
                _turnSide,
                new Tuple<int, int>(xFrom, zFrom),
                new Tuple<int, int>(xTo, zTo)
            );
            OnStep?.Invoke(this, onStepArgs);

            switch (_turnSide)
            {
                case ColorType.White:
                    _turnSide = ColorType.Black;
                    break;
                case ColorType.Black:
                    _turnSide = ColorType.White;
                    break;
            }
        }

        private Tuple<int, int> EatenEnemyForStep(int xFrom, int zFrom, int xTo, int zTo)
        {
            var cellsCrossedCount = Math.Abs(xTo - xFrom);
            var longStep = (cellsCrossedCount) == 2;
            if (longStep)
            {
                for (var i = 1; i < cellsCrossedCount; i++)
                {
                    var xToCheck = xTo - xFrom > 0 ? xFrom + i : xFrom - i;
                    var zToCheck = zTo - zFrom > 0 ? zFrom + i : zFrom - i;
                    if (_chipsOnField[zToCheck].Chips[xToCheck] != null)
                    {
                        return new Tuple<int, int>(xToCheck, zToCheck);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        private bool CheckWin(out string winMessage)
        {
            var allChips = _chipsOnField
                .SelectMany(row => row.Chips)
                .Where(chip => chip != null);
            var whiteWin = allChips.Where(chip => chip.GetColor == ColorType.Black).Count() <= 0
                || _chipsOnField[7].Chips.Where(chip => chip != null && chip.GetColor == ColorType.White).Count() > 0;
            var blackWin = allChips.Where(chip => chip.GetColor == ColorType.White).Count() <= 0
                || _chipsOnField[0].Chips.Where(chip => chip != null && chip.GetColor == ColorType.Black).Count() > 0;
            winMessage = whiteWin ? "White win!" : (blackWin ? "Black win" : "");
            return whiteWin || blackWin;
        }

        #endregion
    }
}