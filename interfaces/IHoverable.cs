using Godot;

public partial interface IHoverable
{
    public Color ReticleModulate {get; set;}
    public bool Active {get; set;}
    public void StartHover();
    public void EndHover();
}