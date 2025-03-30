using Godot;
public partial class Controls : Control
{
	[Export] public MenuButton backButton;
	public dynamic owner;
	public override void _Ready()
	{
		owner = Owner;
		backButton.Pressed += OnBackPressed;
	}
    private void OnBackPressed()
    {	
		if (owner.GetNodeOrNull<Label>("title") is not null) owner.title.Visible = true;
        owner.container.Visible = true;
		Visible = false;
		owner.controlButton.autoFocussed = true;
		owner.controlButton.GrabFocus();
    }
}
