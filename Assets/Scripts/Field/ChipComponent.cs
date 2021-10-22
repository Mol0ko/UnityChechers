using System;

using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        public override void OnPointerEnter()
        {
            CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit()
        {
            CallBackEvent((CellComponent)Pair, false);
        }
    }
}