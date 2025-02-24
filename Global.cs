using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Global : Node
{

		public Node CurrentScene;
		public int currentLevel = 1;
		public AudioStreamPlayer musicPlayer;
		//public Dialogue dialogue;
		public bool paused = false;
		public Pause pauseMenu;
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
		private Vector3 playerRelativePosition;
		public float playerHealth = 100;
		public float currentPlayerHealth = 100;
		public Hud hud;
		public bool dead = false;
		private DeathScreen deathScreen;
		public string Objective = "";
		public Array<string> EnemyTypes = new() {"shooter", "chaser"};
		public float interactionRange = 3;

		public string equippedWeapon;
		//BUFFS
		public float playerDamageScale = 1f;

		public override void _Ready()
		{
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);

		// gets
		player = CurrentScene.GetNodeOrNull<Player>("player");
		hud = player?.GetNode<Hud>("body/head/Camera3D/CanvasLayer/hud");
		deathScreen = player?.GetNodeOrNull<DeathScreen>("body/head/Camera3D/CanvasLayer/deathScreen");
		pauseMenu = player?.GetNodeOrNull<Pause>("body/head/Camera3D/CanvasLayer/pause");
		//CurrentZone.Populate(currentLevel);
		if (CurrentScene is Zone zone) Objective = zone.objective;
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
        	if (Input.IsActionJustPressed("Pause") && pauseMenu != null && !dead)
			{
				GD.Print("try pause");
				PauseMenu();
			} 
        }

		public void AddBuff(string buff)
		{
			if (buff == "Damage") playerDamageScale += .25f;
		}

		public void IncrementHealth(float value)
		{
			if (dead) return;
			currentPlayerHealth = Mathf.Clamp(currentPlayerHealth + value, 0, currentPlayerHealth);
			UpdateHUD();
			if (currentPlayerHealth <= 0) Die();
		}

		public void Die()
    	{
		player.reticle.Visible = false;
		Engine.TimeScale = 0;
		dead = true;
		deathScreen.Show();
		deathScreen.menuButton.autoFocussed = true;
		deathScreen.menuButton.GrabFocus();
    	}

		public void PauseMenu()
		{
			if (dead) return;
			if (paused)
			{
				pauseMenu.Hide();
				Engine.TimeScale = 1;
			}
			else
			{
				pauseMenu.Show();
				pauseMenu.resumeButton.autoFocussed = true;
				pauseMenu.resumeButton.GrabFocus();
				Engine.TimeScale = 0;
			}
			paused = !paused;
		}

    public void UpdateHUD()
    {
        hud?.SetValues();
    }

    public void GotoScene(PackedScene nextScene) => CallDeferred(MethodName.DeferredGotoScene, nextScene);

		public void Reset()
		{
			currentLevel = 1;
			playerDamageScale = 1;
			Engine.TimeScale = 1;
			dead = false;
			currentPlayerHealth = playerHealth;
			paused = false;
		}

		public void DeferredGotoScene(PackedScene nextScene)
		{
			// It is now safe to remove the current scene.
			CurrentScene.Free();
			CurrentScene = nextScene.Instantiate();
			GetTree().Root.AddChild(CurrentScene);
			GetTree().CurrentScene = CurrentScene;

			player = CurrentScene.GetNodeOrNull<Player>("player");
			hud = player?.GetNode<Hud>("body/head/Camera3D/CanvasLayer/hud");
			deathScreen = player?.GetNodeOrNull<DeathScreen>("body/head/Camera3D/CanvasLayer/deathScreen");
			pauseMenu = player?.GetNodeOrNull<Pause>("body/head/Camera3D/CanvasLayer/pause");
			if (CurrentScene is Zone zone) Objective = zone.objective;
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

}
