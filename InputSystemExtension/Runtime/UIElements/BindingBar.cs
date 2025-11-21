/*using Unity.Properties;
using UnityEngine.UIElements;

namespace ElfSoft.InputSystemExtension.UIElements
{
    [UxmlElement]
    public sealed partial class BindingBar : VisualElement
    {
        public Label ActionLabel { get; private set; }
        public Button SettingButton { get; private set; }
        public Label InfoLabel { get; private set; }
        [CreateProperty, UxmlAttribute] public string BindingName { get; set; }
        [CreateProperty, UxmlAttribute] public string ActionText { get => ActionLabel.text; set => ActionLabel.text = value; }
        public InputSettingPanel SettingPanel { get; internal set; }


        public BindingBar()
        {
            ActionLabel = new() { text = "Action" };
            ActionLabel.AddToClassList("text-label");
            Add(ActionLabel);

            VisualElement buttonContainer = new();
            buttonContainer.style.flexGrow = 1;
            buttonContainer.style.alignItems = Align.FlexEnd;

            SettingButton = new() { text = "Key" };
            SettingButton.AddToClassList("custom-button");
            SettingButton.clicked += OnSettingButtonClicked;
            buttonContainer.Add(SettingButton);
            Add(buttonContainer);

            InfoLabel = new();
            InfoLabel.AddToClassList("text-label");
            InfoLabel.AddToClassList("back-label");
            buttonContainer.Add(InfoLabel);
        }

        public void UpdateView()
        {
            SettingButton.text = SettingPanel.InputControl.GetBindingDisplayString(BindingName);
        }

        private void OnSettingButtonClicked()
        {
            if (SettingPanel.IsEditing || !SettingPanel.InputControl.TryGetSetting(BindingName, out _)) return;
            SettingPanel.InputControl.RegisterRebindStartOnce(() => OnRebindStoped(true));
            SettingPanel.InputControl.RegisterRebindStopOnce(() => OnRebindStoped(false));
            SettingPanel.InputControl.StartInteractiveRebind(BindingName);
        }

        private void OnRebindStoped(bool isEditing)
        {
            if (isEditing)
            {
                AddToClassList(InputSettingPanel.EditingStyle);
                InfoLabel.text = SettingPanel.SettingInfo;
            }
            else
            {
                RemoveFromClassList(InputSettingPanel.EditingStyle);
                InfoLabel.text = string.Empty;
            }
            UpdateView();
            SettingPanel.IsEditing = isEditing;
        }
    }
}
*/