using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    public class DialogueDataEditorWindowView : TableView<DialogueData>
    {
        public DialogueGraphView GraphView { get; private set; }
        public VisualElement PropertiesPanel { get; private set; }


        public DialogueDataEditorWindowView()
        {
            var right = this.Q<VisualElement>("right-panel");
            right.Q<VisualElement>("header").style.display = DisplayStyle.None;
            right.Q<VisualElement>("click-area").style.display = DisplayStyle.None;
            GraphView = new(this);
            right.Q<VisualElement>("body").Add(GraphView);

            var left = this.Q<VisualElement>("left-panel");
            left.Q<VisualElement>("header").style.display = DisplayStyle.None;
            PropertiesPanel = left.Q<VisualElement>("body");

            Menu.menu.AppendAction("Show tables window",
                a => LocalizationEditorEx.ShowTableWindow(nameof(DialogueData)), CheckAsset);
        }

        protected override void UpdateView()
        {
            PropertiesPanel.Clear();
            base.UpdateView();
            GraphView.UpdateView();
        }

        public void RefreshProperties(VisualElement element)
        {
            PropertiesPanel.Clear();
            PropertiesPanel.Add(element);
        }
    }
}
