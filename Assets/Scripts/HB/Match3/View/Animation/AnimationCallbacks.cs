using System;
using UnityEngine;

namespace HB.Match3.View.Animation
{
    public class AnimationCallbacks : StateMachineBehaviour
    {
        public string stateName;
        public event Action<Animator, AnimatorStateInfo, string> EnterState;
        public event Action<Animator, AnimatorStateInfo, string> ExitState;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            EnterState?.Invoke(animator, stateInfo, stateName);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            ExitState?.Invoke(animator, stateInfo, stateName);
        }
    }
}