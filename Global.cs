using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Global : Node
{
		[Signal]
		public delegate void ZoneEnteredEventHandler();
		public AudioStreamPlayer musicPlayer;
		public bool paused = false;
		public Pause pauseMenu;
		//private bool enableMusic;
		//private bool enableSound;
		public Player player;
		public Camera3D camera;
		//public CanvasModulate worldModulate;
		private AudioStream music;
		public static Global Singleton => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<Global>("/root/Global");
		public float playerHealth = 100;
		public float currentPlayerHealth = 100;
		public Hud hud;
		public bool dead = false;
		private DeathScreen deathScreen;
		public string Objective = "";
		public Array<string> EnemyTypes = new() {"shooter", "chaser"};

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


		public Dictionary<string, Weapon> weapons = new();


		public bool inDialogue = false;
		public bool inPopup = false;
		private bool animatingDialogue = false;
		private AudioStream charSound;


		private string currentAction;
		public string awaitedAction;
		public Node CurrentScene;
		public int currentLevel;
		private string [] currentDialogue;
		private int currentDialogueStep;
		public string currentIdle = "idle";
		public Weapon equippedWeapon;

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
			
			if (CurrentScene is Zone zone) Objective = zone.objective;
		}
        public override void _Process(double delta)
        {
        	if (Input.IsActionJustPressed("Pause") && pauseMenu != null && !dead) PauseMenu();
			if (inDialogue && Input.IsActionJustPressed("Space")) ProgressDialogue();
			if (Input.IsActionJustPressed("toggle") && !inDialogue) EnterDialogue(testText, "blah", false);
			if (Input.IsActionJustPressed("popup") && !inPopup) SendPopUp("Jump while crouching to dash", "dash");
        }

		public void SendPopUp(string text, string action)
		{
			inPopup = true;
			awaitedAction = action;
			hud.popupText.Text = text;
			hud.popup.Visible = true;
		}

		public void SetAction(string action)
		{
			GD.Print(currentAction);
			currentAction = action;
			if (awaitedAction == action && hud?.popup.Visible == true) ClosePopUp();
		}

		public async void ClosePopUp()
		{
			await ToSignal(GetTree().CreateTimer(1), "timeout");
			awaitedAction = "";
			hud.popup.Visible = false;
			inPopup = false;
		}

		public void EnterDialogue(string[] dialogueText, string name, bool freeze)
		{
			player.PauseInput();
			hud.reticle.Visible = false;
			if (freeze) Engine.TimeScale = 0;
			inDialogue = true;
			currentDialogue = dialogueText;
			currentDialogueStep = 0;
			hud.dialogueName.Text = name;
			AnimateDialogue(dialogueText[0]);
			hud.dialogue.Visible = true;
			
		}

		private async void AnimateDialogue(string text)
		{
			float appearSpeed = 0.05f;
			hud.dialogueText.Text = "";
			await ToSignal(GetTree().CreateTimer(.1f), "timeout");
			animatingDialogue = true;
			foreach(char c in text)
			{
				if (!animatingDialogue) break;
				PlaySound2D(charSound);
				hud.dialogueText.Text += c;
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
		}

		public async void CloseDialogue()
		{
			if (Engine.TimeScale != 1) Engine.TimeScale = 1;
			hud.dialogue.Visible = false;
			inDialogue = false;
			await ToSignal(GetTree().CreateTimer(.5f), "timeout");
			player.ResumeInput();
			hud.reticle.Visible = true;
		}

		public void AddBuff(string buff)
		{

			if (buff == "Increased Damage") playerDamageBuff += 2;
			if (buff == "Increased Critical Damage") playerCritDamageBuff +=.25f;
			if (buff == "Increased Move Speed") playerMoveSpeedBuff += 3;
			if (buff == "Increased Stun Duration") playerStunDurationBuff +=.1f;
			if (buff == "Increased Max Health")
			{
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
		hud.reticle.Visible = false;
		//Engine.TimeScale = 0;
		player.PauseInput();
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

    public void UpdateHUD()
    {
		hud.zoneLabel.Text = "Zone: " + currentLevel.ToString();
        hud.objectiveLabel.Text = "Objective: " + Objective.ToString();
    }

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
