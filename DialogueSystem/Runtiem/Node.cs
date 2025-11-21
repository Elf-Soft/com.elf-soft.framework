using System;
using UnityEngine;

namespace ElfSoft.DialogueSystem
{
    [Serializable]
    public abstract class Node
    {
        [SerializeField] private int id;
        public int Id => id;

        public virtual void OnEnter(IView panel) { }
        public virtual void OnExit(IView panel) { }


#if UNITY_EDITOR
        [SerializeField] private Vector2 position;
#endif
    }
}
