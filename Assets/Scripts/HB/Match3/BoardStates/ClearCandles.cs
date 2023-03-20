using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.View;
using UnityEngine;

namespace Garage.Match3.BoardStates
{
    public class CandleState : BoardState
    {
        private bool _finished;
        private bool _initialized;
        private Dictionary<Cell, CandleModule> allCandles;
        private int totalCandles;

        public CandleState()
        {
            allCandles = new Dictionary<Cell, CandleModule>();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            InitializeAllCandles();
            CheckActiveCandles();
        }

        private void InitializeAllCandles()
        {
            if (_initialized == false)
            {
                // Cache all candles:
                foreach (var cell in Cells)
                {
                    CandleModule candleModule = cell.GetModule<CandleModule>();
                    if (candleModule != null)
                    {
                        allCandles.Add(cell, candleModule);
                    }
                }
                _initialized = true;
            }
        }

        private void CheckActiveCandles()
        {
            if (allCandles.Count == 0)
            {
                _finished = true;
                return;
            }
            bool allActive = true;
            foreach (var candlePair in allCandles)
            {
                if (candlePair.Value.active == false)
                {
                    allActive = false;
                    break;
                }
            }
            if (allActive) ClearAllCandles();
            else _finished = true;
        }

        private void ClearAllCandles()
        {
            Debug.Log("All candles are active, clearing them!");
            totalCandles = allCandles.Count;
            foreach (var candlePair in allCandles)
                candlePair.Value.SetReadyToClear();
            foreach (var candlePair in allCandles)
            {
                Cell cell = candlePair.Key;
                cell.Hit(HitType.Direct, 1);
                cell.Clear(OnCandleCleared);
            }
            BoardView.PlayAudio("candle-clear");
            allCandles.Clear();
        }

        private void OnCandleCleared(Cell obj)
        {
            totalCandles--;
            if (totalCandles == 0)
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