using ElfSoft.StatsSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.InventorySystem
{
    [Serializable]
    public class EquipmentInfo : ItemInfo
    {
        [SerializeField] private Parameter[] initials;
        [SerializeField] private List<Parameter> extras;
        public IReadOnlyList<Parameter> Initials => initials;
        public IReadOnlyList<Parameter> Extras => extras;

    }
}
