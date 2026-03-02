using ElfSoft.ActorSystem.StateSystem;
using ElfSoft.InputSystemExtension;
using UnityEngine;

namespace ElfSoft.ActorSystem
{
    public class ActorManager : MonoBehaviour
    {
        public Animator Anim { get; private set; }
        //public StatsController Stats { get; private set; }
        public MoveController MC { get; private set; }
        public InputController IC { get; private set; }
        //public ActorStateData StateData { get; private set; }
        //[field: SerializeField] public InventoryNormal Inventory { get; private set; }
        [field: SerializeField] public StateMachine Machine { get; private set; }


        #region MonoBehaviour

        private void Awake()
        {
            Anim = GetComponent<Animator>();
            IC = new InputController(PlayerInputController.Instance);
            MC = new MoveController(this);
            //Inventory = Inventory.Clone() as InventoryNormal;
            //StateData = new(this);
            Machine.Init(this);
            //MonoBehaviourEvent.TriggerEvent(this, MonoBehaviourEvent.MonoBehaviourLifeCycle.Awake);
        }

        private void Update()
        {
            Machine.Update();
        }

        #endregion

        //public void LoadData(IActorSaveData data)
        //{
        //    transform.SetLocalPositionAndRotation(data.Position, data.Rotaion);
        //    foreach (var id in Stats.ResourceDic.Keys)
        //    {
        //        if (data.ResourceDic.ContainsKey(id))
        //        {
        //            var oldValue = Stats.ResourceDic[id].Value;
        //            var newValue = data.ResourceDic[id];
        //            Stats.ResourceDic[id].Value = newValue;
        //            EventHub.SendEvent<StatsEvent>(e => e.Init(Stats, id, oldValue, newValue));
        //        }
        //    }
        //    foreach (var id in Stats.ParameterDic.Keys)
        //    {
        //        if (data.ParameterDic.ContainsKey(id))
        //        {
        //            var oldValue = Stats.ParameterDic[id].Value;
        //            Stats.ParameterDic[id].LoadData(data.ParameterDic[id]);
        //            var newValue = Stats.ParameterDic[id].Value;
        //            EventHub.SendEvent<StatsEvent>(e => e.Init(Stats, id, oldValue, newValue));
        //        }
        //    }
        //}
    }
}

