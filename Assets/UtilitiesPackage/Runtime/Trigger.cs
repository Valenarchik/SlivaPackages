using System;
using UnityEngine;

public abstract class Trigger: MonoBehaviour
{
    public abstract bool Triggered { get; }
    public abstract event Action TriggeredEvent;
}
