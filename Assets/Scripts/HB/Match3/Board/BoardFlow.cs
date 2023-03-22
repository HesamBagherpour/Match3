using System.Collections.Generic;
using HB.Match3.Behaviours;
using HB.Match3.Cell;
using HB.Match3.Modules;

namespace HB.Match3.Board
{
    public class BoardFlow
    {
        private static List<Flow> _flows;
        private readonly global::HB.Match3.Match3MainBoard.Board _board;
        private static List<Flow> _subFlows;
        private static int _subFlowIterateCount;
        private List<Flow> _rootFlows = new List<Flow>();

        public BoardFlow(global::HB.Match3.Match3MainBoard.Board board)
        {
            _board = board;
            _flows = new List<Flow>();
            _subFlows = new List<Flow>();
            CreateFlows();
        }

        public List<Flow> GetFlows()
        {
            List<Flow> allFlows = new List<Flow>(_rootFlows.Count + _subFlows.Count);
            allFlows.AddRange(_rootFlows);
            allFlows.AddRange(_subFlows);
            return allFlows;
        }

        private void CreateFlows()
        {
            _flows.Clear();
            _subFlows.Clear();
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = _board.Height - 1; y >= 0; y--)
                {
                    MyCell cell = _board.Cells[x, y];
                    if (cell.IsVisible) cell.flow = null;
                }
            }
            CreateFlowFromSpawner();
            CreateOrphanedFlows();
            _subFlowIterateCount = _board.Width;
            SetOrphanFlowsProvider();
            UpdateInvisibleBlockerFlows();
            // for (int i = 0; i < _flows.Count; i++)
            // {
            //     Log.Debug("Match3", _flows[i].ToString());
            // }
        }

        private void CreateFlowFromSpawner()
        {

            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    MyCell cell = _board.Cells[x, y];

                    if (cell.Contains<SpawnerModule>())
                    {
                        Flow flow = new Flow(null);
                        cell.flow = flow;
                        _flows.Add(flow);
                        _rootFlows.Add(flow);
                        FillFlowCellsTillBlocker(flow, cell.position);
                        var startCell = _board.Cells[x, y - 1];
                        if (CanTradeBlock(cell, startCell))
                        {
                            flow.Incommings.Add((cell, startCell));
                        }
                    }
                }
            }
        }


        private void FillFlowCellsTillBlocker(Flow flow, Point pos)
        {
            int x = pos.x;
            for (int y = pos.y - 1; y >= 0; y--)
            {
                MyCell cell = _board.Cells[x, y];
                var topPos = new Point(x, y + 1);
                if (_board.IsInBounds(topPos))
                {
                    MyCell topCell = _board.Cells[topPos.x, topPos.y];
                    if (CanTradeBlock(topCell, cell))
                    {
                        flow.AddCell(cell);
                    }
                    else break;
                }
                else break;
            }
        }


        private void CreateOrphanedFlows()
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = _board.Height - 1; y >= 0; y--)
                {
                    MyCell cell = _board.Cells[x, y];
                    if (cell.flow != null) continue;
                    if (cell.IsVisible)
                    {
                        Flow flow = new Flow(null);
                        flow.AddCell(cell);
                        _subFlows.Add(flow);
                        int dy = y - 1;
                        while (dy >= 0)
                        {
                            MyCell otherCell = _board.Cells[x, dy];
                            var topPos = new Point(x, dy + 1);
                            if (_board.IsInBounds(topPos))
                            {
                                var topCell = _board.Cells[topPos.x, topPos.y];
                                if (CanTradeBlock(topCell, otherCell))
                                {
                                    flow.AddCell(otherCell);
                                }
                                else break;
                            }
                            else break;
                            dy--;
                        }
                    }
                }
            }
        }

        private void SetOrphanFlowsProvider()
        {
            int mainFlowCount = _flows.Count;
            for (int _height = _board.Height - 1; _height >= 0; _height--)
            {
                for (int i = 0; i < mainFlowCount; i++)
                {
                    Flow mainFlow = _flows[i];
                    SetChildrenAtHeight(_height, mainFlow);
                    // Set subflow children providers
                    for (int j = 0; j < mainFlow.subFlows.Count; j++)
                    {
                        SetChildrenAtHeight(_height, mainFlow.subFlows[j]);
                    }
                }
            }


            _subFlowIterateCount--;
            if (_subFlowIterateCount > 0)
            {
                SetOrphanFlowsProvider();
            }
        }

        private void UpdateInvisibleBlockerFlows()
        {
            foreach (var cell in _board.Cells)
            {
                // if cell is invisibleBlocker
                if (cell.IsVisible == false && cell.Contains<InvisibleBlocker>())
                {
                    // if topCell of invisivleBlocker is a visible cell and it is not locked by any restrictions:
                    Point topPos = new Point(cell.position.x, cell.position.y + 1);
                    if (_board.IsInBounds(topPos))
                    {
                        MyCell topCell = _board.Cells[topPos.x, topPos.y];
                        if (topCell.IsVisible && topCell.IsLocked(ActionType.Move, Direction.Center) == false)
                        {
                            // find left cell and right cell of the invisible blocker and if they are free cells, set topcell as their provider
                            Point leftPos = new Point(cell.position.x - 1, cell.position.y);
                            if (_board.IsInBounds(leftPos))
                            {
                                MyCell leftCell = _board.Cells[leftPos.x, leftPos.y];
                                if (leftCell.IsVisible &&
                                    leftCell.IsLocked(ActionType.Move, Direction.Center) == false &&
                                    leftCell.flow != null &&
                                    leftCell.Contains<CannonModule>() == false &&
                                    // leftCell.flow.Incommings.Count == 0 &&
                                    topCell.flow.Incommings.Count != 0)
                                {
                                    leftCell.flow.Incommings.Add((topCell, leftCell));
                                }
                            }
                            Point rightPos = new Point(cell.position.x + 1, cell.position.y);
                            if (_board.IsInBounds(rightPos))
                            {
                                MyCell rightCell = _board.Cells[rightPos.x, rightPos.y];
                                if (rightCell.IsVisible &&
                                    rightCell.IsLocked(ActionType.Move, Direction.Center) == false &&
                                    rightCell.flow != null &&
                                    rightCell.Contains<CannonModule>() == false &&
                                    // rightCell.flow.Incommings.Count == 0 &&
                                    topCell.flow.Incommings.Count != 0)
                                {
                                    rightCell.flow.Incommings.Add((topCell, rightCell));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetChildrenAtHeight(int _height, Flow mainFlow)
        {
            if (mainFlow.Incommings.Count == 0) return;
            if (_height > mainFlow.Incommings[0].startCell.position.y) return;

            int startIndex = mainFlow.Cells.FindIndex(c => c.position.y == _height);
            if (startIndex < 0) return;
            MyCell cell = mainFlow.Cells[startIndex];
            if (cell.IsVisible)
            {
                SetChildrenFlow(cell);
            }
        }

        private void SetChildrenFlow(MyCell origin)
        {
            Point pos = origin.position;
            if (_board.IsInBounds(pos.x - 1, pos.y - 1))
            {
                MyCell bottomLeft = _board.Cells[pos.x - 1, pos.y - 1];
                SetDownNeighborProvider(bottomLeft, origin);
            }
            if (_board.IsInBounds(pos.x + 1, pos.y - 1))
            {
                MyCell bottomRight = _board.Cells[pos.x + 1, pos.y - 1];
                SetDownNeighborProvider(bottomRight, origin);
            }
        }

        private void SetDownNeighborProvider(MyCell orphanCell, MyCell origin)
        {
            if (orphanCell.IsVisible == false || orphanCell.flow == null) return;
            if (orphanCell.flow.Incommings.Count == 0 && CanTradeBlock(origin, orphanCell))
            {
                orphanCell.flow.Incommings.Add((origin, orphanCell));
                origin.flow.AddFlow(orphanCell.flow);
                //SetChildren(cell);
                if (_subFlows.Contains(orphanCell.flow))
                {
                    _subFlows.Remove(orphanCell.flow);
                }

                if (_flows.Contains(orphanCell.flow) == false)
                {
                    _flows.Add(orphanCell.flow);
                }
            }
        }

        private bool CanTradeBlock(MyCell provider, MyCell receiver)
        {
            if (IsFreeCell(provider) == false || IsFreeCell(receiver) == false) return false;
            Direction providerDirection = GetDirection(provider.position, receiver.position);
            if (providerDirection == Direction.Top)
            {
                return CanTradeFromTop(provider, receiver);
            }
            else if (providerDirection == (Direction.Top | Direction.Left))
            {
                return CanTradeFromTopLeft(provider, receiver);
            }
            else if (providerDirection == (Direction.Top | Direction.Right))
            {
                return CanTradeFromTopRight(provider, receiver);
            }
            else return false;
        }

        private bool CanTradeFromTopRight(MyCell topRight, MyCell receiver)
        {
            // if cells are locked with any kinds of restrictions like wood or grass or iron
            if (topRight.IsLocked(ActionType.Move, Direction.Center)) return false;
            if (receiver.IsLocked(ActionType.Move, Direction.Center)) return false;

            MyCell top = null;
            MyCell right = null;
            if (_board.IsInBounds(receiver.position.x, receiver.position.y + 1)) top = _board.Cells[receiver.position.x, receiver.position.y + 1];
            if (_board.IsInBounds(receiver.position.x - 1, receiver.position.y)) right = _board.Cells[receiver.position.x + 1, receiver.position.y];

            // if there is └ at topRight:
            // if topRight has bottom and top has right restriction:
            if (topRight.IsLocked(ActionType.Move, Direction.Bottom, true) && top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Right, true)) return false;
            // if right has top restriction and top has right restriction:
            if (right != null && right.IsVisible && right.IsLocked(ActionType.Move, Direction.Top, true) &&
                top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Right, true)) return false;
            // if right has top and topRight has left
            if (right != null && right.IsVisible && right.IsLocked(ActionType.Move, Direction.Top, true) && topRight.IsLocked(ActionType.Move, Direction.Left, true)) return false;

            // if there is ┐ at receiver:
            // if top has bottom and receiver has right restriction:
            if (top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Bottom, true) && receiver.IsLocked(ActionType.Move, Direction.Right, true)) return false;
            // if right has left restriction and top has bottom restriction:
            if (right != null && right.IsVisible && right.IsLocked(ActionType.Move, Direction.Left, true) &&
                top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Bottom, true)) return false;
            // if receicer has top and right has left
            if (right != null && right.IsVisible && right.IsLocked(ActionType.Move, Direction.Left, true) && receiver.IsLocked(ActionType.Move, Direction.Top, true)) return false;

            // if there is -- at top 
            if (receiver.IsLocked(ActionType.Move, Direction.Top, true) && right != null && right.IsVisible && right.IsLocked(ActionType.Move, Direction.Top, true)) return false;
            if (receiver.IsLocked(ActionType.Move, Direction.Top, true) && topRight.IsLocked(ActionType.Move, Direction.Bottom, true)) return false;
            if (top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Bottom, true) && right != null && right.IsVisible && right.IsLocked(ActionType.Move, Direction.Top, true)) return false;
            if (top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Bottom, true) && topRight.IsLocked(ActionType.Move, Direction.Bottom, true)) return false;

            return true;
        }

        private bool CanTradeFromTopLeft(MyCell topLeft, MyCell receiver)
        {
            // if cells are locked with any kinds of restrictions like wood or grass or iron
            if (topLeft.IsLocked(ActionType.Move, Direction.Center)) return false;
            if (receiver.IsLocked(ActionType.Move, Direction.Center)) return false;

            MyCell top = null;
            MyCell left = null;
            if (_board.IsInBounds(receiver.position.x, receiver.position.y + 1)) top = _board.Cells[receiver.position.x, receiver.position.y + 1];
            if (_board.IsInBounds(receiver.position.x - 1, receiver.position.y)) left = _board.Cells[receiver.position.x - 1, receiver.position.y];

            // if there is ┘ at topLeft:
            // if topleft has bottom and top has left restriction:
            if (topLeft.IsLocked(ActionType.Move, Direction.Bottom, true) && top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Left, true)) return false;
            // if left has top restriction and top has left restriction:
            if (left != null && left.IsVisible && left.IsLocked(ActionType.Move, Direction.Top, true) &&
                top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Left, true)) return false;
            // if left has top and topLeft has right
            if (left != null && left.IsVisible && left.IsLocked(ActionType.Move, Direction.Top, true) && topLeft.IsLocked(ActionType.Move, Direction.Right, true)) return false;

            // if there is ┌ at receiver:
            // if top has bottom and receiver has left restriction:
            if (top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Bottom, true) && receiver.IsLocked(ActionType.Move, Direction.Left, true)) return false;
            // if left has right restriction and top has bottom restriction:
            if (left != null && left.IsVisible && left.IsLocked(ActionType.Move, Direction.Right, true) &&
                top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Bottom, true)) return false;
            // if receicer has top and left has right
            if (left != null && left.IsVisible && left.IsLocked(ActionType.Move, Direction.Right, true) && receiver.IsLocked(ActionType.Move, Direction.Top, true)) return false;

            // if there is -- at top 
            if (receiver.IsLocked(ActionType.Move, Direction.Top, true) && left != null && left.IsVisible && left.IsLocked(ActionType.Move, Direction.Top, true)) return false;
            if (receiver.IsLocked(ActionType.Move, Direction.Top, true) && topLeft.IsLocked(ActionType.Move, Direction.Bottom, true)) return false;
            if (top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Bottom, true) && left != null && left.IsVisible && left.IsLocked(ActionType.Move, Direction.Top, true)) return false;
            if (top != null && top.IsVisible && top.IsLocked(ActionType.Move, Direction.Bottom, true) && topLeft.IsLocked(ActionType.Move, Direction.Bottom, true)) return false;

            return true;
        }

        private bool CanTradeFromTop(MyCell provider, MyCell receiver)
        {
            if (provider.IsLocked(ActionType.Move, Direction.Bottom) ||
               receiver.IsLocked(ActionType.Move, Direction.Top))
            {
                return false;
            }
            return true;
        }

        Direction GetDirection(Point provider, Point receiver)
        {
            Direction direction = Direction.None;
            if (provider.x > receiver.x && provider.y > receiver.y)
            {
                direction = Direction.Top | Direction.Right;
            }
            else if (provider.x < receiver.x && provider.y > receiver.y)
            {
                direction = Direction.Top | Direction.Left;
            }
            else if (provider.x == receiver.x && provider.y > receiver.y)
            {
                direction = Direction.Top;
            }
            return direction;
        }

        private bool IsFreeCell(MyCell cell)
        {
            if (cell.Contains<ExitModule>()) return false;
            if (cell.Contains<CannonModule>()) return false;
            return true;
        }

        public void Visualize()
        {
            for (int i = 0; i < _flows.Count; i++)
            {
                _flows[i].Visualize();
            }

            for (int i = 0; i < _subFlows.Count; i++)
            {
                _subFlows[i].Visualize();
            }
        }
    }
}