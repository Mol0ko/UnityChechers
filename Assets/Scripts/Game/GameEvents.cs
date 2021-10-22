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

        public OnStepArgs(string serializedString) {
            var items = serializedString.Split('/');
            Side = (ColorType)byte.Parse(items[0]);
            var xFrom = int.Parse(items[1].Split(',')[0]);
            var zFrom = int.Parse(items[1].Split(',')[1]);
            var xTo = int.Parse(items[2].Split(',')[0]);
            var zTo = int.Parse(items[2].Split(',')[1]);
            From = new Tuple<int, int>(xFrom, zFrom);
            To = new Tuple<int, int>(xTo, zTo);
        }

        public string toSerializedString() => $"{(byte)Side}/{From.Item1},{From.Item2}/{To.Item1},{To.Item2}";
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