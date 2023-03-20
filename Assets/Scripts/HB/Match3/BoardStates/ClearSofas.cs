using System.Collections.Generic;
using HB.Match3.BoardModules;

namespace Garage.Match3.BoardStates
{
    public class ClearSofas : BoardState
    {
        private bool _finished;
        protected override void OnEnter()
        {
            base.OnEnter();
            _finished = true;
            List<SofaModule> removableSofas = new List<SofaModule>();
            foreach (var sofa in Agent.SofaList)
            {
                if (sofa.ShouldClear())
                {
                    removableSofas.Add(sofa);
                }
            }

            foreach (var removableSofa in removableSofas)
            {
                Agent.SofaList.Remove(removableSofa);
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished)
            {
                Finished();
                _finished = false;
            }
        }
    }
}