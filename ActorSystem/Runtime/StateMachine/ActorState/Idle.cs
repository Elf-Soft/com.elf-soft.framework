using System;

namespace ElfSoft.ActorSystem.StateSystem
{
    [Serializable]
    public sealed class Idle : ActorState
    {
        public override int NameHash => StringHash.Idle;


        public override void Update()
        {
            //Interact();
            //if (ToAtk()) return;
            //else if (ToMoveStart()) return;
            //else
            //{
            //    if (Input.Interact) Mgr.Interact();
            //}

            if (ToRunStart()) return;
        }

        private bool ToRunStart()
        {
            return ToState(IC.IsMoving, StringHash.Move, StringHash.RunStart, 0.1f);
        }
    }
}
