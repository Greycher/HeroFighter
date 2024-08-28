using System.Collections.Generic;
using System.Reflection;
using HeroFighter.Runtime;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace HeroFighter.Editor
{
    public abstract class BaseAnimatorElementAttributeDrawer : PropertyDrawer
    {
        protected const string EmptyOptionString = "None";
        private AnimatorController _animatorController;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var controller = GetAnimatorController(property);
            if (!controller)
            {
                DrawErrorLabel(position, property, label, "Animator controller can not be found!");
                return;
            }

            AnimatorElementOnGUI(controller, position, property, label);
        }

        protected abstract void AnimatorElementOnGUI(AnimatorController controller, Rect position,SerializedProperty property, GUIContent label);

        protected void DrawErrorLabel(Rect position, SerializedProperty property, GUIContent label, string errorMessage)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                normal =
                {
                    textColor = Color.red
                }
            };
            var content = new GUIContent(errorMessage);
            EditorGUI.LabelField(position, label, content, style);
        }
        
        protected AnimatorController GetAnimatorController(SerializedProperty property)
        {
            var obj = property.serializedObject.targetObject;
            if (obj == null)
            {
                return null;
            }
            
            if (!TryGetAnimatorFromPropertyName(obj, out var animator))
            {
                animator = (obj as MonoBehaviour)?.GetComponentInChildren<Animator>();
            }
            
            if (!animator)
            {
                return null;
            }
            
            if (animator.runtimeAnimatorController is AnimatorOverrideController overridenController)
            {
                _animatorController = overridenController.runtimeAnimatorController as AnimatorController;
            }
            else
            {
                _animatorController = animator.runtimeAnimatorController as AnimatorController;
            }

            return _animatorController;
        }

        public bool TryGetAnimatorFromPropertyName(Object obj, out Animator animator)
        {
            animator = default;

            if (!obj)
            {
                return false;
            }
            
            if (!(attribute is AnimatorElementAttribute attr))
            {
                Debug.LogError($"{GetType().Name} should be use with an attribute derived from {nameof(AnimatorElementAttribute)}!");
                return false;
            }
            
            if (attr.AnimatorFieldName == null)
            {
                return false;
            }

            var type = obj.GetType();
            var fieldInfo = type.GetField(attr.AnimatorFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                return false;
            }

            animator = fieldInfo.GetValue(obj) as Animator;
            return animator;
        }
    }

    [CustomPropertyDrawer(typeof(AnimatorStateAttribute))]
    public class AnimatorStateAttributeDrawer : BaseAnimatorElementAttributeDrawer
    {
        protected override void AnimatorElementOnGUI(AnimatorController controller, Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var options = new List<string>(new [] {EmptyOptionString});
            var index = 0;
            var selectedIndex = 0;
            foreach (var layer in controller.layers)
            {
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    index++;
                    if (property.intValue == state.state.nameHash)
                    {
                        selectedIndex = index;
                    }
                    options.Add(state.state.name);
                }
            }

            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, options.ToArray());
            property.intValue = selectedIndex == 0 ? -1 : Animator.StringToHash(options[selectedIndex]);
            
            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterAttributeDrawer : BaseAnimatorElementAttributeDrawer
    {
        protected override void AnimatorElementOnGUI(AnimatorController controller, Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var attr = attribute as AnimatorParameterAttribute;
            var parameters = controller.parameters;
            var options = new List<string>(new [] {EmptyOptionString});
            var index = 0;
            var selectedIndex = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.type == attr.ParameterType)
                {
                    index++;
                    if (property.intValue == parameter.nameHash)
                    {
                        selectedIndex = index;
                    }
                    options.Add(parameter.name);
                }
            }

            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, options.ToArray());
            property.intValue = selectedIndex == 0 ? -1 : Animator.StringToHash(options[selectedIndex]);
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(AnimatorFloatParameterAttribute))]
    public class AnimatorFloatParameterAttributeDrawer : AnimatorParameterAttributeDrawer { }
    
    [CustomPropertyDrawer(typeof(AnimatorBoolParameterAttribute))]
    public class AnimatorBoolParameterAttributeDrawer : AnimatorParameterAttributeDrawer { }
    
    [CustomPropertyDrawer(typeof(AnimatorIntParameterAttribute))]
    public class AnimatorIntParameterAttributeDrawer : AnimatorParameterAttributeDrawer { }
    
    [CustomPropertyDrawer(typeof(AnimatorTriggerParameterAttribute))]
    public class AnimatorTriggerAttributeDrawer : AnimatorParameterAttributeDrawer { }
}