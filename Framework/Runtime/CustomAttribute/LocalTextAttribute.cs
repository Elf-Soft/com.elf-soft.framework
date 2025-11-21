using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.Framework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LocalTextAttribute : PropertyAttribute
    {
        public readonly StyleLength MinHeight;
        public readonly StyleLength MaxHeight;


        public LocalTextAttribute() { }
        public LocalTextAttribute(float minHeight, float maxHeight)
        {
            if (minHeight > 0) MinHeight = minHeight < 1f ? Length.Percent(minHeight * 100) : minHeight;
            if (maxHeight > 0) MaxHeight = maxHeight < 1f ? Length.Percent(maxHeight * 100) : maxHeight;
        }
    }
}
