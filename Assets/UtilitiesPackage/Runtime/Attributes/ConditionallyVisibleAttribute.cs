using System;
using UnityEngine;

public class ConditionallyVisibleAttribute: PropertyAttribute
{
    public VisibilityAttributeMode Mode { get; }
    public string PropertyName { get; }
    public bool Invert { get; }
    public object Value { get; }
    
    public ConditionallyVisibleAttribute(string propName, bool invert = false)
    {
        Mode = VisibilityAttributeMode.Boolean;
        PropertyName = propName;
        Invert = invert;
    }
    
    public ConditionallyVisibleAttribute(string propName, object value)
    {
        Mode = VisibilityAttributeMode.Object;
        PropertyName = propName;
        Value = value;
    }
}

public enum VisibilityAttributeMode
{
    Boolean,
    Object
}
