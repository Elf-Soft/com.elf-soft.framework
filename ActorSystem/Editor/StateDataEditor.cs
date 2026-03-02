using ElfSoft.Framework.Editor.UIElements;

namespace ElfSoft.ActorSystem.StateSystem.Editor
{
    //[CustomEditor(typeof(StateData), true)]
    [EditorWindowGenerator(typeof(StateDataEditorWindowView), typeof(StateData), false)]
    public partial class StateDataEditor : CustomEditor<StateDataEditorWindow>
    {

    }
}
