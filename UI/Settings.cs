using Godot;
public partial class Settings : Control
{
	[Export] public MenuButton fullscreenButton;
	[Export] public MenuButton volumeButton;
	[Export] public MenuButton backButton;
	[Export] public VBoxContainer container;
	public dynamic owner;
	public override void _Ready()
	{
		owner = Owner;
		fullscreenButton.Pressed += OnFullscreenPressed;
		volumeButton.Pressed += OnVolumePressed;
		backButton.Pressed += OnBackPressed;
		fullscreenButton.label.Text = Global.Singleton.fullscreen ? "Set Display Mode: Windowed" : "Set Display Mode: Fullscreen";
		volumeButton.label.Text = Global.Singleton.sfx ? "Disable Sound effects" : "Enable Audio Effects";
	}


    private void OnFullscreenPressed()
    {
		if (Global.Singleton.fullscreen)
		{
			Global.Singleton.fullscreen = false;
			fullscreenButton.label.Text = "Set Display Mode: Fullscreen";
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		} else 
		{
			Global.Singleton.fullscreen = true;
			fullscreenButton.label.Text = "Set Display Mode: Windowed";
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
    }

    private void OnVolumePressed()
    {
		if (Global.Singleton.sfx)
		{
			Global.Singleton.sfx = false;
			volumeButton.label.Text = "Enable Audio Effects";
		} else 
		{
			Global.Singleton.sfx = true;
			volumeButton.label.Text = "Disable Sound effects";
		}
    }
    private void OnBackPressed()
    {	
		if (owner.GetNodeOrNull<Label>("title") is not null) owner.title.Visible = true;

        owner.container.Visible = true;
		Visible = false;
		owner.settingsButton.autoFocussed = true;
		owner.settingsButton.GrabFocus();
    }
}
