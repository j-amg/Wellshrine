using Godot;
using System;

public partial class ChatLog : PanelContainer
{

	[Export] public LineEdit lineEdit;
	[Export] public VBoxContainer messageContainer;
	private PackedScene messageScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		messageScene = GD.Load<PackedScene>("res://UI/chat_message.tscn");
		lineEdit.TextSubmitted += OnTextSubmitted;
	}

	private void OnTextSubmitted(string newText)
	{
		SendMessage("Player Name", newText);
		lineEdit.Text = "";
	}

	public void SendMessage(string source, string message)
	{
		var messageInst = messageScene.Instantiate();
		messageInst.GetNode<Label>("VBoxContainer/source").Text = source + ": ";
		messageInst.GetNode<Label>("VBoxContainer/message").Text = message;
		messageContainer.AddChild(messageInst);
	}
}
