using System;
using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Behaviours;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using HB.Utilities;

namespace Garage.Match3.BoardStates
{
    public class MeterState : BoardState
    {
        private bool hasAnyMeters;
        private bool initialized;
        private readonly BlockFactory _blockFactory;
        private readonly Random _random;
        private readonly BoardView _boardView;
        private bool _finished;
        private int updatingMeterCounter;
        private List<Cell> meterCells = new List<Cell>();
        private List<Cell> meterSpawnerCells = new List<Cell>();

        public MeterState(BlockFactory blockFactory, Random random, IBoardView boardView)
        {
            initialized = false;
            _blockFactory = blockFactory;
            _random = random;
            _boardView = (BoardView)boardView;
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            if (initialized == false)
            {
                hasAnyMeters = QuestGiver.IsInQuest(BlockIDs.Meter);
                foreach (var cell in Cells)
                {
                    if (cell.Contains<MeterSpawnerModule>())
                    {
                        meterSpawnerCells.Add(cell);
                    }
                }
                initialized = true;
            }

            _finished = false;
            if (hasAnyMeters == false)
            {
                _finished = true;
            }
            else
            {
                UpdateMeterPositions();
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished == true)
            {
                Finished();
            }
        }


        private void UpdateMeterPositions()
        {
            updatingMeterCounter = 0;
            meterCells.Clear();
            foreach (var cell in Cells)
            {
                if (cell.IsVisible)
                {
                    BlockModule block = cell.GetModule<BlockModule>();
                    if (block != null && block.blockType.id == BlockIDs.Meter)
                    {
                        meterCells.Add(cell);
                    }
                }
            }
            foreach (var cell in meterCells)
            {
                if (cell.IsLocked(ActionType.Move, Direction.Top) == false)
                {
                    Point topPosition = new Point(cell.position.x, cell.position.y + 1);
                    if (Agent.IsInBounds(topPosition))
                    {
                        Cell topCell = Cells[topPosition.x, topPosition.y];
                        BlockModule block = cell.GetModule<BlockModule>();
                        BlockModule otherBlock = topCell.GetModule<BlockModule>();
                        if (block != null
                            && otherBlock != null
                            && otherBlock.blockType.id != BlockIDs.Meter
                            && topCell.IsLocked(ActionType.Swap, Direction.Bottom) == false)
                        {
                            updatingMeterCounter++;
                            block.PlayMeterEffect(() =>
                            {
                                cell.ExchangeBlocks(topCell);
                                block.MoveTo(topCell.position, null);
                                otherBlock.MoveTo(cell.position, MoveComplete);
                            });
                            // _boardView.PlayEffect(cell.position, "meter-move", null);
                        }
                    }
                }
            }
            if (updatingMeterCounter == 0)
            {
                SpawnMeter();
            }
            else
            {
                BoardView.PlayAudio("meter-sfx");
            }
        }

        private void MoveComplete()
        {
            updatingMeterCounter--;
            if (updatingMeterCounter == 0)
            {
                SpawnMeter();
            }
        }

        private void SpawnMeter()
        {
            // Check if chance to spawn happens
            bool shouldSpawnByPossibility = _random.NextDouble() > 0.5;
            bool canSpawnMoreMeters = meterCells.Count < QuestGiver.GetMaxCount(BlockIDs.Meter);
            if (shouldSpawnByPossibility && canSpawnMoreMeters)
            {
                Extenstions.Shuffle(meterSpawnerCells);
                foreach (var cell in meterSpawnerCells)
                {
                    BlockModule block = cell.GetModule<BlockModule>();
                    if (cell.IsLocked(ActionType.Spawn, Direction.Center) == false && block != null && block.blockType.id != BlockIDs.Meter)
                    {
                        // Spawn meter
                        BlockType meterBlockType = new BlockType()
                        {
                            color = block.blockType.color,
                            id = BlockIDs.Meter
                        };
                        int dmg = 10;
                        block.Clear(cell, ref dmg, HitType.Direct, null);
                        cell.RemoveBlock();
                        _blockFactory.CreateBlock(cell, meterBlockType);
                        break;
                    }
                }
                _finished = true;
            }
            else
            {
                _finished = true;
            }
        }
    }
}