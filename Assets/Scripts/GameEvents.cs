using System;

namespace Checkers
{
    public class OnStepArgs : EventArgs
    {
        public readonly ColorType Side;
        public readonly Tuple<int, int> From;
        public readonly Tuple<int, int> To;

        public OnStepArgs(ColorType side, Tuple<int, int> from, Tuple<int, int> to)
        {
            Side = side;
            From = from;
            To = to;
        }
    }
    public class CellCoordsEventArgs : EventArgs
    {
        public readonly int x;
        public readonly int z;

        public CellCoordsEventArgs(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }

    public delegate void OnStepEventHandler(object sender, OnStepArgs args);
    
    public delegate void OnEatChipEventHandler(object sender, CellCoordsEventArgs args);

    public delegate void OnSelectChipEventHandler(object sender, CellCoordsEventArgs args);
}