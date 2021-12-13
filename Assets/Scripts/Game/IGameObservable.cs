using System;

namespace Checkers
{
    public interface IGameObservable
    {
        event OnStepEventHandler OnStep;
        event OnEatChipEventHandler OnEatChip;
        event OnSelectChipEventHandler OnSelectChip;
        event Action OnStartGame;
        event Action OnExitGame;
    }
}