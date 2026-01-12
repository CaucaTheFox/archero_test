using System;
using UnityEngine;

/// None of this types or interface types are valid.
[AttributeUsage(AttributeTargets.Field)]
public class SerializeReferenceUIRestrictionExcludeTypes : PropertyAttribute
{
    public readonly Type[] Types;
    public SerializeReferenceUIRestrictionExcludeTypes(params Type[] types) => Types = types;
} 

[AttributeUsage(AttributeTargets.Field)]
public class SerializeReferenceUIRestrictionExcludeEqualTypes : PropertyAttribute
{
    public readonly Type[] Types;
    public SerializeReferenceUIRestrictionExcludeEqualTypes(params Type[] types) => Types = types;
} 