using Godot;
public partial class Pause : Control
{
	public MenuButton resumeButton;
	public override void _Ready()
	{
		resumeButton = GetNode<MenuButton>("VBoxContainer/resume");
		resumeButton.Pressed += OnResumePressed;
		MenuButton menuButton = GetNode<MenuButton>("VBoxContainer/menu");
		menuButton.Pressed += OnMenuPressed;
		MenuButton quitButton = GetNode<MenuButton>("VBoxContainer/quit");
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
