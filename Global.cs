using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Global : Node
{
		[Signal]
		public delegate void HealthChangedEventHandler();
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
		public float basePlayerHealth = 100;
		public float playerHealth = 100;
		public float currentPlayerHealth = 100;
		public Hud hud;
		public bool dead = false;
		private DeathScreen deathScreen;
		public Array<string> EnemyTypes = new() {"shooter", "chaser"};

		public Dictionary<string, StatModifier> statModifiers = new();
		public Dictionary<string, Weapon> weapons = new();


		public bool inDialogue = false;
		public bool inPopup = false;
		private bool animatingDialogue = false;
		private AudioStream charSound;


		private string currentAction;
		public string awaitedAction;
		public Node currentScene;
		public Zone currentZone;
		public int currentLevel;
		private string [] currentDialogue;
		private int currentDialogueStep;
		public string currentIdle = "idle";
		public Weapon equippedWeapon;

		private string[] testText = { "Hello", "this is a test", "of the dialogue",};

		public override void _Ready()
		{
			Gets();
			Reset();

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

		private void Gets()
		{
			Viewport root = GetTree().Root;
        	currentScene = root.GetChild(root.GetChildCount() - 1);
			currentZone = currentScene is Zone ? (Zone)currentScene : null;
			player = currentScene.GetNodeOrNull<Player>("player");
			hud = player?.GetNode<Hud>("body/head/Camera3D/CanvasLayer/hud");
			deathScreen = player?.GetNodeOrNull<DeathScreen>("body/head/Camera3D/CanvasLayer/deathScreen");
			pauseMenu = player?.GetNodeOrNull<Pause>("body/head/Camera3D/CanvasLayer/pause");
			charSound = GD.Load<AudioStream>("res://audio/bip.wav");
		}

		public void Reset()
		{
			InitEquipment();
			currentLevel = 1;
			Engine.TimeScale = 1;
			dead = false;
			currentPlayerHealth = playerHealth;
			paused = false;
		}

		public void InitEquipment()
		{
			weapons.Add("fireball", Weapon.InitWeapon("fireball", 3, 10, 0.25f, .5f, .2f, 5));
			weapons.Add("icespike", Weapon.InitWeapon("icespike", 5, 7, 0.25f, .2f, .1f, 2));
			weapons.Add("shockburst", Weapon.InitWeapon("shockburst", 2, 35, 0.25f, 1f, 1, 10));

			statModifiers.Add("damage", StatModifier.InitModifier("damage", "+25% Damage", "add", .25f, 0, 0));
			statModifiers.Add("critDamage", StatModifier.InitModifier("critDamage", "+25% Critical Damage", "add", .25f, 2, 0));
			statModifiers.Add("moveSpeed", StatModifier.InitModifier("moveSpeed", "+3 Run Speed", "add", 3, 0, 0));
			statModifiers.Add("stun", StatModifier.InitModifier("stun", "+0.15s Stun Duration", "add", .15f, 0, 0));
			statModifiers.Add("health", StatModifier.InitModifier("health", "+50 health", "add", 50, 0, 0));
			statModifiers.Add("recharge", StatModifier.InitModifier("recharge", "-25% recharge", "mult", .75f, 1, 0));
		}

		public float GetPlayerModifier(string modifier)
		{
			StatModifier m = statModifiers[modifier];
			if (m.modifier == "add") return m.baseValue + m.value * m.amount;
			if (m.modifier == "mult") return m.baseValue * (float)Math.Pow(m.value, m.amount);
			return 0;
		}

    	public void AddPlayerModifier(string modifier)
    	{
        	statModifiers[modifier].amount++;
			if (modifier == "health")
			{
				playerHealth = basePlayerHealth + GetPlayerModifier("health");
				EmitSignal(SignalName.HealthChanged);
			}
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


		public void EquipWeapon(string weapon)
		{
			currentIdle = weapon;
			player.handSprite.Play(currentIdle);
			equippedWeapon = weapons[weapon];
		}

		public void IncrementPlayerHealth(float value)
		{
			if (dead) return;
			currentPlayerHealth = Mathf.Clamp(currentPlayerHealth + value, 0, playerHealth);
			EmitSignal(SignalName.HealthChanged);
			if (currentPlayerHealth <= 0) Die();
		}

		public void SetPlayerHealth(float value)
		{
			currentPlayerHealth = Mathf.Clamp(value, 0, playerHealth);
			EmitSignal(SignalName.HealthChanged);
			if (currentPlayerHealth <= 0) Die();
		}
		public Damage GetPlayerDamage()
		{
			//base damge
			float damage = (float)GD.RandRange(equippedWeapon.damageMin + GetPlayerModifier("damage"), equippedWeapon.damageMax + GetPlayerModifier("damage"));
			//crit
			bool crit = GD.Randf() <= equippedWeapon.critChance;
			damage =  crit ? damage * GetPlayerModifier("critDamage") : damage;
			return Damage.InitDamage(damage, crit, player);
		}

		public void Die()
    	{
		hud.reticle.Visible = false;
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

    public void GotoScene(PackedScene nextScene) => CallDeferred(MethodName.DeferredGotoScene, nextScene);
	public void DeferredGotoScene(PackedScene nextScene)
	{
		currentScene.Free();
		currentScene = nextScene.Instantiate();
		currentZone = currentScene is Zone ? (Zone)currentScene : null;
		GetTree().Root.AddChild(currentScene);
		GetTree().CurrentScene = currentScene;
		Gets();
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
