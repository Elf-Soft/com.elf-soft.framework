using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.InputSystemExtension
{
    [Serializable]
    public class BindingSettingGroup
    {
        [SerializeField] private string name;
        [SerializeField] private List<BindingSetting> settings = new();
        public string Name => name;
        public IReadOnlyList<BindingSetting> Settings => settings;

    }
}
