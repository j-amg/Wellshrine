using Godot;
using System;

public partial class Menu : Control
{
	[Export]
	public MenuButton version1;
	[Export]
	public MenuButton version2;
	[Export]
	public MenuButton version3;
	[Export]
	public MenuButton version4;
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
		version1.Pressed += OnVersion1Pressed;
		version2.Pressed += OnVersion2Pressed;
		version3.Pressed += OnVersion3Pressed;
		version4.Pressed += OnVersion4Pressed;
		settingsButton.Pressed += OnSettingsPressed;
		quit.Pressed += OnQuitPressed;
		version1.autoFocussed = true;
		version1.GrabFocus();
		Global.Singleton.Reset();
		defaultButton = version1;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    private void OnSettingsPressed()
    {
		title.Visible = false;
		settings.Visible = true;
		container.Visible = false;
		settings.fullscreenButton.autoFocussed = true;
		settings.fullscreenButton.GrabFocus();
    }

    private void OnVersion4Pressed()
    {
		Global.Singleton.disableTooltips = true;
		Global.Singleton.disableObjectives = true;
		PackedScene zone = GD.Load<PackedScene>("res://zones/startZone.tscn");
		Global.Singleton.GotoScene(zone);
    }

    private void OnVersion3Pressed()
    {
		PackedScene zone = GD.Load<PackedScene>("res://zones/version3/v3_1.tscn");
		Global.Singleton.GotoScene(zone);
    }

    private void OnVersion2Pressed()
    {
		PackedScene zone = GD.Load<PackedScene>("res://zones/version2/v2_1.tscn");
		Global.Singleton.GotoScene(zone);
    }

    private void OnVersion1Pressed()
	{
		PackedScene zone = GD.Load<PackedScene>("res://zones/version1/v1_1.tscn");
		Global.Singleton.GotoScene(zone);
	}
    private void OnQuitPressed() => GetTree().Quit();
}
