using Godot;
using System;

public partial class Menu : Control
{
	[Export]
	public Button start;
	[Export]
	public Button quit;

	private PackedScene startZone;

	public override void _Ready()
	{
		start.Pressed += OnStartPressed;
		quit.Pressed += OnQuitPressed;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		startZone = GD.Load<PackedScene>("res://zones/startZone.tscn");
	}

    private void OnStartPressed()
	{
		Global.Singleton.GotoScene(startZone);
	}
    private void OnQuitPressed() => GetTree().Quit();
}
