using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElfSoft.InputSystemExtension
{
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private BindingSettingData setting;
        [SerializeField] private Location saveLocation;
        [SerializeField] private string saveName = "bindings";
        [SerializeField] private bool instance;
        private enum Location
        {
            PersistentDataPath, PlayerPrefs
        }
        private string DataPath => Path.GetFullPath(Path.Combine(Application.persistentDataPath, saveName, ".json"));
        private readonly Dictionary<int, InputAction> actionDic = new();
        public PlayerInput PlayerInput { get; private set; }
        public int ActiveMapIndex { get; set; }
        public BindingSettingGroup ActiveSettingMap => setting.Groups[ActiveMapIndex];
        public InputAction this[int index] => actionDic[index];
        public static PlayerInputController Instance { get; private set; }


        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            foreach (var map in PlayerInput.actions.actionMaps)
            {
                foreach (var a in map.actions)
                {
                    actionDic.Add(Animator.StringToHash(a.name), a);
                }
            }
            if (instance) Instance = this;
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

        //public string GetBindingDisplayString(string bindingName)
        //{
        //    TryGetSetting(bindingName, out var setting);
        //    var action = PlayerInput.actions.FindAction(setting.Reference.action.id);
        //    return action.GetBindingDisplayString(setting.BindingIndex);
        //}

        public void StartInteractiveRebind(string bindingName)
        {
            TryGetSetting(bindingName, out var setting);
            var action = PlayerInput.actions.FindAction(setting.Reference.action.id);
            var bindingIndex = setting.BindingIndex;

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

        public void SaveData()
        {
            var data = PlayerInput.actions.SaveBindingOverridesAsJson();
            if (saveLocation == Location.PlayerPrefs) PlayerPrefs.SetString(saveName, data);
            else File.WriteAllText(DataPath, data);
        }

        public void LoadData()
        {
            string data = string.Empty;
            if (saveLocation == Location.PlayerPrefs) data = PlayerPrefs.GetString(saveName);
            else if (File.Exists(DataPath)) data = File.ReadAllText(DataPath);
            if (string.IsNullOrEmpty(data)) return;
            PlayerInput.actions.LoadBindingOverridesFromJson(data);
        }
    }
}
