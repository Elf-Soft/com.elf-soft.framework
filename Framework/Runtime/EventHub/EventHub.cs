using System;
using System.Collections.Generic;

namespace ElfSoft.Framework
{
    public static class EventHub
    {
        private static readonly Dictionary<Type, Delegate> delegateDict = new();
        private static readonly Dictionary<Type, EventData> dataDict = new();


        /// <summary>
        /// 添加一个事件的监听者
        /// </summary>
        public static void AddListener<T>(Action<T> callback) where T : EventData
        {
            var type = typeof(T);
            if (delegateDict.ContainsKey(type))
            {
                delegateDict[type] = Delegate.Combine(delegateDict[type], callback);
            }
            else delegateDict[type] = callback;
        }

        /// <summary>
        /// 移除一个事件的监听者
        /// </summary>
        public static void RemoveListener<T>(Action<T> callback) where T : EventData
        {
            var type = typeof(T);
            if (delegateDict.ContainsKey(type))
            {
                delegateDict[type] = Delegate.Remove(delegateDict[type], callback);
            }
        }

        /// <summary>
        /// 触发事件,传入事件参数初始化方法
        /// </summary>
        public static void SendEvent<T>(Action<T> dataInitAction) where T : EventData, new()
        {
            var type = typeof(T);
            if (delegateDict.TryGetValue(type, out var value) && value is Action<T> listeners)
            {
                if (!dataDict.ContainsKey(type)) dataDict[type] = new T();
                var data = dataDict[type] as T;
                dataInitAction(data);
                listeners.Invoke(data);
                data.Reset();
            }
        }
    }
}
