using System;
using UnityEngine;

namespace Sorter.Utils
{
    [Serializable]
    public class IntRange : Range<int>
    {
        public override int Clamp(int value)
        {
            return Mathf.Clamp(value, Min, Max);
        }
        
        public override bool Contains(int value)
        {
            return value >= Min && value <= Max;
        }
    }
}