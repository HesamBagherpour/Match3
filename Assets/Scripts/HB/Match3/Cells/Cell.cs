﻿using System;
using System.Collections.Generic;
using Garage.Core.DI;
using Garage.Match3;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Logger;
using HB.Match3.Behaviours;
using HB.Match3.Cells.Modules;
using UnityEngine;

namespace HB.Match3.Cells
{
    public class Cell
    {
        public event Action<Cell> Cleared;
        public Point position;
        public IContext context;
        public HitType HitType { get; private set; }
        public Flow flow;
        public List<Cell> Adjacents { get; private set; }
        public ICellView View { get; private set; }
        public bool IsVisible { get; private set; }
        private Action<Cell> OnClearCallback;
        private Action<Cell> OnBoosterClearCallback;
        private int _clearedModules;
        private BlockType _previousBlockType = BlockType.None;
        public BlockModule ClearedBlock { get; private set; }
        public int damage;
        private bool markForParquet;

        #region Public Properties

        public MatchType MatchType { get; private set; }
        public bool IsBoosterHost { get; internal set; }
        public bool IsCached { get; set; }

        public static event Action<Point, Direction> SwapRequest;
        public static event Action<Point> PressRequest;

        #endregion

        #region  Constructors

        public Cell()
        {
            context = new Context(new ModuleFactory());
            Adjacents = new List<Cell>(4);
            IsBoosterHost = false;
            markForParquet = false;
        }

        public Cell(CellData data)
        {
            context = new Context(new ModuleFactory());
            Adjacents = new List<Cell>(4);
            position = data.position;
            IList<BaseModule> modules = data.context.GetAll<BaseModule>();
            for (int i = 0; i < modules.Count; i++)
            {
                BaseModule module = modules[i];
                if (module is VisibleModule) IsVisible = true;
                context.AddInstance(module);
            }
            IsBoosterHost = false;
            markForParquet = false;
        }

        #endregion

        #region Public Methods

        public void SetView(ICellView cellView)
        {
            View = cellView;
            IList<BaseModule> modules = GetModules();
            View.AddModuleView(position, modules);
        }

        public BlockType GetBlockType()
        {
            BlockModule block = GetModule<BlockModule>();
            return block?.blockType ?? BlockType.None;
        }

        public bool IsRestrictedBlock(ActionType actionType)
        {
            BlockModule block = GetModule<BlockModule>();
            if (block != null)
            {
                if ((block.restrictionType & actionType) == actionType)
                    return true;
            }

            return false;
        }

        public void SetAdjacents(List<Cell> adjacents)
        {
            if (adjacents.Count > 4)
            {
                Log.Error("Match3", "Each cell can't have more than 4 adjacents");
                return;
            }

            Adjacents = adjacents;
        }

        public void MarkForParquet()
        {
            markForParquet = true;
        }

        public bool CanHaveParquet()
        {
            return IsVisible &&
                    Contains<ParquetModule>() == false &&
                    IsLocked(ActionType.HitBlock, Direction.Center) == false &&
                    Contains<CannonModule>() == false &&
                    Contains<CandleModule>() == false &&
                    Contains<LockModule>() == false &&
                    Contains<BucketModule>() == false &&
                    Contains<FlowerModule>() == false;
        }

        public bool IsParquetableCell()
        {
            return IsVisible &&
                    Contains<ParquetModule>() == false &&
                    Contains<CannonModule>() == false &&
                    Contains<CandleModule>() == false &&
                    Contains<BucketModule>() == false&&
                    Contains<FlowerModule>() == false;
        }

        public bool CanDistributeparquet()
        {
            return IsVisible &&
                    Contains<ParquetModule>() == true &&
                    Contains<LockModule>() == false;
        }

        private void AddParquetModule()
        {
            if (IsVisible &&
                Contains<ParquetModule>() == false &&
                IsLocked(ActionType.HitBlock, Direction.Center) == false &&
                Contains<CannonModule>() == false &&
                Contains<CandleModule>() == false &&
                Contains<LockModule>() == false &&
                Contains<BucketModule>() == false&&
                Contains<FlowerModule>() == false)
            {
                ParquetModule parquetModule = AddModule<ParquetModule>();
                parquetModule.Setup(ParquetModule.LayerName);
                parquetModule.InvokeQuestEvent();
                View.AddModuleView(position, parquetModule);
                parquetModule.PlayEffect(position);
            }
        }

        #region Context

        public T AddModule<T>() where T : new()
        {
            IModule module = context.Register<T>();
            return (T)module;
        }

        internal void RemoveMatchType()
        {
            MatchType = MatchType.None;
        }

        public T RemoveModule<T>() where T : class
        {
            T module = context.Remove<T>();
            return module;
        }

        public bool HasModuleWithID(string id)
        {
            IList<BaseModule> modules = GetModules();
            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                if (module.id == id || module.id.Contains(id)) return true;
            }

            return false;
        }

        public T GetModule<T>() where T : class
        {
            return context.Get<T>();
        }

        public void AddInstance(BaseModule module)
        {
            context.AddInstance(module);
        }

