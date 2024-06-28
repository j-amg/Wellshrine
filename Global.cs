using Godot;
using System;

public partial class Global : Node
{
		public Node CurrentScene;
		public AudioStreamPlayer musicPlayer;
		//public Dialogue dialogue;
		public bool paused = false;
		//public Pause pauseMenu;
		//private bool enableMusic;
		//private bool enableSound;
		public Player player;
		//public CanvasModulate worldModulate;
		private AudioStream music;
		public static Global Singleton => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<Global>("/root/Global");

	public override void _Ready()
	{
		// stuff :3
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);

		// gets
		//pauseMenu = CurrentScene.GetNodeOrNull<Pause>("camera/CanvasLayer/pause");
		player = CurrentScene.GetNode<Player>("player");

		// audio
		//music = GD.Load<AudioStream>("res://audio/music.ogg");
		//musicPlayer = new();
		//AddChild(musicPlayer);
		//PlayMusic();
		//charSound = GD.Load<AudioStream>("res://audio/bip.wav");

		// canvas
		//worldModulate = new();
		//AddChild(worldModulate);
		//ModulateWorld(new Color(0.25f,0.25f,0.25f,1.0f), 0);

		// dialogue
		//PackedScene scene = GD.Load<PackedScene>("res://dialogue/dialogue.tscn");
		//dialogue = scene.Instantiate<Dialogue>();
		//AddChild(dialogue);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
			//GD.Print(player.Position);
            //if (Input.IsActionJustPressed("pause") && pauseMenu != null) PauseMenu();
        }

		public void GotoScene(string path)
		{
			CallDeferred(MethodName.DeferredGotoScene, path);
		}

		public void DeferredGotoScene(string path)
		{
			
			// It is now safe to remove the current scene.
			CurrentScene.Free();
			// Load a new scene.
			var nextScene = GD.Load<PackedScene>(path);

			// Instance the new scene.
			CurrentScene = nextScene.Instantiate();

			// Add it to the active scene, as child of root.
			GetTree().Root.AddChild(CurrentScene);

			// Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
			
			GetTree().CurrentScene = CurrentScene;
			//pauseMenu = CurrentScene.GetNodeOrNull<Pause>("camera/CanvasLayer/pause");
			player = CurrentScene.GetNodeOrNull<Player>("player");
		}

		public void PlaySound(AudioStream audio)
		{
			AudioStreamPlayer player = new() {Stream = audio};
			player.Finished += () => RemoveAudio(player);
			AddChild(player);
			player.Play();
		}
		public static void RemoveAudio(AudioStreamPlayer player) {player.QueueFree();}

		// public void PauseMenu()
		// {
		// 	if (paused)
		// 	{
		// 		pauseMenu.Hide();
		// 		Engine.TimeScale = 1;
		// 	}
		// 	else
		// 	{
		// 		pauseMenu.Show();
		// 		pauseMenu.resumeButton.autoFocussed = true;
		// 		pauseMenu.resumeButton.GrabFocus();
		// 		Engine.TimeScale = 0;
		// 	}
		// 	paused = !paused;
		// }
}
