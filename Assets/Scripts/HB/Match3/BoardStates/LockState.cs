using System;
using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Cells.Modules;

namespace Garage.Match3.BoardStates
{
    public class LockState : BoardState
    {
        private bool _finished;
        private int clearedModule;

        protected override void OnEnter()
        {
            base.OnEnter();
            clearedModule = 0;
            for (int i = 0; i < QuestGiver.LockQuests.Count; i++)
            {
                if (i < QuestGiver.LockQuests.Count)
                {
                    Cell cell = QuestGiver.LockQuests[i];
                    var lockModuleQuest = cell.GetModule<LockQuestModule>();
                    if (lockModuleQuest.Count == 0)
                    {
                        clearedModule++;
                        RemoveLockModuleFromCell(cell, lockModuleQuest);
                    }
                }
            }
            if (clearedModule == 0) _finished = true;
        }

        private void RemoveLockModuleFromCell(Cell cell, LockQuestModule lockModuleQuest)
        {
            int damage = 10;
            lockModuleQuest.Clear(cell, ref damage, HitType.Direct, (module) =>
            {
                clearedModule--;
                // Remove all lockQuest cells.
                foreach (Cell lockCell in lockModuleQuest.Cells)
                {
                    var lockModule = lockCell.GetModule<LockModule>();
                    lockCell.RemoveModule<LockModule>();
                    var lockDamage = 10;
                    lockModule.Clear(lockCell, ref lockDamage, HitType.Direct, null);
                }
                cell.RemoveModule<LockQuestModule>();
                QuestGiver.RemoveLockQuest(cell);
                if (clearedModule == 0) _finished = true;
            });
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