        public void RemoveInstance(BaseModule module)
        {
            context.RemoveInstance(module);
        }

        public bool Contains<T>() where T : class
        {
            return context.Get<T>() != null;
        }

        public IList<BaseModule> GetModules()
        {
            return context.GetAll<BaseModule>();
        }
        public void SetClearedBlock(BlockModule blockModule)
        {
            ClearedBlock = blockModule;

        }

        #endregion

        public void Hit(HitType hitType, int damage)
        {
            //if (GetModule<BlockModule>() != null && GetModule<BlockModule>().id.Equals(BlockIDs.Gnome))
            //  Debug.LogError("gnome being hit type" + HitType);
            HitType = hitType;
            this.damage = damage;
        }

        public void HitAdjacents()
        {
            if (!IsLocked(ActionType.HitBlock, Direction.Center))
            {
                for (int i = 0; i < Adjacents.Count; i++)
                {
                    if (Adjacents[i].HitType == HitType.None)
                        Adjacents[i].Hit(HitType.Indirect, 1);
                }
            }
        }

        public void AddMatchType(MatchType type)
        {
            MatchType |= type;
        }

        public void ClearFlags()
        {
            MatchType = MatchType.None;
            HitType = HitType.None;
        }

        public void ClearBooster(HitType hitType, int damage, Action<Cell> onFinished)
        {
            if (HitType == HitType.None || damage == 0)
            {
                onFinished?.Invoke(this);
                return;

            }

            BoosterModule booster = GetModule<BoosterModule>();
            if (booster == null || booster.IsActive)
            {
                onFinished?.Invoke(this);
                return;
            }
            AddMatchType(MatchType.Booster);
            OnBoosterClearCallback = onFinished;
            booster.Clear(this, ref damage, hitType, OnBoosterClearFinish);
        }

        private void OnBoosterClearFinish(BaseModule module)
        {
            if (module != null)
            {
                RemoveInstance(module);
            }

            OnBoosterClearCallback?.Invoke(this);
            OnBoosterClearCallback = null;
        }

        public void Clear(Action<Cell> onFinished)
        {
            if (HitType == HitType.None)
            {
                onFinished?.Invoke(this);
                return;
            }

            if (markForParquet)
            {
                AddParquetModule();
                markForParquet = false;
            }

            OnClearCallback = onFinished;

            if (HitType == HitType.Direct)
            {
                _clearedModules = 2;
                ClearTopRestriction(ref damage, HitType, OnModuleCleared);
                // Debug.Log($"Clearing block at {position}");
                ClearBlock(ref damage, HitType, OnModuleCleared);
            }
            else if (HitType == HitType.Indirect)
            {
                _clearedModules = 1;
                ClearTopRestriction(ref damage, HitType, OnModuleCleared);


                SetBlockCombo();
                HitType = HitType.None;
            }

        }

        private void SetBlockCombo()
        {
            // Add combo to blocks
            if (IsLocked(ActionType.HitBlock, Direction.Center) == false)
            {
                BlockModule blockModule = GetModule<BlockModule>();
                if (blockModule == null) return;
                if (IsRestrictedBlock(ActionType.HitBlock)) return;
                if (QuestGiver.IsInQuest(blockModule) && !blockModule.id.Equals(BlockIDs.Gnome) && !blockModule.id.Equals(BlockIDs.flower))
                {
                    blockModule.AddCount(1);
                }
            }
        }

        private void OnModuleCleared(BaseModule module)
        {
            if (module != null)
            {
                // Debug.Log($"Module {module.id} was cleared cell at {position}");
                RemoveInstance(module);
            }


            if (GetTopRestriction() == null)
            {
                var block = GetModule<BlockModule>();
                if (block != null && block.id.Equals(BlockIDs.Gnome))
                {
                    damage++;
                    ClearBlock(ref damage, HitType, OnModuleCleared);
                    return;
                }
            }




            _clearedModules--;
            if (_clearedModules == 0)
            {
                OnClearCallback?.Invoke(this);
                OnClearCallback = null;
                Cleared?.Invoke(this);
            }
        }
         
        public bool IsLocked(ActionType type, Direction direction, bool onlyWall = false)
        {
            if (onlyWall == false)
            {
                var _restrictions = (List<RestrictionModule>)context.GetAll<RestrictionModule>();
                for (int i = 0; i < _restrictions.Count; i++)
                {
                    if (_restrictions[i].Restriction.Contains(type, direction)) return true;
                }
            }
            var ironWallModule = GetModule<IronWallModule>();
            if (ironWallModule == null) return false;
            if (ironWallModule.Restriction.Contains(type, direction)) return true;
            return false;
        }

        public bool IsEmptyCell()
        {
            if (this.damage == 0 &&
                this.HitType == HitType.None && 
                this.Contains<GrassModule>() == false &&
                this.Contains<WoodIronModule>() == false &&
                this.Contains<CannonModule>() == false)
                return true;
            else
                return false;

        }

