using System;

namespace MissionControl.Host.Core.Utilities
{
    public struct Maybe<T> where T:class
    {
        private readonly T _value;

        public Maybe(T value)
        {
            _value = value;
        }

        public bool IsSome => _value != null;
        public bool IsNull => _value == null;

        public T Value
        {
            get
            {
                if (IsNull)
                    throw new NullReferenceException("Value is null");

                return _value;
            }
        }

        public T ValueOrNull => _value;

        public static Maybe<T> From(T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> None()
        {
            return new Maybe<T>(null);
        }
        
        public static implicit operator Maybe<T>(T valueOrNull)
        {
            return From(valueOrNull);
        }
    }
}