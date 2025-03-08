using System;
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
		public float playerHealth = 100;
		public float currentPlayerHealth = 100;
		public Hud hud;
		public VBoxContainer dialogue;
		public bool dead = false;
		private DeathScreen deathScreen;
		public string Objective = "";
		public Array<string> EnemyTypes = new() {"shooter", "chaser"};
		public float interactionRange = 3;

		public Weapon equippedWeapon;
		//BUFFS

		public const int basePlayerDamageBuff = 0;
		public const float basePlayerCritDamageBuff = 2f;
		public const float basePlayerMoveSpeedBuff = 0f;
		public const float basePlayerStunDurationBuff = 0f;
		public const int basePlayerMaxHealthBuff = 0;
		public const float basePlayerRechargeBuff = 1;

		public int playerDamageBuff;
		public float playerCritDamageBuff;
		public float playerMoveSpeedBuff;
		public float playerStunDurationBuff;
		public int playerMaxHealthBuff;
		public float playerRechargeBuff;

		public string currentIdle = "idle";

		public Dictionary<string, Weapon> weapons = new();

		private string [] currentDialogue;
		private int currentDialogueStep;
		private Label dialogueText;
		public bool inDialogue = false;
		private bool animatingDialogue = false;
		private AudioStream charSound;

		private string[] testText = { "Hello", "this is a test", "of the dialogue",};


		public override void _Ready()
		{
        	Viewport root = GetTree().Root;
        	CurrentScene = root.GetChild(root.GetChildCount() - 1);
			charSound = GD.Load<AudioStream>("res://audio/bip.wav");
			Gets();

			// create weapons
			weapons.Add("fireball", Weapon.InitWeapon("fireball", 3, 10, .5f, .2f, 5));
			weapons.Add("icespike", Weapon.InitWeapon("icespike", 5, 7, .2f, .1f, 2));
			weapons.Add("shockburst", Weapon.InitWeapon("shockburst", 2, 35, 1f, 1, 10));

			Reset();
			UpdateHUD();

		// audio
		//music = GD.Load<AudioStream>("res://audio/music.ogg");
		//musicPlayer = new();
		//AddChild(musicPlayer);
		//PlayMusic();

		// canvas
		//worldModulate = new();
		//AddChild(worldModulate);
		//ModulateWorld(new Color(0.25f,0.25f,0.25f,1.0f), 0);
		}

		public void Reset()
		{
			playerDamageBuff = basePlayerDamageBuff;
			playerCritDamageBuff = basePlayerCritDamageBuff;
			playerMoveSpeedBuff = basePlayerMoveSpeedBuff;
			playerStunDurationBuff = basePlayerStunDurationBuff;
			playerMaxHealthBuff = basePlayerMaxHealthBuff;
			playerRechargeBuff = basePlayerRechargeBuff;
			equippedWeapon = null;

			currentLevel = 1;
			playerDamageBuff = 1;
			Engine.TimeScale = 1;
			dead = false;
			currentPlayerHealth = playerHealth;
			paused = false;
		}

		private void Gets()
		{
			player = CurrentScene.GetNodeOrNull<Player>("player");
			hud = player?.GetNode<Hud>("body/head/Camera3D/CanvasLayer/hud");
			deathScreen = player?.GetNodeOrNull<DeathScreen>("body/head/Camera3D/CanvasLayer/deathScreen");
			pauseMenu = player?.GetNodeOrNull<Pause>("body/head/Camera3D/CanvasLayer/pause");
			dialogue = hud?.GetNode<VBoxContainer>("dialogue");
			dialogueText = dialogue?.GetNode<Label>("text");
			if (CurrentScene is Zone zone) Objective = zone.objective;
		}

		public void EnterDialogue(string[] dialogueText, string name, bool freeze)
		{
			player.PauseInput();
			if (freeze) Engine.TimeScale = 0;
			inDialogue = true;
			currentDialogue = dialogueText;
			currentDialogueStep = 0;
			dialogue.GetNode<Label>("name").Text = name;
			AnimateDialogue(dialogueText[0]);
			dialogue.Visible = true;
			
		}

		private async void AnimateDialogue(string text)
		{
			float appearSpeed = 0.05f;
			dialogueText.Text = "";
			await ToSignal(GetTree().CreateTimer(.1f), "timeout");
			animatingDialogue = true;
			foreach(char c in text)
			{
				if (!animatingDialogue) break;
				PlaySound2D(charSound);
				dialogueText.Text += c;
				await ToSignal(GetTree().CreateTimer(appearSpeed), "timeout");
			}
		}

		public void ProgressDialogue()
		{
			if (currentDialogueStep == currentDialogue.Length - 1)
			{
				CloseDialogue();
				return;
			} 
			animatingDialogue = false;
			currentDialogueStep++;
			AnimateDialogue(currentDialogue[currentDialogueStep]);

			GD.Print("progress dialogue");
		}

		public async void CloseDialogue()
		{
			GD.Print("close dialogue");
			if (Engine.TimeScale != 1) Engine.TimeScale = 1;
			dialogue.Visible = false;
			inDialogue = false;
			await ToSignal(GetTree().CreateTimer(.5f), "timeout");
			player.ResumeInput();
			
		}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        	if (Input.IsActionJustPressed("Pause") && pauseMenu != null && !dead) PauseMenu();
			if (inDialogue && Input.IsActionJustPressed("Space")) ProgressDialogue();
			if (Input.IsActionJustPressed("toggle") && !inDialogue) EnterDialogue(testText, "blah", false);
        }

		public void AddBuff(string buff)
		{

			if (buff == "Increased Damage") playerDamageBuff += 2;
			if (buff == "Increased Critical Damage") playerCritDamageBuff +=.25f;
			if (buff == "Increased Move Speed") playerMoveSpeedBuff += 3;
			if (buff == "Increased Stun Duration") playerStunDurationBuff +=.1f;
			if (buff == "Increased Max Health")
			{
				//GD.Print("increase health");
				playerHealth += 50;
				UpdateHUD();
			} 
			if (buff == "Reduced Rechare") playerRechargeBuff *= .75f;
		}

		public void EquipWeapon(string weapon)
		{
			currentIdle = weapon;
			player.handSprite.Play(currentIdle);
			equippedWeapon = weapons[weapon];
		}

		public void IncrementHealth(float value)
		{
			if (dead) return;
			currentPlayerHealth = Mathf.Clamp(currentPlayerHealth + value, 0, currentPlayerHealth);
			UpdateHUD();
			if (currentPlayerHealth <= 0) Die();
		}

		public float GetDamage()
		{
			int baseDamage = GD.RandRange(equippedWeapon.damageMin + playerDamageBuff, equippedWeapon.damageMax + playerDamageBuff);
			return GD.Randf() <= .25f ? baseDamage * playerCritDamageBuff : baseDamage;
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
				player.ResumeInput();
				pauseMenu.Hide();
				Engine.TimeScale = 1;
			}
			else
			{
				player.PauseInput();
				pauseMenu.Show();
				pauseMenu.resumeButton.autoFocussed = true;
				pauseMenu.resumeButton.GrabFocus();
				Engine.TimeScale = 0;
			}
			paused = !paused;
		}

    public void UpdateHUD() => hud?.SetValues();

    public void GotoScene(PackedScene nextScene) => CallDeferred(MethodName.DeferredGotoScene, nextScene);
	public void DeferredGotoScene(PackedScene nextScene)
	{
		CurrentScene.Free();
		CurrentScene = nextScene.Instantiate();
		GetTree().Root.AddChild(CurrentScene);
		GetTree().CurrentScene = CurrentScene;
		Gets();
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
