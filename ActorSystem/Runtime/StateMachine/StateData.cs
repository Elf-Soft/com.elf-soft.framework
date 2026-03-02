using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.ActorSystem.StateSystem
{
    [CreateAssetMenu(menuName = "GameData/ActorSystem/StateData", fileName = "StateData", order = -1)]
    public class StateData : ScriptableObject
    {
        [SerializeReference] private List<State> states;
        public IReadOnlyList<State> States => states;
    }
}
