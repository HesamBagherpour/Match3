using System;
using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3;
using UnityEngine;

namespace Garage.Match3.BoardStates
{
    public class HitBoosterState : BoardState
    {
        private bool _finished;
        private int boosterCount = 0;
        public static bool BoosterHitted { get; private set; }
        protected override void OnEnter()
        {
            base.OnEnter();
            boosterCount = 0;
            BoosterHitted = false;
            int width = Agent.Width;
            int height = Agent.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = Cells[x, y];
                    if (cell.IsVisible && cell.HitType == HitType.Direct && cell.Contains<BoosterModule>() && cell.IsLocked(ActionType.HitBlock, Direction.Center) == false)
                    {
                        boosterCount++;
                        BoosterHitted = true;
                        _finished = false;
                        Debug.Log($"Hitting Booster at {cell.position} - {boosterCount} booster hitted");
                        cell.ClearBooster(HitType.Direct, 1, OnBoosterCleared);
                    }
                }
            }
            if (boosterCount == 0)
            {
                _finished = true;
            }
        }

        private void OnBoosterCleared(Cell cell)
        {
            boosterCount--;
            // Debug.Log($"Booster cleared at {cell.position} - {boosterCount} remaining");
            if (boosterCount == 0)
            {
                _finished = true;
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