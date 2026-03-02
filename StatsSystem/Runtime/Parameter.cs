using System;
using UnityEngine;

namespace ElfSoft.StatsSystem
{
    [Serializable]
    public struct Parameter
    {
        [SerializeField] private float value;
        [SerializeField] private ParameterType type;
        public float Value
        {
            get => value;
            set => this.value = value;
        }
        public readonly ParameterType Type => type;

        public Parameter(float value, ParameterType type)
        {
            this.value = value;
            this.type = type;
        }

    }
}
