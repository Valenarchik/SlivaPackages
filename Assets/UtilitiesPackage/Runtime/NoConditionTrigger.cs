
using System;

public class NoConditionTrigger: Trigger
{
    private bool triggered;
    public override bool Triggered => triggered;
    public override event Action TriggeredEvent;

    private void Start()
    {
        triggered = true;
        TriggeredEvent?.Invoke();
    }
}
