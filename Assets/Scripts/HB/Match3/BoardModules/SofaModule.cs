using System;
using System.Collections.Generic;
using Garage.HomeDesign.Ui_Menus;
using Garage.Match3;
using Garage.Match3.BoardEditor;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using MessagePack;
using UnityEngine;

namespace HB.Match3.BoardModules
{
    [Serializable]
    [MessagePackObject]
    public class SofaModule : BaseModule
    {
        [Key(3)] public Point position;
        [Key(4)] public Point size;

        // public static event Action<string> OnClear;
        private List<Cell> _cells;
        private SofaView _sofaView;

        public SofaModule()
        {
        }

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            id = size.x == size.y ? "sofa_square" : "sofa_rect";
            _sofaView = (SofaView)view;
        }

        public void Init(Board board)
        {
            _cells = new List<Cell>();

            for (int x = position.x; x < position.x + size.x; x++)
            {
                for (int y = position.y - (size.y - 1); y <= position.y; y++)
                {
                    if (board.IsInBounds(x, y))
                    {
                        Cell cell = board.Cells[x, y];
                        _cells.Add(cell);
                    }
                }
            }
        }

        public bool ShouldClear()
        {
            int count = _cells.Count;
            bool shouldRemove = true;
            for (int i = 0; i < count; i++)
            {
                Cell cell = _cells[i];
                if (cell.IsLocked(ActionType.Collect, Direction.Center))
                {
                    shouldRemove = false;
                    break;
                }
            }
            int damage = 1;
            if (shouldRemove) Clear(null, ref damage, HitType.Direct, null);
            return shouldRemove;
        }

        public void Collect()
        {
        }

        protected override void OnSetup()
        {
        }

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            _sofaView.Clear(onFinished);
            InvokeClearEvent(this);
        }
    }

    public class SofaView : ModuleView
    {
        private SofaModule sofaModule;
        private string aspectId;

        public SofaView(BaseModule module) : base(module)
        {
            Visible = true;
            sofaModule = (SofaModule)module;
            aspectId = sofaModule.size.x == sofaModule.size.y ? "sofa_square" : "sofa_rect";
        }

        public void Clear(Action<BaseModule> onFinished)
        {
            if (Board.IsActive && QuestGiver.IsInQuest(aspectId))
            {
                var targetPos = Match3GameUi.QuestItemPosition(aspectId);
                var tilemapLayer = (TilemapLayer)Layer;
                var pos = tilemapLayer.GetPosition(this);
                Vector3 startPos = Layer.CellToWorld(pos) + new Vector3(sofaModule.size.x / 2, -sofaModule.size.y / 2);
                Effect collectEffect = Layer.PlayEffect(startPos, targetPos, "collect");
                BoardView.PlayAudio("ReleaseSofa");

                var sofaTile = (SofaTile)Layer.GetTile(pos.x, pos.y);
                collectEffect.SetSprite(sofaTile.sprite);
            }
            Layer.Clear(this);
        }
    }
}