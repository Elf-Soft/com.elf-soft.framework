using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.ActorSystem.StateSystem
{
    [Serializable]
    public class StateMachine
    {
        [SerializeField] private StateData data;
        private readonly Dictionary<int, State> stateDic = new();
        public object Target { get; private set; }
        public State CurrentState { get; private set; }


        public void Init(object target)
        {
            Target = target;
            stateDic.Clear();
            foreach (var state in data.States)
            {
                var s = state.Clone();
                s.Init(this);
                stateDic.Add(s.NameHash, s);
            }
            SwitchState(data.States[0].NameHash);
        }

        public void SwitchState(int nameHash)
        {
            CurrentState?.Exit();
            CurrentState = stateDic[nameHash];
            CurrentState.Enter();
        }

        public void Update() => CurrentState.Update();
    }
}
