using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3;

namespace Garage.Match3.BoardStates
{
    public class CannonFireState : BoardState
    {
        private int cannonFireCounter;
        private bool _finished;
        private readonly List<CannonModule> cannons;

        public CannonFireState()
        {
            cannons = new List<CannonModule>();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            SetupCannons();
            FireCannons();
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

        private void SetupCannons()
        {
            cannons.Clear();
            var width = Agent.Width;
            var height = Agent.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = Cells[x, y];
                    if (cell.IsLocked(ActionType.HitBlock, Direction.Center) == false)
                    {
                        CannonModule cannon = cell.GetModule<CannonModule>();
                        if (cannon != null)
                        {
                            List<Cell> cells = GetCellsInDirection(cell.position, cannon.direction);
                            cannon.SetCells(cells);
                            cannons.Add(cannon);
                        }
                    }
                }
            }
        }
        private List<Cell> GetCellsInDirection(Point position, Direction direction)
        {
            List<Cell> result = new List<Cell>();
            switch (direction)
            {
                case Direction.Top:
                    for (int y = position.y + 1; y <= Agent.Height; y++)
                    {
                        if (Agent.IsInBounds(position.x, y))
                        {
                            Cell cell = Cells[position.x, y];
                            // if bottom is locked with wall, break immedietly
                            if (cell.IsLocked(ActionType.BlockCannon, Direction.Bottom))
                                break;
                            result.Add(cell);
                            // if top is locked with wall, add current cell to lock list then break loop
                            if (cell.IsLocked(ActionType.BlockCannon, Direction.Top))
                                break;
                        }
                    }
                    break;
                case Direction.Left:
                    for (int x = position.x - 1; x >= 0; x--)
                    {
                        if (Agent.IsInBounds(x, position.y))
                        {
                            Cell cell = Cells[x, position.y];
                            // if right is locked with wall, break immedietly
                            if (cell.IsLocked(ActionType.BlockCannon, Direction.Right))
                                break;
                            result.Add(cell);
                            // if left is locked with wall, add current cell to lock list then break loop
                            if (cell.IsLocked(ActionType.BlockCannon, Direction.Left))
                                break;
                        }
                    }
                    break;
                case Direction.Bottom:
                    for (int y = position.y - 1; y >= 0; y--)
                    {
                        if (Agent.IsInBounds(position.x, y))
                        {
                            Cell cell = Cells[position.x, y];
                            // if top is locked with wall, break immedietly
                            if (cell.IsLocked(ActionType.BlockCannon, Direction.Top))
                                break;
                            result.Add(cell);
                            // if bottom is locked with wall, add current cell to lock list then break loop
                            if (cell.IsLocked(ActionType.BlockCannon, Direction.Bottom))
                                break;
                        }
                    }
                    break;
                case Direction.Right:
                    for (int x = position.x + 1; x <= Agent.Width; x++)
                    {
                        if (Agent.IsInBounds(x, position.y))
                        {
                            Cell cell = Cells[x, position.y];
                            // if left is locked with wall, break immedietly
                            if (cell.IsLocked(ActionType.BlockCannon, Direction.Left))
                                break;
                            result.Add(cell);
                            // if right is locked with wall, add current cell to lock list then break loop
                            if (cell.IsLocked(ActionType.BlockCannon, Direction.Right))
                                break;
                        }
                    }
                    break;
                case Direction.None:
                case Direction.Center:
                case Direction.All:
                default:
                    break;
            }

            return result;
        }

        private void FireCannons()
        {
            if (cannons.Count == 0)
            {
                _finished = true;
                return;
            }
            cannonFireCounter = cannons.Count;
            for (int i = 0; i < cannons.Count; i++)
            {
                _finished = false;
                cannons[i].Fire(OnFireComplete);
            }
        }

        private void OnFireComplete()
        {
            cannonFireCounter--;
            if (cannonFireCounter == 0)
            {
                _finished = true;
            }
        }
    }
}