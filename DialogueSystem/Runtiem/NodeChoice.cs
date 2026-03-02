using ElfSoft.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.DialogueSystem
{
    [Serializable]
    [TypeMenu(1, name: "Choice")]
    public sealed class NodeChoice : Node
    {
        [SerializeField, LocalText] private string text;
        [SerializeField] private List<Option> choices = new();
        public string Text => text;
        public IReadOnlyList<Option> Choices => choices;


        public override void OnEnter(IView view)
        {
            view.ShowText(Text);
            view.ShowChoices(choices);
        }

        public override void OnExit(IView view)
        {
            view.ClearChoices();
        }
    }

}
