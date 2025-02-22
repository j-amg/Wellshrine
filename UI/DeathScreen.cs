using Godot;
using System;

public partial class DeathScreen : Control
{
	public MenuButton menuButton;
	public override void _Ready()
	{
		//respawnButton = GetNode<MenuButton>("VBoxContainer/respawn");
		//respawnButton.Pressed += OnResumePressed;
		menuButton = GetNode<MenuButton>("VBoxContainer/menu");
		menuButton.Pressed += OnMenuPressed;
		MenuButton quitButton = GetNode<MenuButton>("VBoxContainer/quit");
		quitButton.Pressed += OnQuitPressed;
	}
    //private void OnResumePressed() => Global.Singleton.Respawn();
    private void OnMenuPressed()
    {
		PackedScene menu = GD.Load<PackedScene>("res://UI/menu.tscn");
        Global.Singleton.GotoScene(menu);
		Engine.TimeScale = 1;
    }

    private void OnQuitPressed() => GetTree().Quit();
}
