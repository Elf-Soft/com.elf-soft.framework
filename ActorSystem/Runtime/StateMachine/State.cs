namespace ElfSoft.ActorSystem.StateSystem
{
    [System.Serializable]
    public abstract class State
    {
        public abstract int NameHash { get; }
        public StateMachine Machine { get; private set; }


        public State Clone() => MemberwiseClone() as State;

        public virtual void Init(StateMachine machine)
        {
            Machine = machine;
        }

        public void To(int nameHash) => Machine.SwitchState(nameHash);

        public virtual void Enter() { }

        public virtual void Update() { }

        public virtual void Exit() { }
    }
}
