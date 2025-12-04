using System;
using Godot;
public partial class Pause : Control
{
	[Export] public Button resumeButton;
	[Export] public Button settingsButton;
	[Export] public Button menuButton;
	[Export] public Button controlButton;
	[Export] public Button quitButton;
	[Export] public VBoxContainer container;
	private PackedScene menu;
	public override void _Ready()
	{
		resumeButton.Pressed += OnResumePressed;
		settingsButton.Pressed += OnSettingsPressed;
		controlButton.Pressed += OnControlPressed;
		menuButton.Pressed += OnMenuPressed;
		quitButton.Pressed += OnQuitPressed;
		menu = GD.Load<PackedScene>("res://UI/title.tscn");
	}

    private void OnControlPressed()
    {
        // controls.Visible = true;
		// container.Visible = false;
    }

    private void OnSettingsPressed()
    {
		// settings.Visible = true;
		// container.Visible = false;
    }

	private void OnResumePressed() => Global.Singleton.TogglePause();
    private void OnMenuPressed()
    {
		Global.Singleton.TogglePause();
        Global.Singleton.GotoScene(menu);
    }
    private void OnQuitPressed() => GetTree().Quit();
}
