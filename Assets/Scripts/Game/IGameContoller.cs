namespace Checkers
{
    public interface IGameController
    {
        void MakeStep(int xFrom, int zFrom, int xTo, int zTo);
    }
}