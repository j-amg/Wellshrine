using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Global : Node
{
		[Signal]
		public delegate void HealthChangedEventHandler();
		[Signal]
		public delegate void PopUpClosedEventHandler(string action);
		[Signal]
		public delegate void DialogueFinishedEventHandler();
		[Signal] public delegate void EquippedWeaponEventHandler();

		public AudioStreamPlayer musicPlayer;
		public bool paused = false;
		public Pause pauseMenu;
		public Player player;
		public Camera3D camera;
		private AudioStream music;
		public static Global Singleton => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<Global>("/root/Global");
		public float basePlayerHealth = 100;
		public float playerHealth = 100;
		public float currentPlayerHealth = 100;
		public float interactionRange = 3;
		public Hud hud;
		public bool dead = false;
		private DeathScreen deathScreen;
		public Array<string> EnemyTypes = new() {"shooter", "chaser"};
		public Array<string> killZones = new() {"killZone1","killZone2","killZone3", "killZone4", "killZone5"};
		public Dictionary<string, StatModifier> statModifiers = new();
		public Dictionary<string, Weapon> weapons = new();


		public bool inDialogue = false;
		public bool inPopup = false;
		private bool animatingDialogue = false;
		public bool fullscreen = false;
		public bool disableTooltips = false;
		public bool disableObjectives = false;
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
		public bool sfx = true;

		public string[] testText = { "Hello", "this is a test", "of the dialogue",};

		public override void _Ready()
		{
			charSound = GD.Load<AudioStream>("res://audio/bip.wav");
			Gets();
			Reset();


			// audio
			music = GD.Load<AudioStream>("res://audio/wellshrine.wav");
        	musicPlayer = new() {VolumeDb = Mathf.LinearToDb(.1f)};
        	AddChild(musicPlayer);
		}

		private void Gets()
		{
			Viewport root = GetTree().Root;
        	currentScene = root.GetChild(root.GetChildCount() - 1);
			currentZone = currentScene is Zone zone ? zone : null;
			player = currentScene.GetNodeOrNull<Player>("player");
			hud = player?.GetNode<Hud>("body/head/Camera3D/CanvasLayer/hud");
			deathScreen = player?.GetNodeOrNull<DeathScreen>("body/head/Camera3D/CanvasLayer/deathScreen");
			pauseMenu = player?.GetNodeOrNull<Pause>("body/head/Camera3D/CanvasLayer/pause");
		}

		public void Reset()
		{
			statModifiers.Clear();
			weapons.Clear();
			InitEquipment();
			equippedWeapon = null;
			currentLevel = 1;
			Engine.TimeScale = 1;
			dead = false;
			currentPlayerHealth = playerHealth;
			paused = false;
			currentIdle = "idle";
		}

		public void InitEquipment()
		{
			weapons.Add("fireball", Weapon.InitWeapon("fireball", 3, 10, 0.25f, .5f, .35f, 5, "Launch a ball of flame that explodes in a small area on impact. Moderate damage, and moderate recharge duration."));
			weapons.Add("icespike", Weapon.InitWeapon("icespike", 5, 7, 0.1f, .2f, .2f, 2, "Shoot a fast moving spike of ice that pierces enemies and walls. Low damage, but reduced recharge duration."));
			weapons.Add("shockburst", Weapon.InitWeapon("shockburst", 2, 35, 0.4f, 1f, 1, 10, "Release a burst of lighting in a short-range area. High damage, but high recharge duration."));

			statModifiers.Add("damage", StatModifier.InitModifier("damage", "+5 Damage", "Increases the base damage of a spell.", "add", 5, 0, 0));
			statModifiers.Add("critDamage", StatModifier.InitModifier("critDamage", "+25% Critical Damage", "Multiples the damage of a spell on a Critical Hit. Starts at a 200% multiplier.", "add", .25f, 2, 0));
			statModifiers.Add("moveSpeed", StatModifier.InitModifier("moveSpeed", "+3 Run Speed", "Increases player movement speed", "add", 3, 0, 0));
			statModifiers.Add("stun", StatModifier.InitModifier("stun", "+0.15s Stun Duration", "Increases the duration that an enemy stops moving after being hit by a spell.", "add", .15f, 0, 0));
			statModifiers.Add("health", StatModifier.InitModifier("health", "+50 health", "Increases current and maximum player Health. (The amount of damage that a player can sustain before dying.)", "add", 50, 0, 0));
			statModifiers.Add("recharge", StatModifier.InitModifier("recharge", "-25% recharge", "Reduces the time between when a spell can be recast", "mult", .75f, 1, 0));
			statModifiers.Add("critChance", StatModifier.InitModifier("critChance", "+10% Critical Chance", "Increases the likeliness that a spell hit will critically strike, dealing increased damage. Critical hit chance above 100% increases the damage multiplier.", "add", .1f, 0, 0));
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
				IncrementPlayerHealth(GetPlayerModifier("health"));
				EmitSignal(SignalName.HealthChanged);
			}
    	}
        public override void _Process(double delta)
        {
        	if (Input.IsActionJustPressed("Pause") && pauseMenu != null && !dead) PauseMenu();
			if (inDialogue && Input.IsActionJustPressed("Space")) ProgressDialogue();
        }

		public void SendPopUp(string text, string action)
		{
			hud.popupText.Modulate = new Color(1, 1, 0);
			inPopup = true;
			awaitedAction = action;
			hud.popupText.Text = text;
			hud.popup.Visible = true;
		}

		public void SetAction(string action)
		{
			currentAction = action;
			if (awaitedAction == action && inPopup)ClosePopUp(action, false);
		}

		public async void ClosePopUp(string action, bool overrideWait)
		{
			hud.popupText.Modulate = new Color(0, 1, 0);
			inPopup = false;
			awaitedAction = "";
			if (!overrideWait) await ToSignal(GetTree().CreateTimer(1.5), "timeout");
			hud.popup.Visible = false;
			EmitSignal(SignalName.PopUpClosed, action);
		}

		public void ShowTooltip(string text)
		{
			if (disableTooltips) return;
			hud.tooltip.Visible = true;
			hud.tooltipText.Text = text;
		}

    	public void CloseTooltip() => hud.tooltip.Visible = false;

    	public void EnterDialogue(string[] dialogueText, string name, bool freeze)
		{
			player.PauseInput();
			hud.reticle.Visible = false;
			hud.healthBar.Visible = false;
			hud.healthLabel.Visible = false;
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
			animatingDialogue = true;
			hud.dialogueText.Text = "";
			await ToSignal(GetTree().CreateTimer(.1f), "timeout");
			
			foreach(char c in text)
			{
				if (!animatingDialogue) break;
				PlaySound2D(charSound);
				hud.dialogueText.Text += c;
				float waitTime = c == ',' || c == '.' ? .15f : 0.05f;
				await ToSignal(GetTree().CreateTimer(waitTime), "timeout");
			}
			animatingDialogue = false;
		}

		public void ProgressDialogue()
		{
			if (animatingDialogue)
			{
				animatingDialogue = false;
				hud.dialogueText.Text = currentDialogue[currentDialogueStep];
			} 
			else
			{
				if (currentDialogueStep == currentDialogue.Length - 1)
				{
					CloseDialogue();
					return;
				} 
				currentDialogueStep++;
				AnimateDialogue(currentDialogue[currentDialogueStep]);
			}

		}

		public async void CloseDialogue()
		{
			if (Engine.TimeScale != 1) Engine.TimeScale = 1;
			hud.dialogue.Visible = false;
			inDialogue = false;
			await ToSignal(GetTree().CreateTimer(.5f), "timeout");
			player.ResumeInput();
			hud.reticle.Visible = true;
			hud.healthBar.Visible = true;
			hud.healthLabel.Visible = true;
			EmitSignal(SignalName.DialogueFinished);
		}


		public void EquipWeapon(string weapon)
		{
			EmitSignal(SignalName.EquippedWeapon);
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
			//base damage
			float damage = (float)GD.RandRange(equippedWeapon.damageMin + GetPlayerModifier("damage"), equippedWeapon.damageMax + GetPlayerModifier("damage"));
			//crit
			float critValue = equippedWeapon.critChance + GetPlayerModifier("critChance");
			float critBase = Mathf.Ceil(critValue);
			float critRoll = GD.Randf();
			bool crit = critRoll <= critValue;
			bool critExcess = critRoll <= (critValue - MathF.Truncate(critValue));
			damage *= critExcess ? (GetPlayerModifier("critDamage") * critBase) : 1;
			return Damage.InitDamage(damage, crit, player);
		}

		public void Die()
    	{
			hud.reticle.Visible = false;
			player.PauseInput();
			player.handSprite.Visible = false;
			dead = true;
			player.SetCollisionLayerValue(1,false);
			deathScreen.Show();
			deathScreen.menuButton.autoFocussed = true;
			deathScreen.menuButton.GrabFocus();
    	}

		public void PauseMenu()
		{
			if (dead) return;
			if (paused)
			{
				if (!inDialogue) player.ResumeInput();
				pauseMenu.Hide();
				pauseMenu.settings.Visible = false;
				Engine.TimeScale = 1;
			}
			else
			{
				player.PauseInput();
				pauseMenu.Show();
				pauseMenu.controls.Visible = false;
				pauseMenu.container.Visible = true;
				pauseMenu.resumeButton.autoFocussed = true;
				pauseMenu.resumeButton.GrabFocus();
				Engine.TimeScale = 0;
			}
			paused = !paused;
		}

    public void GotoScene(PackedScene nextScene) => CallDeferred(MethodName.DeferredGotoScene, nextScene);
	public void DeferredGotoScene(PackedScene nextScene)
	{
		currentZone?.CloseZone();
		currentScene.Free();
		currentScene = nextScene.Instantiate();
		currentZone = currentScene is Zone zone ? zone : null;
		GetTree().Root.AddChild(currentScene);
		GetTree().CurrentScene = currentScene;
		Gets();
		
	}

	public void PlaySound3D(Vector3 position, AudioStream audio)
	{
		if (!sfx) return;
		AudioStreamPlayer3D p = new() {Stream = audio};
		p.VolumeDb = Mathf.LinearToDb(0.25f);
		p.Finished += () => RemoveAudio3D(p);
		AddChild(p);
		p.Position = position;
		p.Play();
	}
	public static void RemoveAudio3D(AudioStreamPlayer3D player) {player.QueueFree();}

	public void PlaySound2D(AudioStream audio)
	{
		if (!sfx) return;
		AudioStreamPlayer2D player = new() {Stream = audio};
		player.Finished += () => RemoveAudio2D(player);
		AddChild(player);
		player.Play();
	}
	public static void RemoveAudio2D(AudioStreamPlayer2D player) {player.QueueFree();}

	public void PlayMusic()
		{
			musicPlayer.Stream = music;
			musicPlayer.Play();
		}

    public void PauseMusic() => musicPlayer.StreamPaused = true;

    public void ResumeMusic() => musicPlayer.StreamPaused = false;

}
