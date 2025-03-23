using Godot;

public partial interface IHoverable
{
    public Color ReticleModulate {get; set;}
    public bool Active {get; set;}
    public bool PopUp {get; set;}
    public string PopUpText {get; set;}
    public float HoverRange {get; set;}
    public void StartHover();
    public void EndHover();
}