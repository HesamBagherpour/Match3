using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3;

namespace Garage.Match3.BoardStates
{
    public class BoosterInfo
    {
        public BoosterType Type;
        public Cell OriginCell;
        public List<Cell> cells;
    }
    public class DetectBoosterState : BoardState
    {
        private readonly IBoardView _boardView;
        private bool _finished;

        public DetectBoosterState(IBoardView boardView)
        {
            _boardView = boardView;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            Agent.BoosterInfo = ConvertMatchInfosToBooster();
            _finished = true;
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

        private List<BoosterInfo> ConvertMatchInfosToBooster()
        {
            List<BoosterInfo> boosterInfos = new List<BoosterInfo>();
            foreach (var matchInfo in Agent.MatchInfos)
            {
                if (matchInfo.MatchedCells.Count == 3) continue;
                switch (matchInfo.matchType)
                {
                    case MatchType.Horizontal:
                        boosterInfos.Add(new BoosterInfo()
                        {
                            cells = matchInfo.MatchedCells,
                            OriginCell = matchInfo.OriginCell,
                            Type = BoosterType.Vertical
                        });
                        break;
                    case MatchType.Vertical:
                        boosterInfos.Add(new BoosterInfo()
                        {
                            cells = matchInfo.MatchedCells,
                            OriginCell = matchInfo.OriginCell,
                            Type = BoosterType.Horizontal
                        });
                        break;
                    case MatchType.Rainbow:
                        boosterInfos.Add(new BoosterInfo()
                        {
                            cells = matchInfo.MatchedCells,
                            OriginCell = matchInfo.OriginCell,
                            Type = BoosterType.Rainbow
                        });
                        break;
                    case MatchType.Cross:
                        boosterInfos.Add(new BoosterInfo()
                        {
                            cells = matchInfo.MatchedCells,
                            OriginCell = matchInfo.OriginCell,
                            Type = BoosterType.Cross
                        });
                        break;
                    case MatchType.Square:
                        boosterInfos.Add(new BoosterInfo()
                        {
                            cells = matchInfo.MatchedCells,
                            OriginCell = matchInfo.OriginCell,
                            Type = BoosterType.Square
                        });
                        break;
                    default:
                        break;
                }
            }
            return boosterInfos;
        }


    }
}