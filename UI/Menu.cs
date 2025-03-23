using Godot;
using System;

public partial class Menu : Control
{
	[Export]
	MenuButton version1;
	[Export]
	MenuButton version2;
	[Export]
	MenuButton version3;
	[Export]
	MenuButton version4;
	[Export]
	MenuButton quit;
	public override void _Ready()
	{
		version1.Pressed += OnVersion1Pressed;
		version2.Pressed += OnVersion2Pressed;
		version3.Pressed += OnVersion3Pressed;
		version4.Pressed += OnVersion4Pressed;
		quit.Pressed += OnQuitPressed;
		version1.autoFocussed = true;
		version1.GrabFocus();
		Global.Singleton.Reset();
		//Global.Singleton.PlayMusic();
	}

    private void OnVersion4Pressed()
    {
		PackedScene zone = GD.Load<PackedScene>("res://zones/startZone.tscn");
		Global.Singleton.GotoScene(zone);
		//Global.Singleton.PauseMusic();
    }

    private void OnVersion3Pressed()
    {
		PackedScene zone = GD.Load<PackedScene>("res://zones/startZone.tscn");
		Global.Singleton.GotoScene(zone);
		//Global.Singleton.PauseMusic();
    }

    private void OnVersion2Pressed()
    {
		PackedScene zone = GD.Load<PackedScene>("res://zones/startZone.tscn");
		Global.Singleton.GotoScene(zone);
		//Global.Singleton.PauseMusic();
    }

    private void OnVersion1Pressed()
	{
		PackedScene zone = GD.Load<PackedScene>("res://zones/version1/v1_1.tscn");
		Global.Singleton.GotoScene(zone);
		//Global.Singleton.PauseMusic();
	}

    //private void OnSettingsPressed() => Global.Singleton.GotoScene("res://UI/settings.tscn");
    private void OnQuitPressed() => GetTree().Quit();
}
