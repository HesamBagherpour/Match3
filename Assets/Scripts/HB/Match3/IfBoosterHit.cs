using Garage.Match3.BoardStates;
using HB.StateMachine;

namespace HB.Match3
{
    public class IfBoosterHit : Condition
    {
        public override bool CheckCondition()
        {
            return HitBoosterState.BoosterHitted;
        }
    }
    public class IfPostClearNeedNewClear : Condition
    {
        public override bool CheckCondition()
        {
            return PostClearState.UpdateNeeded;
        }
    }
}