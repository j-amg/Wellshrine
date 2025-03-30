using System;
using Godot;
public partial class Pause : Control
{
	[Export] public MenuButton resumeButton;
	[Export] public MenuButton settingsButton;
	[Export] public MenuButton menuButton;
	[Export] public MenuButton controlButton;
	[Export] public MenuButton quitButton;
	[Export] public Settings settings;
	[Export] public Controls controls;
	[Export] public VBoxContainer container;
	public MenuButton defaultButton;
	public override void _Ready()
	{
		defaultButton = resumeButton;
		resumeButton.Pressed += OnResumePressed;
		settingsButton.Pressed += OnSettingsPressed;
		controlButton.Pressed += OnControlPressed;
		menuButton.Pressed += OnMenuPressed;
		quitButton.Pressed += OnQuitPressed;
	}

    private void OnControlPressed()
    {
        controls.Visible = true;
		container.Visible = false;
		controls.backButton.autoFocussed = true;
		controls.backButton.GrabFocus();
    }

    private void OnSettingsPressed()
    {
		settings.Visible = true;
		container.Visible = false;
		settings.fullscreenButton.autoFocussed = true;
		settings.fullscreenButton.GrabFocus();
    }

	private void OnResumePressed() => Global.Singleton.PauseMenu();
    private void OnMenuPressed()
    {
		Global.Singleton.PauseMenu();
		PackedScene menu = GD.Load<PackedScene>("res://UI/title.tscn");
        Global.Singleton.GotoScene(menu);
    }
    private void OnQuitPressed() => GetTree().Quit();
}
