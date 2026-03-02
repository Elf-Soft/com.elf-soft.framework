using ElfSoft.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace ElfSoft.InventorySystem
{
    [UxmlElement]
    public partial class ItemSlotBar : VisualElement
    {
        private readonly Image iconImage;
        private readonly TextElement nameTextElement, stackTextElement;
        private readonly VisualElement anchor;
        private static readonly string ussClassName = "item-slot-bar";
        private static readonly string iconUssClassName = ussClassName + "__icon";
        private static readonly string nameLabelUssClassName = ussClassName + "__name-label";
        private static readonly string stackLabelUssClassName = ussClassName + "__stack-label";
        private AsyncOperationHandle<Sprite> handel;
        public VisualElement Anchor => anchor;
        [UxmlAttribute]
        public string NameText
        {
            get => nameTextElement.text;
            set => nameTextElement.text = value;
        }
        [UxmlAttribute]
        public string StackText
        {
            get => stackTextElement.text;
            set => stackTextElement.text = value;
        }
        public ItemSlot Slot { get; private set; }


        public ItemSlotBar()
        {
            AddToClassList(ussClassName);
            style.flexDirection = FlexDirection.Row;
            this.AddManipulator(new Clickable(_ => Debug.Log($"{Slot?.Item.Info.Name} x {Slot?.Stack}")));
            RegisterCallback<PointerEnterEvent>(evt => EventHub.SendEvent<EventData<PointerEnterEvent>>(e => e.Init(evt.target, evt)));

            Add(anchor = new());
            anchor.AddToClassList(UssClassName.anchor);

            Add(iconImage = new Image());
            iconImage.AddToClassList(iconUssClassName);

            Add(nameTextElement = new());
            nameTextElement.AddToClassList(nameLabelUssClassName);

            Add(stackTextElement = new());
            stackTextElement.AddToClassList(stackLabelUssClassName);
        }

        public void Bind(ItemSlot slot)
        {
            this.Slot = slot;
            EventHub.AddListener<SlotChangedEventData>(OnSlotChange);
            Update();
        }

        public void Unbind()
        {
            Slot = null;
            EventHub.RemoveListener<SlotChangedEventData>(OnSlotChange);
            Reset();
        }

        private void OnSlotChange(SlotChangedEventData e)
        {
            if (e.Target == Slot) Update();
        }

        protected void Update()
        {
            if (Slot?.Item != null)
            {
                handel.TryRelease();
                handel = Addressables.LoadAssetAsync<Sprite>(Slot.Item.Info.Icon);
                handel.Completed += h => iconImage.sprite = h.Result;

                nameTextElement.text = Slot.Item.Info.Name;
                stackTextElement.text = Slot.Stack.ToString();
            }
            else Reset();
        }

        protected void Reset()
        {
            iconImage.sprite = null;
            handel.TryRelease();
            nameTextElement.text = string.Empty;
            stackTextElement.text = string.Empty;
        }

    }
}
