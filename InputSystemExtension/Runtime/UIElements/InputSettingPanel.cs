/*using ElfSoft.UIElements;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ElfSoft.InputSystemExtension.UIElements
{
    public class InputSettingPanel : VisualElement, IView<PlayerInputController>
    {
        protected List<BindingBar> bars;
        public Button ResetButton { get; private set; }
        public Button SubmitButton { get; private set; }
        public PlayerInputController InputControl { get; private set; }
        public bool IsEditing { get; set; }
        public static readonly string EditingStyle = "editing";
        public virtual string SettingInfo => "Wait Input...";


        public InputSettingPanel() : this("UI/UIDocument/InputSettingPanel.uxml") { }

        public InputSettingPanel(string docPath)
        {
            UIElement.CloneTreeAsset(this, docPath);
            ResetButton = this.Q<Button>("reset-button");
            SubmitButton = this.Q<Button>("submit-button");
            ResetButton.clicked += ResetAction;
            SubmitButton.clicked += Close;

            bars = new();
            var content = this.Q<ScrollView>().contentContainer;
            content.Query<BindingBar>().ForEach(b =>
            {
                b.SettingPanel = this;
                if (!string.IsNullOrEmpty(b.BindingName))
                {
                    bars.Add(b);
                }
            });
        }

        public void Bind(PlayerInputController ctrl)
        {
            InputControl = ctrl;
            //InputControl.ActiveMapIndex = index;
            foreach (var bar in bars)
            {
                bar.UpdateView();
            }
        }

        public void Unbind()
        {
            if (IsEditing || !InputControl) return;
            InputControl.SaveData();
            InputControl = null;
        }

        public bool CanClose() => !IsEditing;

        private void ResetAction()
        {
            if (IsEditing) return;
            InputControl.RemoveAllBindingOverrides();
            foreach (var bar in bars)
            {
                bar.UpdateView();
            }
        }

        private void Close()
        {
            if (IsEditing || !InputControl) return;
            Unbind();
            UIDocumentController.Instance.RemoveFromRoot(this);
        }

        ~InputSettingPanel() => UIElement.Release(this);
    }
}
*/