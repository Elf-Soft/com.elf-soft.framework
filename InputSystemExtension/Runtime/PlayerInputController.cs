using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElfSoft.InputSystemExtension
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] protected BindingSettingData setting;
        protected Dictionary<int, InputAction> actionDic = new();
        public PlayerInput PlayerInput { get; protected set; }
        public int ActiveMapIndex { get; set; }
        public BindingSettingGroup ActiveSettingMap => setting.Groups[ActiveMapIndex];
        public InputAction this[int index] => actionDic[index];


        protected virtual void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            foreach (var map in PlayerInput.actions.actionMaps)
            {
                foreach (var a in map.actions)
                {
                    actionDic.Add(Animator.StringToHash(a.name), a);
                }
            }
        }

        private void Start()
        {
            LoadData();
        }

        public bool TryGetSetting(string name, out BindingSetting setting)
        {
            setting = (ActiveSettingMap.Settings as List<BindingSetting>).Find(s => s.Name == name);
            return setting != null;
        }

        public string GetBindingDisplayString(string bindingName)
        {
            TryGetSetting(bindingName, out var setting);
            var action = PlayerInput.actions.FindAction(setting.Reference.action.id);
            return action.GetBindingDisplayString(setting.BindingIndex);
        }

        public void StartInteractiveRebind(string bindingName)
        {
            TryGetSetting(bindingName, out var setting);
            var action = PlayerInput.actions.FindAction(setting.Reference.action.id);
            var bindingIndex = setting.BindingIndex;
            if (bindingIndex < 0) return;

            //禁用复合绑定
            if (action.bindings[bindingIndex].isComposite) throw new InvalidOperationException("禁用复合绑定");

            PerformInteractiveRebind(action, bindingIndex);
        }

        /// <summary>
        /// 执行按键绑定
        /// </summary>
        private void PerformInteractiveRebind(InputAction action, int bindingIndex, Action onStart = null, Action onStop = null)
        {
            var enable = action.enabled;
            action.Disable();
            var operation = action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsExcluding("Mouse")//排除的目标控件类型
                    .WithCancelingThrough("*/{Cancel}")//取消本次重绑定按键
                    .OnCancel(CleanUp)
                    .OnComplete(CleanUp);
            onStart?.Invoke();
            operation.Start();

            void CleanUp(InputActionRebindingExtensions.RebindingOperation o)
            {
                onStop?.Invoke();
                if (enable) action.Enable();
                o.Dispose();
            }
        }

        /// <summary>
        /// 重置当前活动map所有绑定
        /// </summary>
        public void RemoveAllBindingOverrides()
        {
            foreach (var setting in ActiveSettingMap.Settings)
            {
                var action = PlayerInput.actions.FindAction(setting.Reference.action.id);
                action.RemoveBindingOverride(setting.BindingIndex);
            }
        }

        public virtual void SaveData()
        {
            var data = PlayerInput.actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(nameof(BindingSettingData), data);
        }

        public virtual void LoadData()
        {
            var data = PlayerPrefs.GetString(nameof(BindingSettingData));
            if (string.IsNullOrEmpty(data)) return;
            PlayerInput.actions.LoadBindingOverridesFromJson(data);
        }
    }
}
