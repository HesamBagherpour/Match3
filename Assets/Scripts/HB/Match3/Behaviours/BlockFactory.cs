using Garage.Match3;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using Random = System.Random;

namespace HB.Match3.Behaviours
{
    public class BlockFactory
    {
        private readonly BlockType[] _levelBlockTypes;
        private readonly BlockType[] _allBlockTypes;
        private readonly BoardViewData boardViewData;
        private readonly Random _rand;

        public BlockFactory(BlockType[] blockTypes, BoardViewData boardViewData, Random rand)
        {
            _levelBlockTypes = blockTypes;
            _allBlockTypes = boardViewData.blockViewData.GetBlockTypes();
            this.boardViewData = boardViewData;
            _rand = rand;
        }

        public void CreateBlock(Cell cell, BlockType bt, bool falling = false)
        {
            if (cell.Contains<BlockModule>()) return;
            BlockModule blockModule = cell.AddModule<BlockModule>();
            blockModule.Setup(bt);
            blockModule.restrictionType = boardViewData.blockViewData.blockViewDatas.Find(x => x.blockType == bt).restrictionType;
            cell.AddBlock(blockModule);
            cell.View.AddModuleView(cell.position, blockModule);
            if (PuzzleController.IsCouponEvent && falling) SetCoupon(blockModule);
        }

        private void SetCoupon(BlockModule blockModule)
        {
            if (blockModule.id.Contains(BlockIDs.Simple))
            {
                var shouldHaveCoupon = _rand.Next(100) <= CreateBehaviour.Setting.FallCouponChancePercent;
                if (shouldHaveCoupon) blockModule.SetCoupon(true);
            }
        }

        public void CreateFallingBlock(Cell cell)
        {
            CreateBlock(cell, GetRandomBlockType(true), true);
        }

        public BlockType GetRandomBlockType(bool checkQuest = false)
        {
            BlockType bt = BlockType.None;
            // if there are any block types in quest that we dont have in board, pick that blockType instead
            if (checkQuest)
            {
                BlockType _requiredBlockType = QuestGiver.RequiredBlockType();
                if (_requiredBlockType != BlockType.None)
                {
                    bt = _requiredBlockType;
                }
            }
            if (bt == BlockType.None)
            {
                // Pick a random block
                int index = _rand.Next(0, _levelBlockTypes.Length);
                bt = _levelBlockTypes[index];
            }
            return bt;
        }

        internal BlockType GetBlockTypeByIdAndColor(string id, BlockColor color)
        {
            for (int i = 0; i < _allBlockTypes.Length; i++)
            {
                BlockType blockType = _allBlockTypes[i];
                if (blockType.id.Contains(id) && blockType.color == color) return blockType;
            }
            return BlockType.None;
        }
        internal BlockType GetBlockTypeById(string id)
        {
            for (int i = 0; i < _allBlockTypes.Length; i++)
            {
                BlockType blockType = _allBlockTypes[i];
                if (blockType.id.Contains(id)) return blockType;
            }
            return BlockType.None;
        }
        internal BlockType GetFlowerBlockWithHealth(string id)
        {
            for (int i = 0; i < _allBlockTypes.Length; i++)
            {
                BlockType blockType = _allBlockTypes[i];
                if (blockType.id.Contains(id)) return blockType;
            }
            return BlockType.None;
        }


        internal BlockType[] GetLevelBlockTypes()
        {
            return _levelBlockTypes;
        }
    }
}