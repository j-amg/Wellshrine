using Godot;
using System;

public partial class Global : Node
{

		public Zone CurrentZone;
		public int currentLevel = 1;
		public AudioStreamPlayer musicPlayer;
		//public Dialogue dialogue;
		public bool paused = false;
		//public Pause pauseMenu;
		//private bool enableMusic;
		//private bool enableSound;
		public Player player;
		public Camera3D camera;
		//public CanvasModulate worldModulate;
		private AudioStream music;
		public static Global Singleton => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<Global>("/root/Global");
		private int playerGold;
		private float playerAttackSpeed = 1;
		private float playerMoveSpeed = 1;
		private float playerDamageMult = 1;
		private Vector3 playerRelativePosition;
		public float playerHealth = 100;
		public float currentPlayerHealth = 100;
		public Hud hud;
		public override void _Ready()
		{
        Viewport root = GetTree().Root;
        CurrentZone = (Zone)root.GetChild(root.GetChildCount() - 1);

		// gets
		//pauseMenu = CurrentScene.GetNodeOrNull<Pause>("camera/CanvasLayer/pause");
		player = CurrentZone.GetNodeOrNull<Player>("player");
		hud = player.GetNode<Hud>("body/head/Camera3D/hud");
		//CurrentZone.Populate(currentLevel);
		CurrentZone.UpdateObjective();
		UpdateHUD();

		//camera = CurrentScene.GetNode<Player>("player").GetNode<Node3D>("head").GetNode<Camera3D>("Camera3D");

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
            //if (Input.IsActionJustPressed("pause") && pauseMenu != null) PauseMenu();

        }

		public void UpdateHUD()
		{
			hud.SetValues();
		}

		public void GotoZone(PackedScene nextZone, Door door)
		{
			playerRelativePosition = player.GlobalTransform.Origin - door.GlobalTransform.Origin;
			CallDeferred(MethodName.DeferredGotoZone, nextZone);
		}

		public void DeferredGotoZone(PackedScene nextZone)
		{
			// It is now safe to remove the current scene.
			CurrentZone.Free();

			// Instance the new scene.
			CurrentZone = Zone.Initialise(nextZone);

			// Add it to the active scene, as child of root.
			GetTree().Root.AddChild(CurrentZone);

			// Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
			GetTree().CurrentScene = CurrentZone;

			//pauseMenu = CurrentScene.GetNodeOrNull<Pause>("camera/CanvasLayer/pause");
			player = CurrentZone.GetNodeOrNull<Player>("player");
			hud = player.GetNode<Hud>("body/head/Camera3D/hud");
			player.GlobalPosition = playerRelativePosition;

			// spawn enemies at current level
			CurrentZone.Populate(currentLevel);
			UpdateHUD();
		}

		public void PlaySound3D(Vector3 position, AudioStream audio)
		{
			AudioStreamPlayer3D player = new() {Stream = audio};
			player.Finished += () => RemoveAudio3D(player);
			AddChild(player);
			player.Position = position;
			player.Play();
		}
		public static void RemoveAudio3D(AudioStreamPlayer3D player) {player.QueueFree();}

		public void PlaySound2D(AudioStream audio)
		{
			AudioStreamPlayer2D player = new() {Stream = audio};
			player.Finished += () => RemoveAudio2D(player);
			AddChild(player);
			player.Play();
		}
		public static void RemoveAudio2D(AudioStreamPlayer2D player) {player.QueueFree();}

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
