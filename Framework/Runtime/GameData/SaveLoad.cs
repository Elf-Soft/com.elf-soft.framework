using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElfSoft.Framework
{
    public class SaveLoad<T> where T : class
    {
        private readonly string saveName;
        private readonly string extension;
        private readonly string saveDirectory;
        public event Action<T> OnSave;
        public event Action<T> OnLoad;


        public SaveLoad(string saveName, string extension = ".sav", string directory = null)
        {
            this.saveName = saveName;
            this.extension = extension;
            saveDirectory = directory ?? Application.persistentDataPath;
        }

        public void Save(string path, T data)
        {
            OnSave?.Invoke(data);
            WrithFile(path, data);
        }

        /// <summary>
        /// 将数据保存为新文件,文件名序号++
        /// </summary>
        public void IncrementSave(T data)
        {
            int index = 0;
            string path;
            do
            {
                index++;
                path = Path.GetFullPath(Path.Combine(saveDirectory, saveName + index + extension));
            } while (File.Exists(path));

            OnSave?.Invoke(data);
            WrithFile(path, data);
        }

        public bool TryLoad(T data)
        {
            var isValid = CheckData(data);
            if (isValid) OnLoad?.Invoke(data);
            return isValid;
        }

        public bool TryLoadFromPath(string path, out T data)
        {
            data = Deserialize(path);
            return TryLoad(data);
        }

        public Dictionary<string, T> GetSaveDictionary()
        {
            Dictionary<string, T> saves = new();
            ForeachFiles((f, d) => saves.Add(f, d));
            return saves;
        }

        /// <summary>
        /// 遍历存档文件
        /// </summary>
        private void ForeachFiles(Action<string, T> callback)
        {
            if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);
            var files = Directory.GetFiles(saveDirectory, $"*{extension}");
            foreach (var file in files)
            {
                var data = Deserialize(file);
                if (CheckData(data)) callback(file, data);
            }
        }

        /// <summary>
        /// 将数据写入文件
        /// </summary>
        protected virtual void WrithFile(string path, T data)
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// 读取文件并反序列化为数据
        /// </summary>
        protected virtual T Deserialize(string path)
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// 检查数据是否有效
        /// </summary>
        protected virtual bool CheckData(T data)
        {
            return data != null;
        }

    }
}
