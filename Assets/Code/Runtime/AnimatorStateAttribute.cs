using UnityEngine;

namespace HeroFighter.Runtime
{
    public class AnimatorElementAttribute : PropertyAttribute
    {
        public string AnimatorFieldName;

        public AnimatorElementAttribute() { }

        public AnimatorElementAttribute(string animatorFieldName = "animator")
        {
            AnimatorFieldName = animatorFieldName;
        }
        
    }
    
    public class AnimatorStateAttribute : AnimatorElementAttribute { }

    public class AnimatorParameterAttribute : AnimatorElementAttribute
    {
        public AnimatorControllerParameterType ParameterType { get; }

        public AnimatorParameterAttribute()
        {
            ParameterType = AnimatorControllerParameterType.Float;
        }

        public AnimatorParameterAttribute(AnimatorControllerParameterType parameterType)
        {
            ParameterType = parameterType;
        }
        
        public AnimatorParameterAttribute(string animatorFieldName, AnimatorControllerParameterType parameterType) : base(animatorFieldName)
        {
            ParameterType = parameterType;
        }
    }
    
    public class AnimatorFloatParameterAttribute : AnimatorParameterAttribute
    {
        public AnimatorFloatParameterAttribute() : base(AnimatorControllerParameterType.Float) { }

        public AnimatorFloatParameterAttribute(string animatorFieldName) : base(animatorFieldName, AnimatorControllerParameterType.Float)  { }
    }
    
    public class AnimatorBoolParameterAttribute : AnimatorParameterAttribute 
    {
        public AnimatorBoolParameterAttribute() : base(AnimatorControllerParameterType.Bool) { }

        public AnimatorBoolParameterAttribute(string animatorPropertyName) : base(animatorPropertyName, AnimatorControllerParameterType.Bool)  { }
    }
    public class AnimatorIntParameterAttribute : AnimatorParameterAttribute
    {
        public AnimatorIntParameterAttribute() : base(AnimatorControllerParameterType.Int) { }

        public AnimatorIntParameterAttribute(string animatorPropertyName) : base(animatorPropertyName, AnimatorControllerParameterType.Int)  { }
    }
    
    public class AnimatorTriggerParameterAttribute : AnimatorParameterAttribute
    {
        public AnimatorTriggerParameterAttribute() : base(AnimatorControllerParameterType.Trigger) { }

        public AnimatorTriggerParameterAttribute(string animatorPropertyName) : base(animatorPropertyName, AnimatorControllerParameterType.Trigger)  { }
    }
}