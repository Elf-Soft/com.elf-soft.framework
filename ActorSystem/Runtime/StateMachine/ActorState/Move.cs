using System;

//using ElfSoft.StatsSystem;
using UnityEngine;

namespace ElfSoft.ActorSystem.StateSystem
{
    [Serializable]
    public sealed class Move : ActorState
    {
        public override int NameHash => StringHash.Move;
        [SerializeField] private float runEndToIdleTime = 0.3f;


        public override void Update()
        {
            //Interact();
            //if (ToAtk()) return;
            //else if (RunToIdle()) return;
            if (ToRunStop()) return;

            SetMove();
        }

        private void SetMove()
        {
            if (IC.MoveValue.x != 0) Anim.SetFloat(StringHash.Horizontal, IC.MoveValue.x < 0f ? 0f : 1f);
            if (CurAnimName == StringHash.RunLoop || (CurAnimName == StringHash.RunStart && Anim.GetNextAnimatorClipInfoCount(0) > 0))
            {
                MC.Move(IC.ViewMoveValue, 3.75f);
            }
        }

        private bool ToRunStop()
        {
            if (!IC.IsMoving)
            {
                if (CurAnimName == StringHash.RunLoop || (CurAnimName == StringHash.RunStart && AnimTime >= runEndToIdleTime))
                {
                    Anim.CrossFade(StringHash.RunStop, 0.2f);
                }
                else Anim.CrossFade(StringHash.RunStop, 0.2f, 0, 0.2f);

                To(StringHash.Idle);
                return true;
            }
            return false;
        }
    }
}
