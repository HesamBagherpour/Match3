using System;
using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using UnityEngine;
using Random = System.Random;
using UnityEngine.Assertions;
using System.Linq;
using System.Security.AccessControl;
using HB.Match3;
using HB.Match3.Behaviours;
using HB.Match3.Cells.Modules;
using HB.Match3.View;

namespace Garage.Match3.BoardStates
{
    public class FlowerState : BoardState
    {
        private bool _finished;
        private int hasAnyFlowersHit;
        private int effectMoveCount;
        private readonly BlockFactory _blockFactory;
        private readonly Random _random;
        private readonly BoardView _boardView;
        private readonly List<Cell> coloredCells;
        private List<Cell> flowerCells = new List<Cell>();
        private List<Cell> hit_FlowerCells = new List<Cell>();
        private List<Cell> result = new List<Cell>();
        private List<Cell> freeBlockCells = new List<Cell>();
        private List<Cell> _simpleCacheBlockCells = new List<Cell>();
        private readonly Dictionary<BlockType, List<Cell>> finalResultToPaint;
        private bool _isContain_GlassModule = false;
        private Point tempPosition;
        public FlowerState(BlockFactory blockFactory, Random random, IBoardView boardView)
        {
            finalResultToPaint = new Dictionary<BlockType, List<Cell>>();
            tempPosition = new Point(500, 500);
            _blockFactory = blockFactory;
            coloredCells = new List<Cell>();
            _random = random;
            _isContain_GlassModule = false;
            _boardView = (BoardView)boardView;
        }
        protected override void OnEnter()
        {
            base.OnEnter();
            ResetValues();
            FindHittedFlowers();
            Flowerdistribute();
        }
        private void FindHittedFlowers()
        {
            _simpleCacheBlockCells = new List<Cell>();
            foreach (var cell in Cells)
            {
                if (cell.IsVisible)
                {
                    cell.IsCached = false;
                    _simpleCacheBlockCells.Add(cell);
                }
                if (cell.IsVisible && cell.IsLocked(ActionType.HitBlock, Direction.Center) == false)
                {

                    BlockModule flowerModule = cell.GetModule<BlockModule>();
                    if (flowerModule != null && flowerModule.blockType.id.Equals(BlockIDs.flower))
                    {

                        if (cell.HitType != HitType.None)
                        {
                            hasAnyFlowersHit++;
                            hit_FlowerCells.Add(cell);
                        }
                        //else
                        //{
                        //    for (int i = 0; i < cell.Adjacents.Count; i++)
                        //    {
                        //        Cell adjacent = cell.Adjacents[i];

                        //        if (adjacent.HitType == HitType.Direct)
                        //        {
                        //            hasAnyFlowersHit++;

                        //            hit_FlowerCells.Add(cell);


                        //        }
                        //    }
                        //}
                    }
                }
            }
            if (hasAnyFlowersHit == 0) _finished = true;
        }
        private void OnClearFinished(Cell cell)
        {
            hasAnyFlowersHit--;
            effectMoveCount--;
            if (effectMoveCount == 0 /*|| hasAnyFlowersHit == 0*/)
            {
                _finished = true;
            }
        }
        private void Flowerdistribute()
        {
            var dmg = 1;
            foreach (Cell cell in hit_FlowerCells)
            {
                BlockModule flower = cell.GetModule<BlockModule>();
               
                if (cell.Contains<GlassModule>())
                {
                    
                    _isContain_GlassModule = true;
                }
                if (flower.blockType.color == BlockColor.Green)// 3 flower.id == FlowerIDs.Flower3
                {

                    BlockType targetBlockType = _blockFactory.GetBlockTypeByIdAndColor(BlockIDs.flower, BlockColor.Red);
                    List<Cell> targetCells = GetTwoFlowerTarget();
                    effectMoveCount += targetCells.Count;
                    for (int i = 0; i < targetCells.Count; i++)
                    {
                        _boardView.PlayFlowerBlockEffect(cell.position,
                                                         targetCells[i].position,
                                                         targetBlockType,
                                                         FlowerMoveEffectComplete());
                    }

                    if (finalResultToPaint.ContainsKey(targetBlockType) == false)
                        finalResultToPaint.Add(targetBlockType, targetCells);
                    else finalResultToPaint[targetBlockType].AddRange(targetCells);
                    cell.ClearBlock(ref dmg, HitType.Direct, module => OnClearFinished(cell));

                }
                else if (flower.blockType.color == BlockColor.Red) // 2  flower.id == FlowerIDs.Flower2
                {
                    BlockType targetBlockType = _blockFactory.GetBlockTypeByIdAndColor(BlockIDs.flower, BlockColor.None);
                    List<Cell> targetCells = GetTwoFlowerTarget();
                    effectMoveCount += targetCells.Count;
                    for (int i = 0; i < targetCells.Count; i++)
                    {
                        _boardView.PlayFlowerBlockEffect(cell.position,
                                                         targetCells[i].position,
                                                         targetBlockType,
                                                         FlowerMoveEffectComplete());
                    }
                    if (finalResultToPaint.ContainsKey(targetBlockType) == false)
                        finalResultToPaint.Add(targetBlockType, targetCells);
                    else finalResultToPaint[targetBlockType].AddRange(targetCells);

                    cell.ClearBlock(ref dmg, HitType.Direct, module => OnClearFinished(cell));

                }
                else if (flower.blockType.color == BlockColor.None) // 1 flower.id == FlowerIDs.Flower1
                {
                    effectMoveCount++;
                   
              
                    cell.Selected_ClearBlock(ref dmg, HitType.Direct, module => OnClearFinished(cell));

                }
            }
            if (effectMoveCount == 0) _finished = true;
        }
        private Action FlowerMoveEffectComplete()
        {
            return () =>
            {
                effectMoveCount--;

                if (effectMoveCount == 0 || effectMoveCount < 0)
                {
                    ChangeTargetBlockColor();
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
                foreach (var cell in targetCells)
                {
                    if (cell.GetModule<BlockModule>() == null)
                    {
                        _blockFactory.CreateBlock(cell, _blockFactory.GetBlockTypeByIdAndColor(BlockIDs.flower, targetBlockType.color));
                    }
                    else
                    {
                        var block = cell.GetModule<BlockModule>();
                        block.ChangeType(targetBlockType);
                    }

                }
            }
            _finished = true;
        }
        private List<Cell> GetTwoFlowerTarget()
        {
            var _freeBlockCells = new List<Cell>();
            // Find Free Block Cell 
            for (int i = 0; i < _simpleCacheBlockCells.Count; i++)
            {
                BlockModule block = _simpleCacheBlockCells[i].GetModule<BlockModule>();
                BlockType bt = _simpleCacheBlockCells[i].GetPreviousBlockType();
                if (block == null && _simpleCacheBlockCells[i].IsEmptyCell()
                    && _simpleCacheBlockCells[i].IsCached == false
                    && _simpleCacheBlockCells[i].IsLocked(ActionType.Move, Direction.Center) == false && _simpleCacheBlockCells[i].Contains<CannonModule>() == false
                    && _simpleCacheBlockCells[i].Contains<ParquetModule>() == false && _simpleCacheBlockCells[i].position.Equals(tempPosition) == false)
                {
                    _simpleCacheBlockCells[i].IsCached = true;
                    tempPosition = _simpleCacheBlockCells[i].position;
                    _freeBlockCells.Add(_simpleCacheBlockCells[i]);
                    if (_freeBlockCells.Count == 2) break;

                }
            }
            if(_freeBlockCells.Count <= 1)
            {
                for (int i = 0; i < _simpleCacheBlockCells.Count; i++)
                {
                    BlockModule block = _simpleCacheBlockCells[i].GetModule<BlockModule>();
                    if (block == null
                        && _simpleCacheBlockCells[i].IsCached == false
                        && _simpleCacheBlockCells[i].IsEmptyCell()
                        && _simpleCacheBlockCells[i].IsLocked(ActionType.Move, Direction.Center) == false && _simpleCacheBlockCells[i].Contains<CannonModule>() == false
                        && _simpleCacheBlockCells[i].Contains<ParquetModule>() == false && _simpleCacheBlockCells[i].position.Equals(tempPosition) == false )
                    {
                        _simpleCacheBlockCells[i].IsCached = true;
                        tempPosition = _simpleCacheBlockCells[i].position;
                        _freeBlockCells.Add(_simpleCacheBlockCells[i]);
                        if (_freeBlockCells.Count == 2) break;

                    }
                }
            }
            if (_freeBlockCells.Count <= 1)
            {
                for (int i = 0; i < _simpleCacheBlockCells.Count; i++)
                {
                    BlockModule block = _simpleCacheBlockCells[i].GetModule<BlockModule>();
                    if (block != null && block.id.Contains(BlockIDs.Simple)
                        && _simpleCacheBlockCells[i].IsCached == false
                        && _simpleCacheBlockCells[i].IsEmptyCell()
                        && block.IsJumbo == false
                        && block.id.Contains(BlockIDs.flower) == false &&
                        _simpleCacheBlockCells[i].Contains<GrassModule>() == false &&
                        _simpleCacheBlockCells[i].Contains<WoodIronModule>() == false &&
                        //_simpleCacheBlockCells[i].IsLocked(ActionType.Move, Direction.Center) == false &&
                        _simpleCacheBlockCells[i].Contains<CannonModule>() == false && _simpleCacheBlockCells[i].position.Equals(tempPosition) == false)
                    {
                        _simpleCacheBlockCells[i].IsCached = true;
                        tempPosition = _simpleCacheBlockCells[i].position;
                        _freeBlockCells.Add(_simpleCacheBlockCells[i]);
                        if (_freeBlockCells.Count == 2) break;

                    }

                }

            }
            if (_freeBlockCells.Count <= 1)
            {
                for (int i = 0; i < _simpleCacheBlockCells.Count; i++)
                {
                    BlockModule block = _simpleCacheBlockCells[i].GetModule<BlockModule>();
                    if (block != null && block.id.Contains(BlockIDs.Simple)
                        && _simpleCacheBlockCells[i].IsCached == false
                        && block.IsJumbo == false
                        && block.id.Contains(BlockIDs.flower) == false &&
                        _simpleCacheBlockCells[i].Contains<GrassModule>() == false &&
                        _simpleCacheBlockCells[i].Contains<WoodIronModule>() == false &&
                        _simpleCacheBlockCells[i].Contains<CannonModule>() == false && _simpleCacheBlockCells[i].position.Equals(tempPosition) == false)
                    {

                        _simpleCacheBlockCells[i].IsCached = true;
                        tempPosition = _simpleCacheBlockCells[i].position;
                        _freeBlockCells.Add(_simpleCacheBlockCells[i]);
                        if (_freeBlockCells.Count == 2) break;
                    }
                }
            }
            return new List<Cell>() { _freeBlockCells[0], _freeBlockCells[1] };
        }
        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished == true)
            {
                Finished();
            }
        }
        private void ResetValues()
        {
            _finished = false;
            flowerCells.Clear();
            hit_FlowerCells.Clear();
            finalResultToPaint.Clear();
            _simpleCacheBlockCells.Clear();
            coloredCells.Clear();
            result.Clear();
            freeBlockCells.Clear();
            hasAnyFlowersHit = 0;
            effectMoveCount = 0;
        }
    }
}



