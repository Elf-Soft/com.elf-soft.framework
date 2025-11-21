using ElfSoft.Framework.Editor.UIElements;
using UnityEditor;

namespace ElfSoft.InputSystemExtension.Editor
{
    [CustomEditor(typeof(BindingSettingData), true)]
    [EditorWindowGenerator(typeof(BindingSettingDataEditorWindowView), typeof(BindingSettingData), false)]
    public partial class BindingSettingDataEditor : CustomEditor<BindingSettingDataEditorWindow>
    {

    }
}
