namespace Garage.Match3.BoardStates
{
    public class InitState : BoardState
    {
        #region Protected Methods

        protected override void OnEnter()
        {
            base.OnEnter();
            Finished();
        }

        #endregion
    }
}