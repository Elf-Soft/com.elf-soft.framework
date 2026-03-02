using ElfSoft.Framework;
using System;
using UnityEngine;

namespace ElfSoft.DialogueSystem
{
    [Serializable]
    [TypeMenu(2, path: "Action/")]
    public abstract class NodeAction : Node
    {
        [SerializeField] private int next = -1;
        public int Next => next;
    }

    public sealed class NodeDebugLog : NodeAction
    {

    }
}
