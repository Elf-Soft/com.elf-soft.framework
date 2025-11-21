/*using ElfSoft.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElfSoft.InputSystemExtension
{
    public sealed class InputController : PlayerInputController
    {
        private string savePath;
        public Vector2 MoveValue => this[StringHash.Move].ReadValue<Vector2>();
        public Vector3 ViewMoveValue
        {
            get
            {
                if (PlayerInput.camera == null) return MoveValue;
                var dir = PlayerInput.camera.transform.forward * MoveValue.y;
                dir += PlayerInput.camera.transform.right * MoveValue.x;
                dir.y = 0;
                return dir.normalized;
            }
        }
        public bool IsMoving => MoveValue.x != 0;
        public static InputController Instance { get; private set; }


        protected override void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(this);
                return;
            }
            base.Awake();

            PlayerInput.actions.FindActionMap("UI").Enable();
            savePath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "InputActionsBinding.json"));
        }

        public override void SaveData()
        {
            var data = PlayerInput.actions.SaveBindingOverridesAsJson();
            File.WriteAllText(savePath, data);
        }

        public override void LoadData()
        {
            if (File.Exists(savePath))
            {
                var data = File.ReadAllText(savePath);
                PlayerInput.actions.LoadBindingOverridesFromJson(data);
            }
        }
    }

}
*/