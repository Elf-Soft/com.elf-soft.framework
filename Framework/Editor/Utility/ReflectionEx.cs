using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace ElfSoft.Framework.Editor
{
    public static class ReflectionEx
    {
        private static Dictionary<string, Dictionary<Type, Delegate>> GetDelegateCache { get; } = new();
        private static Dictionary<string, Dictionary<Type, Delegate>> SetDelegateCache { get; } = new();
        private static Dictionary<string, Dictionary<Type, FieldInfo>> FieldInfoCache { get; } = new();
        private static Dictionary<Type, Dictionary<MemberInfo, Attribute>> AttributeCache { get; } = new();
        public const BindingFlags PublicFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        public const BindingFlags PrivateFlags = BindingFlags.NonPublic | BindingFlags.Instance;


        #region 属性
        public static TValue GetProperty<TObj, TValue>(TObj obj, string propertyName, BindingFlags flags = PublicFlags)
        {
            InitDelegateDictionary<TObj, TValue>(GetDelegateCache, propertyName, GetGetDelegate, flags);
            return (GetDelegateCache[propertyName][typeof(TObj)] as Func<TObj, TValue>)(obj);

            static Delegate GetGetDelegate(PropertyInfo info)
            {
                var method = info.GetGetMethod(true) ?? throw new Exception($"类型{typeof(TObj).Name}的属性({info.Name})没有获取方法(get)");
                return method.CreateDelegate(typeof(Func<TObj, TValue>));
            }
        }

        public static void SetProperty<TObj, TValue>(TObj obj, string propertyName, TValue value, BindingFlags flags = PublicFlags)
        {
            InitDelegateDictionary<TObj, TValue>(SetDelegateCache, propertyName, GetSetDeleget, flags);
            (SetDelegateCache[propertyName][typeof(TObj)] as Action<TObj, TValue>).Invoke(obj, value);

            static Delegate GetSetDeleget(PropertyInfo info)
            {
                var method = info.GetSetMethod(true) ?? throw new Exception($"类型{typeof(TObj).Name}的属性({info.Name})没有设置方法(Set)");
                return method.CreateDelegate(typeof(Action<TObj, TValue>));
            }
        }

        private static void InitDelegateDictionary<TObj, TValue>(Dictionary<string, Dictionary<Type, Delegate>> dic,
            string propertyName, Func<PropertyInfo, Delegate> func, BindingFlags flags)
        {
            if (!dic.TryGetValue(propertyName, out var delegateDic) || !delegateDic.ContainsKey(typeof(TObj)))
            {
                var info = typeof(TObj).GetProperty(propertyName, flags);
                if (info == null) throw new Exception($"无法在类型({typeof(TObj).Name})中找到属性({propertyName})");
                else if (info.PropertyType != typeof(TValue)) throw new Exception($"类型{typeof(TObj).Name}的属性({propertyName})类型与指定类型({typeof(TValue)})不符");
                var d = func(info);

                if (!dic.ContainsKey(propertyName)) dic[propertyName] = new();
                dic[propertyName][typeof(TObj)] = d;
            }
        }

        #endregion

        #region 字段
        public static TValue GetFieldValue<TObj, TValue>(TObj obj, string fieldName, BindingFlags flags = PrivateFlags)
        {
            var info = GetFieldInfoCache<TObj, TValue>(fieldName, flags);
            return (TValue)info.GetValue(obj);
        }

        public static void SetFieldValue<TObj, TValue>(TObj obj, string fieldName, TValue value, BindingFlags flags = PrivateFlags)
        {
            var info = GetFieldInfoCache<TObj, TValue>(fieldName, flags);
            info.SetValue(obj, value);
        }

        private static FieldInfo GetFieldInfoCache<TObj, TValue>(string fieldName, BindingFlags flags)
        {
            if (!FieldInfoCache.TryGetValue(fieldName, out var infoDic) || !infoDic.ContainsKey(typeof(TObj)))
            {
                var info = typeof(TObj).GetField(fieldName, flags);
                if (info == null) throw new Exception($"无法在类型({typeof(TObj).Name})中找到字段({fieldName})");
                else if (info.FieldType != typeof(TValue)) throw new Exception($"类型{typeof(TObj).Name}的字段({info.Name})类型与指定类型({typeof(TValue)})不符");

                if (!FieldInfoCache.ContainsKey(fieldName)) FieldInfoCache[fieldName] = new();
                FieldInfoCache[fieldName][typeof(TObj)] = info;
            }
            return FieldInfoCache[fieldName][typeof(TObj)];
        }

        #endregion

        /// <summary>
        /// 尝试获取自定义特性
        /// </summary>
        public static bool TryGetCustomAttribute<TAtt>(this MemberInfo info, out TAtt attr) where TAtt : Attribute
        {
            if (!AttributeCache.ContainsKey(typeof(TAtt))) AttributeCache[typeof(TAtt)] = new();
            if (!AttributeCache[typeof(TAtt)].ContainsKey(info))
            {
                attr = info.GetCustomAttribute<TAtt>();
                AttributeCache[typeof(TAtt)][info] = attr;
            }
            attr = AttributeCache[typeof(TAtt)][info] as TAtt;
            return attr != null;
        }

        /// <summary>
        /// 获取类型的所有非抽象子类
        /// </summary>
        public static List<Type> GetSubTypes(this Type type, Func<Type, bool> func = null, bool withSelf = false, bool withAbstract = false)
        {
            List<Type> list = new();
            if (withSelf) Handel(type);
            var types = TypeCache.GetTypesDerivedFrom(type);
            foreach (var t in types)
            {
                Handel(t);
            }
            return list;

            void Handel(Type t)
            {
                if (t.IsAbstract && !withAbstract) return;
                if (func == null || func.Invoke(t)) list.Add(t);
            }
        }

        /// <summary>
        /// Get type from SerializedProperty.managedReferenceFieldTypename
        /// </summary>
        public static Type GetType(string typeName)
        {
            try
            {
                int splitIndex = typeName.IndexOf(' ');
                var assembly = Assembly.Load(typeName[..splitIndex]);
                return assembly.GetType(typeName[(splitIndex + 1)..]);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
