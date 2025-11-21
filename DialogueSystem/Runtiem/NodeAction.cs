using ElfSoft.Framework;
using UnityEngine;

namespace ElfSoft.DialogueSystem
{
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
