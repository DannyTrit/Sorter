using System;
using UnityEngine;

namespace Sorter.Utils
{
    [Serializable]
    public class FloatRange : Range<float>
    {
        public override float Clamp(float value)
        {
            return Mathf.Clamp(value, Min, Max);
        }
        
        public override bool Contains(float value)
        {
            return value >= Min && value <= Max;
        }
    }
}