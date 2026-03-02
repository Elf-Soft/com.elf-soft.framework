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
        [SerializeField] private bool percent;
        [SerializeField, LocalText] private string name;
        public int Id => id;
        public string Abbr => abbr;
        public bool Percent => percent;
        public string Name => LocalizationEx.GetLocalizedString(name);
    }
}
