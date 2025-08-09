using Godot;
using System;

public partial class InvItem : ColorRect
{
    public bool picked = false;

    public void Pick()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        picked = true;
    }

    public void Put()
    {
        MouseFilter = MouseFilterEnum.Pass;
        picked = false;
    }
}
