using System;
using UnityEngine.UIElements;

namespace ElfSoft.Framework
{
    public class ClickElement : VisualElement
    {
        private Clickable clickable;
        public Clickable Clickable
        {
            get
            {
                return clickable;
            }
            set
            {
                if (clickable != null && clickable.target == this)
                {
                    this.RemoveManipulator(clickable);
                }

                clickable = value;
                if (clickable != null)
                {
                    this.AddManipulator(clickable);
                }
            }
        }
        public event Action Clicked
        {
            add
            {
                if (clickable == null) clickable = new Clickable(value);
                else clickable.clicked += value;
            }
            remove
            {
                if (clickable != null) clickable.clicked -= value;
            }
        }


        public ClickElement(Action clickEvent = null)
        {
            Clickable = new Clickable(clickEvent);
        }

    }
}
