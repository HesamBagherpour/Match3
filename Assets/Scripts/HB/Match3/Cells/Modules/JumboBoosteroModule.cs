using System;
using System.Collections.Generic;
using MessagePack;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject]
    public class JumboBoosteroModule : BoosterModule
    {
        // protected override void OnSetup()
        // {
        // }

        // public override void Clear(Action<BaseModule> onFinished)
        // {
        //     onFinished?.Invoke(this);
        // }

        //public override void Execute(Board board)
        //{
        //    //List<(int x, int y)> blocks = new List<(int x, int y)>();
        //    //// 2 cells right
        //    //for (int i = 0; i < 2; i++)
        //    //{
        //    //    (int x, int y) pos = (base.pos.x + 1 + i, base.pos.y);
        //    //    if (board.HasEmptyCell(pos)) blocks.Add(pos);
        //    //}
        //    //// 2 cells left
        //    //for (int i = 0; i < 2; i++)
        //    //{
        //    //    (int x, int y) pos = ((base.pos.x - 1) - i, base.pos.y);
        //    //    if (board.HasEmptyCell(pos)) blocks.Add(pos);
        //    //}
        //    //// 2 cells top
        //    //for (int i = 0; i < 2; i++)
        //    //{
        //    //    (int x, int y) pos = (base.pos.x, base.pos.y + 1 + i);
        //    //    if (board.HasEmptyCell(pos)) blocks.Add(pos);
        //    //}
        //    //// 2 cells bottom
        //    //for (int i = 0; i < 2; i++)
        //    //{
        //    //    (int x, int y) pos = (base.pos.x, (base.pos.y - 1) - i);
        //    //    if (board.HasEmptyCell(pos)) blocks.Add(pos);
        //    //}
        //    //var topright = (pos.x + 1, pos.y + 1);
        //    //if (board.HasEmptyCell(topright)) blocks.Add(topright);
        //    //var topleft = (pos.x - 1, pos.y + 1);
        //    //if (board.HasEmptyCell(topleft)) blocks.Add(topleft);
        //    //var bottomRight = (pos.x + 1, pos.y - 1);
        //    //if (board.HasEmptyCell(bottomRight)) blocks.Add(bottomRight);
        //    //var bottomLeft = (pos.x - 1, pos.y - 1);
        //    //if (board.HasEmptyCell(bottomLeft)) blocks.Add(bottomLeft);

        //    //MatchInfo matchInfos = new MatchInfo()
        //    //{
        //    //    MatchedPoses = blocks,
        //    //    OriginPos = pos,
        //    //    Type = MatchType.Booster
        //    //};
        //    //board.MatchInfos.Add(matchInfos);
        //    //Log.Debug("BoosterModule", $"Executing JumboBoosterModule at {pos} and disposing");
        //}

        //protected override void OnExecute(Board board)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}