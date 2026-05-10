using Godot;
using System;

public partial class Damage : Resource
{
    [Signal]
    public delegate void damageExecutedEventHandler(Damage damage);
    public float amount;
    public bool crit;
    public dynamic source;

    public static Damage InitDamage(float amount, bool crit, dynamic source)
    {
        Damage d = new() { amount = amount, crit = crit, source = source };
        return d;
    }

    public void Hit() => EmitSignal(SignalName.damageExecuted, this);
}
