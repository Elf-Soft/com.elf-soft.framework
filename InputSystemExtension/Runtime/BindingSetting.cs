using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElfSoft.InputSystemExtension
{
    [Serializable]
    public class BindingSetting
    {
        [SerializeField] private string name;
        [SerializeField] private InputActionReference reference;
        [SerializeField] private int bindingIndex;
        public string Name => name;
        public InputActionReference Reference => reference;
        public int BindingIndex => bindingIndex;

    }
}
