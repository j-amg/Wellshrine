using Godot;
using System;

public partial class Menu : Control
{
	[Export]
	public MenuButton start;
	[Export]
	public MenuButton settingsButton;
	[Export]
	public MenuButton quit;
	[Export]
	public Settings settings;
	[Export]
	public VBoxContainer container;
	[Export]
	public Label title;
	public MenuButton defaultButton;
	public override void _Ready()
	{
		start.Pressed += OnStartPressed;
		settingsButton.Pressed += OnSettingsPressed;
		quit.Pressed += OnQuitPressed;
		start.autoFocussed = true;
		start.GrabFocus();
		Global.Singleton.Reset();
		defaultButton = start;
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

    private void OnSettingsPressed()
    {
		title.Visible = false;
		settings.Visible = true;
		container.Visible = false;
		settings.fullscreenButton.autoFocussed = true;
		settings.fullscreenButton.GrabFocus();
    }

    private void OnStartPressed()
	{
		PackedScene zone = GD.Load<PackedScene>("res://zones/startzone.tscn");
		Global.Singleton.GotoScene(zone);
	}
    private void OnQuitPressed() => GetTree().Quit();
}
