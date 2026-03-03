using System;

namespace ElfSoft.Framework.Editor.UIElements
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GameDataEditorGeneratorAttribute : Attribute
    {
        public readonly Type DataType;
        public readonly bool AddToWindowMenu;

        public GameDataEditorGeneratorAttribute(Type dataType, bool addToWindowMenu)
        {
            DataType = dataType;
            AddToWindowMenu = addToWindowMenu;
        }
    }
}
