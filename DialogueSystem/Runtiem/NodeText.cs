using ElfSoft.Framework;
using UnityEngine;

namespace ElfSoft.DialogueSystem
{
    [TypeMenu(0)]
    public sealed class NodeText : NodeAction
    {
        [SerializeField, LocalText] private string text;
        public string Text => text;

        public override void OnEnter(IView view)
        {
            view.ShowText(Text);
        }
    }
}
