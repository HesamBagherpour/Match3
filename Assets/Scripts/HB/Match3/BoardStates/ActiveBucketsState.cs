using System;
using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Behaviours;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using UnityEngine;
using UnityEngine.Assertions;

namespace Garage.Match3.BoardStates
{
    public class ActiveBucketsState : BoardState
    {
        private bool _finished;
        private readonly List<Cell> allBuckets;
        private readonly BlockFactory _blockFactory;
        private readonly System.Random _rand;
        private readonly IBoardView _boardView;
        private readonly List<Cell> coloredCells;
        private readonly Dictionary<BlockType, List<Cell>> finalResultToPaint;
        private bool cached;
        private int effectMoveCount;

        public ActiveBucketsState(IBoardView boardView, BlockFactory blockFactory, System.Random rand)
        {
            finalResultToPaint = new Dictionary<BlockType, List<Cell>>();
            coloredCells = new List<Cell>();
            allBuckets = new List<Cell>();
            _blockFactory = blockFactory;
            _rand = rand;
            _boardView = boardView;
        }

        private void CacheAllBuckets()
        {
            Debug.Log("ActiveBucketState - CacheAllBuckets");
            foreach (var cell in Cells)
            {
                if (cell.IsVisible && cell.Contains<BucketModule>())
                {
                    allBuckets.Add(cell);
                }
            }
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            ResetValues();

            if (cached == false)
            {
                CacheAllBuckets();
                cached = true;
            }

            if (allBuckets.Count == 0)
            {
                _finished = true;
                return;
            }

            Debug.Log("ActiveBucketState - OnEnter");
            foreach (var bucketCell in allBuckets)
            {
                if (bucketCell.Contains<GrassModule>() == false)
                {
                    BucketModule bucket = bucketCell.GetModule<BucketModule>();
                    if (bucket.IsActive)
                    {
                        // Execute Bucket behaviour
                        BlockType targetBlockType = _blockFactory.GetBlockTypeByIdAndColor(BlockIDs.Simple, bucket.color);
                        List<Cell> targetCells = GetRandomFreeBlocks(4, targetBlockType.color);
                        effectMoveCount += targetCells.Count;
                        for (int i = 0; i < targetCells.Count; i++)
                        {
                            _boardView.PlayBucketBlockEffect(bucketCell.position,
                                                             targetCells[i].position,
                                                             targetBlockType,
                                                             BucketMoveEffectComplete());
                        }
                        if (finalResultToPaint.ContainsKey(targetBlockType) == false)
                            finalResultToPaint.Add(targetBlockType, targetCells);
                        else finalResultToPaint[targetBlockType].AddRange(targetCells);
                        bucket.BurstComplete();
                    }
                }
            }
            if (effectMoveCount == 0) _finished = true;
        }

        private Action BucketMoveEffectComplete()
        {
            return () =>
            {
                effectMoveCount--;
                if (effectMoveCount == 0)
                {
                    ChangeTargetBlockColor();
                    Debug.Log("Playing bucket-splash-onboard");
                    BoardView.PlayAudio("bucket-splash-onboard");
                }
            };
        }

        private void ChangeTargetBlockColor()
        {
            foreach (var resultPair in finalResultToPaint)
            {
                var targetBlockType = resultPair.Key;
                var targetCells = resultPair.Value;
                Debug.Log($"ActiveBucketState - ChangeTargetBlockColor to {targetBlockType}");
                Assert.IsFalse(targetBlockType == BlockType.None, $"Could not find targetBlocktype for {targetBlockType.color}");
                foreach (var cell in targetCells)
                {
                    var block = cell.GetModule<BlockModule>();
                    block.ChangeType(targetBlockType);
                    Debug.Log($"ActiveBucketState - Change Block type at {cell.position} to {targetBlockType}");
                }
            }
            _finished = true;
        }

        private List<Cell> GetRandomFreeBlocks(int count, BlockColor color)
        {
            var result = new List<Cell>();
            List<Cell> _simpleBlockCells = GetAllBlocksExceptColor(color);
            if (_simpleBlockCells != null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (_simpleBlockCells.Count > 0)
                    {
                        int randomIndex = _rand.Next(0, _simpleBlockCells.Count - 1);
                        Cell cell = _simpleBlockCells[randomIndex];
                        result.Add(cell);
                        Debug.Log($"ActiveBucketState - GetRandomFreeBlock at {cell.position}");
                        coloredCells.Add(cell);
                        _simpleBlockCells.RemoveAt(randomIndex);
                    }
                }
            }
            return result;
        }

        private List<Cell> GetAllBlocksExceptColor(BlockColor color)
        {
            var _simpleBlockCells = new List<Cell>();
            foreach (var cell in Cells)
            {
                if (cell.IsVisible && cell.IsLocked(ActionType.Move, Direction.Center) == false)
                {
                    BlockModule block = cell.GetModule<BlockModule>();
                    if (block != null && block.id.Contains(BlockIDs.Simple) && block.IsJumbo == false && block.blockType.color != color  && !block.id.Contains(BlockIDs.flower))
                    {
                        if (coloredCells.Contains(cell) == false)
                        {
                            _simpleBlockCells.Add(cell);
                        }
                    }
                }
            }

            return _simpleBlockCells;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished)
            {
                Finished();
            }
        }

        private void ResetValues()
        {
            effectMoveCount = 0;
            coloredCells.Clear();
            finalResultToPaint.Clear();
            _finished = false;
        }
    }
}