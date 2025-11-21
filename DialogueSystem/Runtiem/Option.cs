using ElfSoft.Framework;
using System;
using UnityEngine;

namespace ElfSoft.DialogueSystem
{
    [Serializable]
    public class Option
    {
        [SerializeField] private int id;
        [SerializeField] private int next = -1;
        [SerializeField, LocalText] private string text;
        public int Id => id;
        public int Next => next;
        public string Text => text;


    }
}
