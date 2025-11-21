using ElfSoft.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.DialogueSystem
{
    [CreateAssetMenu(menuName = "GameData/DialogueData", fileName = "DialogueData")]
    public class DialogueData : ScriptableObject
    {
        [SerializeReference] private List<Node> nodes = new();
        public IReadOnlyList<Node> Nodes => nodes;

    }
}
