using ElfSoft.Framework.Editor.UIElements;
using UnityEditor;

namespace ElfSoft.StatsSystem.Editor
{
    [CustomEditor(typeof(ParameterInfoData), true)]
    [EditorWindowGenerator(typeof(ParameterInfoDataEditorView), typeof(ParameterInfoData), true)]
    public class ParameterInfoDataEditor : CustomEditor<ParameterInfoDataEditorWindow>
    {
    }
}
