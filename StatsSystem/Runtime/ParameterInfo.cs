using ElfSoft.Framework;
using System;
using UnityEngine;

namespace ElfSoft.StatsSystem
{
    [Serializable]
    public class ParameterInfo
    {
        [SerializeField] private int id;
        [SerializeField] private string abbr;
        [SerializeField, LocalText] private string name;
        public int Id => id;
        public string Abbr => abbr;
        public string Name
        {
            get
            {
                Utils.SplitLocalText(name, out var tableName, out var entryKey);
                return Utils.GetLocalizedString(tableName, entryKey);
            }
        }
    }
}
