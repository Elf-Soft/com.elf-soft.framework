using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.Framework
{
    public class CursorElement : VisualElement
    {
        private readonly VisualElement animatable, icon;
        private Tween tween;
        private static readonly string ussClassName = "cursor-element";
        private static readonly string iconUssClassName = ussClassName + "__icon";

        public CursorElement()
        {
            pickingMode = PickingMode.Ignore;
            AddToClassList(ussClassName);

            animatable = new() { pickingMode = PickingMode.Ignore };
            animatable.style.flexGrow = 1;
            Add(animatable);

            icon = new() { pickingMode = PickingMode.Ignore };
            icon.AddToClassList(iconUssClassName);
            animatable.Add(icon);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(e =>
            {
                var elem = e.target as CursorElement;
                if (elem.tween.isAlive) elem.tween.Complete();
            });
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            if (tween.isAlive) tween.Complete();
            tween = Tween.Custom(this, -5f, 5f, 0.5f,
                (target, newValue) => target.animatable.style.translate = new Translate(newValue, 0, 0)
                , Ease.InOutSine, -1, CycleMode.Yoyo).OnComplete(this, e => e.animatable.style.translate = StyleKeyword.Null);

        }
    }
}
