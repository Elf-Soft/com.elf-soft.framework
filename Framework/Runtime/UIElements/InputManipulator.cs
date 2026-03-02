using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ElfSoft.Framework
{
    public class InputManipulator : Manipulator
    {
        private readonly InputAction action;
        private readonly float loopTime;
        private readonly Action<InputAction> onPerformed;
        private readonly Action<InputAction> onHoldPerformed;
        private Awaitable awaitable;


        public InputManipulator(InputAction action, Action<InputAction> onPerformed, Action<InputAction> onHoldPerformed = null, float loopTime = 0.2f)
        {
            this.action = action;
            this.loopTime = loopTime;
            this.onPerformed = onPerformed;
            this.onHoldPerformed = onHoldPerformed;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            target.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            target.UnregisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent _) => action.performed += OnPerformed;
        private void OnDetachFromPanel(DetachFromPanelEvent _)
        {
            action.performed -= OnPerformed;
            awaitable?.Cancel();
        }


        private void OnPerformed(InputAction.CallbackContext context)
        {
            onPerformed.Invoke(context.action);
            if (onHoldPerformed == null) return;
            awaitable?.Cancel();
            awaitable = HoldPerformedAsync();
        }

        private async Awaitable HoldPerformedAsync()
        {
            var startTime = Time.realtimeSinceStartup;
            var triggerTime = Time.realtimeSinceStartup;
            while (action.IsPressed())
            {
                if (Time.realtimeSinceStartup - startTime >= 0.4f && Time.realtimeSinceStartup - triggerTime >= loopTime)
                {
                    triggerTime = Time.realtimeSinceStartup;
                    onHoldPerformed.Invoke(action);
                }
                await Awaitable.NextFrameAsync();
            }
        }

    }
}
