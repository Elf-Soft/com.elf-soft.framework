using ElfSoft.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ElfSoft.InventorySystem
{
    [UxmlElement]
    public partial class InventoryPanel : VisualElement
    {
        private readonly VisualElement content;
        private readonly PageBar pageBar;
        private readonly CursorElement cursorElement;
        private int selectedIndex;
        private int activeChildCount;
        private IModuleObject<Inventory<ItemSlot>> obj;
        public override VisualElement contentContainer => content;
        private static readonly string ussClassName = "inventory-panel";


        public InventoryPanel()
        {
            UIElementEx.CloneTreeAsset(this, "UI/UIDocument/InventoryPanel.uxml");
            AddToClassList(UssClassName.panel);
            AddToClassList(ussClassName);
            content = this.Q(classes: "panel-container");
            InputManipulator m = new(InputSystem.actions.FindAction("Navigate"), OnNavigate, OnNavigate);
            this.AddManipulator(m);

            hierarchy.Add(cursorElement = new());

            pageBar = this.Q<PageBar>();
            pageBar.clickElementPrevious.Clicked += () => OnPageBarClicked(pageBar.Page - 1);
            pageBar.clickElementNext.Clicked += () => OnPageBarClicked(pageBar.Page + 1);

            EventHub.AddListener<EventData<PointerEnterEvent>>(d =>
            {
                var index = IndexOf(d.Args.target as VisualElement);
                if (index >= 0) UpdateSelection(index);
            });

        }

        public void Bind(IModuleObject<Inventory<ItemSlot>> obj)
        {
            this.obj = obj;
            Update();
        }

        public void Unbind()
        {
            obj = null;
            Reset();
        }

        public void Update()
        {
            SetupMaxPage();
            ChangePage(1);
        }

        protected void Reset()
        {
            for (int i = 0; i < childCount; i++)
            {
                var bar = GetBar(i);
                if (i == selectedIndex)
                {
                    bar.RemoveFromClassList(UssClassName.selected);
                    EventHub.SendEvent<ItemSlotBarSelectedEventData>(e => e.Init(null));
                }
                bar.Unbind();
                bar.style.visibility = Visibility.Hidden;
            }
        }

        private void SetupMaxPage()
        {
            var amount = obj.module.GetNonEmptySlotAmount();
            var maxPage = Mathf.CeilToInt((float)amount / childCount);
            pageBar.MaxPage = maxPage == 0 ? 1 : maxPage;
            if (maxPage == 0) cursorElement.style.visibility = Visibility.Hidden;
            else cursorElement.style.visibility = Visibility.Visible;
        }

        public void ChangePage(int value)
        {
            Reset();
            pageBar.Page = Mathf.Clamp(value, 1, pageBar.MaxPage);
            var startIndex = childCount * (value - 1);
            var endCount = Mathf.Clamp(startIndex + childCount, startIndex, obj.module.Slots.Count);
            activeChildCount = endCount - startIndex;
            for (int i = 0; i < childCount && startIndex < endCount; i++)
            {
                var slot = obj.module.Slots[startIndex];
                startIndex++;
                var bar = GetBar(i);
                bar.Bind(slot);
                bar.style.visibility = Visibility.Visible;
            }
            UpdateSelection(selectedIndex);
        }

        private void UpdateSelection(int value)
        {
            if (activeChildCount == 0) return;
            var index = Mathf.Clamp(value, 0, activeChildCount - 1);
            if (selectedIndex < childCount)
            {
                this[selectedIndex].RemoveFromClassList(UssClassName.selected);
                EventHub.SendEvent<ItemSlotBarSelectedEventData>(e => e.Init(null));
            }
            selectedIndex = index;
            var bar = GetBar(selectedIndex);
            _ = UIElementEx.MoveElementToAsync(cursorElement, bar.Anchor);
            bar.AddToClassList(UssClassName.selected);
            EventHub.SendEvent<ItemSlotBarSelectedEventData>(e => e.Init(bar));
        }

        private ItemSlotBar GetBar(int index) => this[index] as ItemSlotBar;


        private void OnNavigate(InputAction action)
        {
            var vector = action.ReadValue<Vector2>();
            if (!Mathf.Approximately(vector.y, 0))
            {
                var s = selectedIndex + (vector.y > 0 ? -1 : 1);
                s = s < 0 ? activeChildCount - 1 : s >= activeChildCount ? 0 : s;
                if (selectedIndex != s) UpdateSelection(s);
            }
            else if (!Mathf.Approximately(vector.x, 0))
            {
                var b = vector.x < 0;
                OnPageBarClicked(b ? pageBar.Page - 1 : pageBar.Page + 1);
                if (b) pageBar.PreviousClickedAnimation();
                else pageBar.NextClickedAnimation();
            }
        }

        private void OnPageBarClicked(int value)
        {
            value = value < 1 ? pageBar.MaxPage : value > pageBar.MaxPage ? 1 : value;
            if (pageBar.Page != value) ChangePage(value);
        }
    }
}
