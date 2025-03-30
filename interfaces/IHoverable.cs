using Godot;

public partial interface IHoverable
{
    public Color ReticleModulate {get; set;}
    public bool Active {get; set;}
    public bool Tooltip {get; set;}
    public string TooltipText {get; set;}
    public float HoverRange {get; set;}
    public void StartHover();
    public void EndHover();
}