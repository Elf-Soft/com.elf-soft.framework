using ElfSoft.Framework.Editor.UIElements;
using UnityEditor;

namespace ElfSoft.DialogueSystem.Editor
{
    [CustomEditor(typeof(DialogueData), true)]
    [EditorWindowGenerator(typeof(DialogueDataEditorWindowView), typeof(DialogueData), true)]
    public class DialogueDataEditor : CustomEditor<DialogueDataEditorWindow>
    {

    }
}
