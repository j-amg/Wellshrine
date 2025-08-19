using System;
using Godot;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string name = "";
    [Export] public string description = "";
    [Export] public bool stackable = false;
    [Export] public Texture2D texture;
}