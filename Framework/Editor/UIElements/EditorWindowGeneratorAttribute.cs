using System;

namespace ElfSoft.Framework.Editor.UIElements
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorWindowGeneratorAttribute : Attribute
    {
        public readonly Type m_ViewType;
        public readonly Type m_TargetType;
        public readonly bool m_AddToWindowMenu;

        public EditorWindowGeneratorAttribute(Type viewType, Type targetType, bool addToWindowMenu)
        {
            m_ViewType = viewType;
            m_TargetType = targetType;
            m_AddToWindowMenu = addToWindowMenu;
        }
    }
}
