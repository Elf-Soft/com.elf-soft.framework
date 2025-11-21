using System;
using System.Collections.Generic;
using System.Reflection;

namespace ElfSoft.Framework.Editor
{
    public static class TypeMenuUtility
    {
        private static Dictionary<Type, List<TypeMenuData>> TypeMenuDataCache { get; } = new();


        public static IReadOnlyList<TypeMenuData> GetSubTypeTypeMenuDatas(Type type, Func<Type, bool> func = null)
        {
            if (!TypeMenuDataCache.ContainsKey(type))
            {
                var childen = type.GetSubTypes(func ?? IsValidType);
                SortByTypeMenuAttribute(childen);
                List<TypeMenuData> list = new();
                foreach (var c in childen)
                {
                    TypeMenuData d = c.TryGetCustomAttribute<TypeMenuAttribute>(out var attr) ?
                        new(c, attr.Path, attr.Name) :
                        new(c, null, c.Name);
                    list.Add(d);
                }
                TypeMenuDataCache.Add(type, list);
            }
            return TypeMenuDataCache[type];
        }

        /// <summary>
        /// 先按名称排序,然后按属性排序
        /// </summary>
        private static void SortByTypeMenuAttribute<T>(List<T> list) where T : MemberInfo
        {
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            list.Sort((x, y) =>
            {
                var z = x.TryGetCustomAttribute<TypeMenuAttribute>(out var xAttr) ? xAttr.Order : int.MaxValue;
                var w = y.TryGetCustomAttribute<TypeMenuAttribute>(out var yAttr) ? yAttr.Order : int.MaxValue;
                return z.CompareTo(w);
            });
        }

        private static bool IsValidType(Type type)
        {
            return
                (type.IsPublic || type.IsNestedPublic || type.IsNestedPrivate) &&
                !type.IsAbstract && !type.IsGenericType &&
                !typeof(UnityEngine.Object).IsAssignableFrom(type) &&
                //Attribute.IsDefined(type, typeof(SerializableAttribute)) &&
                !Attribute.IsDefined(type, typeof(HideInTypeMenuAttribute));
        }
    }
}
