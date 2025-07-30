using System;

namespace Sorter
{
    /// <summary>
    /// Required for automatically declare Zenject signal
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class SignalAttribute : Attribute { }
}
