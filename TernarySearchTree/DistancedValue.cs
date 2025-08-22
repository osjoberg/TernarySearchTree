using System;
using System.Collections.Generic;

namespace TernarySearchTree
{
    public struct DistancedValue<TValue>(int distance, TValue value): IComparable<DistancedValue<TValue>>
    {
        public int Distance = distance;
        public TValue Value = value;
        
        public int CompareTo(DistancedValue<TValue> other)
        {
            var compareDistance = Distance.CompareTo(other.Distance);
            return compareDistance != 0 ? compareDistance : Comparer<TValue>.Default.Compare(Value, other.Value);
        }
    }
}