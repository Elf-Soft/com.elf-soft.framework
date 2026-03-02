using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ElfSoft.InputSystemExtension.Editor
{
    public class BindingSettingView : VisualElement
    {
        private readonly Foldout foldout;
        private readonly ObjectField referenceField;
        private readonly DropdownField bindingField;
        private SerializedProperty property;
        private InputActionReference Reference => referenceField.value as InputActionReference;


        public BindingSettingView()
        {
            foldout = new();
            Add(foldout);

            referenceField = new(nameof(Reference)) { objectType = typeof(InputActionReference) };
            referenceField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue == e.previousValue) return;
                UpdateBindingField(0);
                ApplyProperty();
            });
            foldout.Add(referenceField);

            bindingField = new(nameof(Binding));
            bindingField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue == e.previousValue) return;
                UpdateName(bindingField.index);
                ApplyProperty();
            });
            foldout.Add(bindingField);

            this.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Copy Name",
                    a => GUIUtility.systemCopyBuffer = foldout.text,
                    a => !string.IsNullOrEmpty(foldout.text) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }));

            foldout.RegisterValueChangedCallback(e => { if (property != null) property.isExpanded = e.newValue; });
        }

        public void BindProperty(SerializedProperty prop)
        {
            property = prop;
            UpdateView();
        }

        public void Unbind()
        {
            property = null;
            referenceField.SetValueWithoutNotify(null);
            UpdateName(0);
        }

        private void UpdateView()
        {
            property.serializedObject.Update();
            referenceField.SetValueWithoutNotify(property.FindPropertyRelative("reference").objectReferenceValue);
            UpdateBindingField(property.FindPropertyRelative("bindingIndex").intValue);
            foldout.SetValueWithoutNotify(property.isExpanded);
        }

        //更新下拉列表显示项
        private void UpdateBindingField(int index)
        {
            List<string> list = new();
            if (Reference != null)
            {
                var bindings = Reference.action.bindings;
                for (int i = 0; i < bindings.Count; i++)
                {
                    list.Add(GetBindingDisplayName(bindings[i], i));
                }
            }
            bindingField.choices = list;
            bindingField.SetValueWithoutNotify(index >= 0 && list.Count > index ? list[index] : string.Empty);
            UpdateName(index);
        }

        private void UpdateName(int index)
        {
            if (index >= 0 && Reference != null && Reference.action.bindings.Count > index)
            {
                var action = Reference.action;
                var binding = action.bindings[index];

                foldout.text = binding.isPartOfComposite ? $"{action.name}_{binding.name}" : action.name;
            }
            else foldout.text = "<null>";
        }

        private void ApplyProperty()
        {
            property.FindPropertyRelative("name").stringValue = foldout.text;
            property.FindPropertyRelative("reference").objectReferenceValue = Reference;
            property.FindPropertyRelative("bindingIndex").intValue = bindingField.index;
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 获取绑定的显示名称,
        /// 修改自Unity示例场景(Rebinding UI)脚本<InputBindingSettingCustomEditor.RefreshBindingOptions()>
        /// </summary>
        private string GetBindingDisplayName(InputBinding binding, int index)
        {
            var action = Reference.action;
            var asset = action.actionMap?.asset;

            var haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

            // If we don't have a binding groups (control schemes), show the device that if there are, for example,
            // there are two bindings with the display string "A", the user can see that one is for the keyboard
            // and the other for the gamepad.
            var displayOptions =
                InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
            if (!haveBindingGroups)
                displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

            // Create display string.
            var displayString = action.GetBindingDisplayString(index, displayOptions);

            // If binding is part of a composite, include the part name.
            if (binding.isPartOfComposite)
                displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

            // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
            // by instead using a backlash.
            displayString = displayString.Replace('/', '\\');

            // If the binding is part of control schemes, mention them.
            if (haveBindingGroups)
            {
                if (asset != null)
                {
                    var controlSchemes = string.Join("", binding.groups.Split(InputBinding.Separator).
                        Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                    displayString = $"{displayString} ({controlSchemes})";
                }
            }
            return displayString;
        }
    }
}