        public void AddBlock(BlockModule blockModule)
        {
            if (blockModule == null) return;
            ListenToBlockModule(blockModule);
            if (Contains<BlockModule>() == false)
            {
                AddInstance(blockModule);
            }
        }

        private void ListenToBlockModule(BlockModule blockModule)
        {
            // Log.Debug("Listeners", $"Cell {position} is Listening to block {blockModule.blockType}");
            blockModule.SwapRequest += OnBlockSwapRequest;
            blockModule.PressRequest += OnBlockPressRequest;
        }

        private void RemoveBlockListeners(BlockModule blockModule)
        {
            // Log.Debug("Listeners", $"Cell {position} Stops Listening to block {blockModule.blockType}");
            blockModule.SwapRequest -= OnBlockSwapRequest;
            blockModule.PressRequest -= OnBlockPressRequest;
        }

        public void ExchangeBlocks(Cell otherCell)
        {
            BlockModule blockModule = RemoveBlock();
            BlockModule otherBlockModule = otherCell.RemoveBlock();
            AddBlock(otherBlockModule);
            otherCell.AddBlock(blockModule);
        }

        public BlockModule RemoveBlock()
        {
            BlockModule blockModule = RemoveModule<BlockModule>();
            if (blockModule != null)
            {
                RemoveBlockListeners(blockModule);
                _previousBlockType = blockModule.blockType;
            }
            return blockModule;
        }

        private void OnBlockSwapRequest(Direction direction)
        {
            SwapRequest?.Invoke(position, direction);
        }

        private void OnBlockPressRequest()
        {
            PressRequest?.Invoke(position);
        }

        #endregion


        #region Private Methods

        public void ClearBlock(ref int damage, HitType type, Action<BaseModule> onFinished)
        {
            if (damage == 0)
            {
                onFinished?.Invoke(null);
                return;
            }
            BlockModule blockModule = GetModule<BlockModule>();
            if (blockModule != null)
            {
                if (IsRestrictedBlock(ActionType.HitBlock) == false ||
                    MatchType == MatchType.ExittingPlant)
                {
                    blockModule.Clear(this, ref damage, type, onFinished);
                    RemoveBlock();
                }
                else
                {
                    onFinished?.Invoke(null);
                }
            }
            else
            {
                onFinished?.Invoke(null);
            }
        }

        public void Selected_ClearBlock(ref int damage, HitType type, Action<BaseModule> onFinished)
        {
            if (damage == 0)
            {
                onFinished?.Invoke(null);
                return;
            }
            var _restrictions = (List<RestrictionModule>)context.GetAll<RestrictionModule>();
            for (int i = 0; i < _restrictions.Count; i++)
            {
                Debug.Log(_restrictions[0].id);
            }
            BlockModule blockModule = GetModule<BlockModule>();
            if (blockModule != null)
            {
                if (IsRestrictedBlock(ActionType.HitBlock) == false ||
                    MatchType == MatchType.ExittingPlant)
                {
                    blockModule.Clear(this, ref damage, type, onFinished);
                    RemoveBlock();
                }
                else
                {
                    onFinished?.Invoke(null);
                }
            }
            else
            {
                onFinished?.Invoke(null);
            }
        }
        private void ClearTopRestriction(ref int damage, HitType type, Action<BaseModule> onFinished)
        {
            if (damage == 0)
            {

                onFinished?.Invoke(null);
                return;
            }

            RestrictionModule restriction = GetTopRestriction();
            // BlockModule blockModule = GetModule<BlockModule>();
            // onFinished?.Invoke(null);
            
            //if (restriction != null && restriction.layerName == "glass" && this._previousBlockType.id.Equals(BlockIDs.flower) == false)
            //{
            //    restriction.Clear(this, ref damage, type, onFinished);
            //}
            //else
            //{

            //    if(restriction != null)
            //    {
            //        restriction.Clear(this, ref damage, type, onFinished);
            //    }
            //    else
            //    {

            //        onFinished?.Invoke(null);
            //    }
            //}

            if (restriction != null)
            {
                restriction.Clear(this, ref damage, type, onFinished);
            }
            else
            {

                onFinished?.Invoke(null);
            }
            //if (restriction != null) restriction.Clear(this, ref damage, type, onFinished);

        }

        public RestrictionModule GetTopRestriction()
        {
            var _restrictions = (List<RestrictionModule>)context.GetAll<RestrictionModule>();
            _restrictions.Sort((m1, m2) => m2.order.CompareTo(m1.order));
            if (_restrictions.Count > 0) return _restrictions[0];
            return default;
        }

        #endregion

        public override string ToString()
        {
            return $"({position}_{MatchType})";
        }

        public void Dispose()
        {
            BlockModule block = RemoveBlock();
            block?.Dispose();
            SwapRequest = null;
            PressRequest = null;
            IList<BaseModule> modules = GetModules();
            for (var i = 0; i < modules.Count; i++)
            {
                modules[i].Dispose();
                context.RemoveInstance(modules[i]);
            }
        }

        public BlockType GetPreviousBlockType()
        {
            return _previousBlockType;
        }
    }
}