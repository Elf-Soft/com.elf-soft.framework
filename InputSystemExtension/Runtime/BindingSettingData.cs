using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.InputSystemExtension
{
    [CreateAssetMenu(menuName = "GameData/InputSystemEx/BindingSettingData", fileName = "BindingSettingData")]
    public class BindingSettingData : ScriptableObject
    {
        [SerializeField] private List<BindingSettingGroup> groups = new();
        public IReadOnlyList<BindingSettingGroup> Groups => groups;
    }

}
