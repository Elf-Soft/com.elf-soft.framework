using System;
using UnityEngine;


namespace ElfSoft.ActorSystem.StateSystem
{
    [Serializable]
    public abstract class ActorState : State
    {
        public ActorManager Mgr => Machine.Target as ActorManager;
        public Animator Anim => Mgr.Anim;
        public MoveController MC => Mgr.MC;
        public InputController IC => Mgr.IC;
        public float AnimTime => Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        public int CurAnimName => Anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        public bool IsCurAnim => CurAnimName == NameHash;


        /// <summary>
        /// 切换到其它状态, stateHash:状态机哈希名, animHash:Unity动画状态机动画状态名, predicate:切换条件
        /// </summary>
        /// <returns></returns>
        protected bool ToState(bool predicate, int stateHash, int animHash,
            float duration = 0.1f, float offset = 0f, int layer = 0, Action beforeSwitch = null)
        {
            if (predicate)
            {
                beforeSwitch?.Invoke();
                Anim.CrossFade(animHash, duration, layer, offset);
                To(stateHash);
                return true;
            }
            return false;
        }

        public bool ToMove(float duration = 0.1f, float offset = 0f, int layer = 0)
        {
            return ToState(IC.IsMoving, StringHash.Move, StringHash.Run, duration, offset, layer);
        }

        public bool ToIdle(int animHash, float duration = 0.1f, float offset = 0f, int layer = 0)
        {
            return ToState(!IC.IsMoving, StringHash.Idle, animHash, duration, offset, layer);
        }

        //public bool ToAtk(float duration = 0.1f, float offset = 0f, int layer = 0)
        //{
        //    return ToState(Input[StringHash.Attack].WasPerformedThisFrame(),
        //        StringHash.Attack, StringHash.Attack, duration, offset, layer);
        //}

        //protected void Interact()
        //{
        //    if (Input[StringHash.Interact].WasPressedThisFrame())
        //    {
        //        //Mgr.InteractCtrl.SetartInteract();
        //    }
        //}
    }
}
