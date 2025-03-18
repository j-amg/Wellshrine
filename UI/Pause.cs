using Godot;
public partial class Pause : Control
{
	[Export] public MenuButton resumeButton;
	[Export] public MenuButton settingsButton;
	[Export] public MenuButton menuButton;
	[Export] public MenuButton quitButton;
	public override void _Ready()
	{
		resumeButton.Pressed += OnResumePressed;
		menuButton.Pressed += OnMenuPressed;
		quitButton.Pressed += OnQuitPressed;
	}

	private void OnResumePressed() => Global.Singleton.PauseMenu();
    private void OnMenuPressed()
    {
		Global.Singleton.PauseMenu();
		PackedScene menu = GD.Load<PackedScene>("res://UI/menu.tscn");
        Global.Singleton.GotoScene(menu);
    }
    private void OnQuitPressed() => GetTree().Quit();
}
