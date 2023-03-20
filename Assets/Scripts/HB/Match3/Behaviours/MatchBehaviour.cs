﻿using System;
using System.Collections.Generic;
using Garage.Match3;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Logger;

namespace HB.Match3.Behaviours
{
    public class MatchBehaviour
    {
        #region Private Fields

        private readonly Board _board;

        #endregion

        #region  Constructors

        public MatchBehaviour(Board board)
        {
            _board = board;
        }

        #endregion

        #region Public Methods

        public void CheckMatches(Action onSuccess = null, Action onFailed = null)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    Cell cell = _board.Cells[x, y];

                    if (cell.Contains<ExitModule>()) CheckPlantToExit(x, y);

                    BlockType bt = cell.GetBlockType();
                    if (_board.IsIgnoredBlockType(bt)) continue;
                    CheckHorizontalMatches(x, y, bt);
                    CheckVerticalMatches(x, y, bt);
                    CheckSquareMatch(x, y, bt);
                }
            }

            if (_board.MatchInfos.Count > 0)
                onSuccess?.Invoke();
            else
                onFailed?.Invoke();
        }

        private void CheckPlantToExit(int x, int y)
        {
            var cell = _board.Cells[x, y];
            if (cell.Contains<ExitModule>() && _board.IsInBounds(x, y + 1))
            {
                Cell topCell = _board.Cells[x, y + 1];
                if (topCell.IsRestrictedBlock(ActionType.HitBlock))
                {
                    Log.Debug("Match3", "Adding plant to matchInfos");
                    AddToMatchInfos(topCell, new List<Cell> { topCell }, MatchType.ExittingPlant);
                }
            }
        }

        #endregion

        #region Private Methods

        private void CheckVerticalMatches(int x, int y, BlockType bt)
        {
            Cell origin = _board.Cells[x, y];
            if (origin.MatchType.HasFlag(MatchType.Vertical)) return;

            int dy = y + 1;
            List<Cell> matchedCells = new List<Cell> { origin };

            while (dy < _board.Height)
            {
                Cell otherCell = _board.Cells[x, dy];
                BlockType _bt = otherCell.GetBlockType();
                if (otherCell.IsLocked(ActionType.Match, Direction.All) == false && _bt.CanMatchWith(bt) && !otherCell.IsRestrictedBlock(ActionType.Match))
                {
                    origin = GetSwappedOrigin(otherCell, origin);
                    matchedCells.Add(otherCell);
                    dy++;
                }
                else
                {
                    break;
                }
            }

            AddToMatchInfos(origin, matchedCells, MatchType.Vertical);
        }

        private Cell GetSwappedOrigin(Cell cell, Cell origin)
        {
            if (origin == _board.LastValidSwap.cell || origin == _board.LastValidSwap.otherCell)
                return origin;
            if (cell == _board.LastValidSwap.cell || cell == _board.LastValidSwap.otherCell)
                return cell;

            return null;
        }

        private void CheckHorizontalMatches(int x, int y, BlockType bt)
        {
            Cell origin = _board.Cells[x, y];
            if (origin.MatchType.HasFlag(MatchType.Horizontal)) return;
            int dx = x + 1;
            List<Cell> matchedCells = new List<Cell> { origin };

            while (dx < _board.Width)
            {
                Cell otherCell = _board.Cells[dx, y];
                BlockType _bt = otherCell.GetBlockType();
                if (otherCell.IsLocked(ActionType.Match, Direction.All) == false && bt.CanMatchWith(_bt) && !otherCell.IsRestrictedBlock(ActionType.Match))
                {
                    origin = GetSwappedOrigin(otherCell, origin);
                    matchedCells.Add(otherCell);
                    dx++;
                }
                else
                {
                    break;
                }
            }

            AddToMatchInfos(origin, matchedCells, MatchType.Horizontal);
        }

        private void CheckSquareMatch(int x, int y, BlockType bt)
        {
            if (x == _board.Width - 1 || y == _board.Height - 1) return;
            Cell origin = _board.Cells[x, y];
            if (origin.MatchType.HasFlag(MatchType.Square)) return;

            List<Cell> matchedCells = new List<Cell> { origin };

            Point otherPos = new Point(x + 1, y);
            origin = CheckMatchAndStore(ref otherPos, bt, origin, matchedCells);

            otherPos.y++;
            origin = CheckMatchAndStore(ref otherPos, bt, origin, matchedCells);

            otherPos.x--;
            origin = CheckMatchAndStore(ref otherPos, bt, origin, matchedCells);

            AddToMatchInfos(origin, matchedCells, MatchType.Square);
        }

        private Cell CheckMatchAndStore(ref Point point, BlockType bt, Cell origin, List<Cell> matchedCells)
        {
            Cell otherCell = _board.Cells[point.x, point.y];

            BlockType _bt = otherCell.GetBlockType();
            if (otherCell.IsLocked(ActionType.Match, Direction.All) == false && _bt.CanMatchWith(bt) && !otherCell.IsRestrictedBlock(ActionType.Match))
            {
                origin = GetSwappedOrigin(otherCell, origin);
                matchedCells.Add(otherCell);
            }

            return origin;
        }

        private void AddToMatchInfos(Cell origin, List<Cell> matchedCells, MatchType type)
        {
            if (matchedCells.Count <= 2 && type != MatchType.ExittingPlant) return;
            if (type == MatchType.Square && matchedCells.Count != 4) return;
            for (int i = 0; i < matchedCells.Count; i++)
            {
                if (matchedCells[i].IsLocked(ActionType.Match, Direction.Center)) return;
                matchedCells[i].AddMatchType(type);
            }
            if (origin == null) origin = matchedCells[0];

            MatchInfo info = new MatchInfo
            {
                OriginCell = origin,
                matchType = type,
                MatchedCells = matchedCells
            };

            _board.MatchInfos.Add(info);
        }

        #endregion
    }
}