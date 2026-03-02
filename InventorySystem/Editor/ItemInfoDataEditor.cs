using ElfSoft.Framework.Editor.UIElements;
using UnityEditor;

namespace ElfSoft.InventorySystem.Editor
{
    [CustomEditor(typeof(ItemInfoData), true)]
    [EditorWindowGenerator(typeof(ItemInfoDataEditorWindowView), typeof(ItemInfoData), true)]
    public partial class ItemInfoDataEditor : CustomEditor<ItemInfoDataEditorWindow>
    {

    }
}
