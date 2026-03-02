using ElfSoft.InputSystemExtension;
using UnityEngine;

namespace ElfSoft.ActorSystem
{
    public sealed class InputController
    {
        private readonly PlayerInputController pic;
        private readonly Camera camera;
        public Vector2 MoveValue => pic[StringHash.Move].ReadValue<Vector2>();
        public Vector3 ViewMoveValue
        {
            get
            {
                var dir = camera.transform.forward * MoveValue.y;
                dir += camera.transform.right * MoveValue.x;
                dir.y = 0;
                return dir.normalized;
            }
        }
        public bool IsMoving => MoveValue != Vector2.zero;


        public InputController(PlayerInputController pic)
        {
            this.pic = pic;
            camera = pic.PlayerInput.camera;
        }

    }
}
