using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class SerializeReferenceButtonAttribute : PropertyAttribute
{
    public readonly bool showIndexInLabel;
    
    public SerializeReferenceButtonAttribute(bool showIndexInLabel = false)
    {
        this.showIndexInLabel = showIndexInLabel;
    }
}