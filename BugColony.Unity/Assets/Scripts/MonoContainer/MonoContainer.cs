using System;
using UnityEngine;

namespace MonoContainer
{
    public abstract class MonoContainer<T>
    {
        public virtual string Name => typeof(T).Name;
        public MonoContainer()
        {
            Value = DefaultValue();
        }
        private T _value;

        public T Value
        {
            get
            {
                if (_value == null)
                {
                    Debug.LogWarning($"[MonoContainer] Value of type {typeof(T)} is null.");
                    return DefaultValue();
                }

                return _value;
            }
            set
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        public event Action<T> OnValueChanged;

        public abstract T DefaultValue();
    }


    public class IntContainer : MonoContainer<int>
    {
        public override int DefaultValue() =>  0; 
    }

    public class FloatContainer : MonoContainer<float>
    {
        public override float DefaultValue() => 0f;
    }

    public class StringContainer : MonoContainer<string>
    {
        public override string DefaultValue() => string.Empty;
    }
}