using Godot;
using System;

public partial class Menu : Control
{
	public override void _Ready()
	{
		MenuButton startButton = GetNode<MenuButton>("VBoxContainer/start");
		//MenuButton settingsButton = GetNode<MenuButton>("VBoxContainer/settings");
		MenuButton quitButton = GetNode<MenuButton>("VBoxContainer/quit");
		startButton.Pressed += OnStartPressed;
		//settingsButton.Pressed += OnSettingsPressed;
		quitButton.Pressed += OnQuitPressed;
		startButton.autoFocussed = true;
		startButton.GrabFocus();
		//if (!Global.Singleton.musicPlayer.Playing) Global.Singleton.PlayMusic();
	}
	
	private void OnStartPressed()
	{
		Global.Singleton.Reset();
		PackedScene zone = GD.Load<PackedScene>("res://zones/startZone.tscn");
		Global.Singleton.GotoScene(zone);
		//Global.Singleton.PauseMusic();
	}
    //private void OnSettingsPressed() => Global.Singleton.GotoScene("res://UI/settings.tscn");
    private void OnQuitPressed() => GetTree().Quit();
}
