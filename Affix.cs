using System;
using Godot;
public partial class Affix(string id, int tier, float[] values, string text) : Resource
{
    public string id = id;
    public int tier = tier;
    public float[] values = values;
    public string text = text;
}