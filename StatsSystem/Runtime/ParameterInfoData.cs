using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.StatsSystem
{
    [CreateAssetMenu(menuName = "GameData/StatsSystem/ParameterInfoData", fileName = "ParameterInfoData")]
    public class ParameterInfoData : ScriptableObject
    {
        [SerializeField] private List<ParameterInfo> infos = new();
        public IReadOnlyList<ParameterInfo> Infos => infos;
    }
}
