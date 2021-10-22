namespace Checkers
{
    public interface IGameObservable
    {
        event OnStepEventHandler OnStep;
        event OnEatChipEventHandler OnEatChip;
        event OnSelectChipEventHandler OnSelectChip;
    }
}