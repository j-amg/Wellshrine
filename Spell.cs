using Godot;


[GlobalClass]
public partial class Spell : Resource
{ 
    [Export] public string name;
    public void Cast(Player player)
    {
        GD.Print("Cast");
    }
}
