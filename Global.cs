using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Runtime.InteropServices.JavaScript;

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
	public Inv inventory;
	public bool dead = false;
	private DeathScreen deathScreen;
	public Array<string> killZones = ["killZone1","killZone2","killZone3", "killZone4", "killZone5"];
	public Dictionary<string, Weapon> weapons = [];


	public bool inDialogue = false;
	public bool inPopup = false;
	private bool animatingDialogue = false;
	public bool fullscreen = false;
	public bool disableTooltips = false;
	public bool disableObjectives = false;
	public bool invOpen = false;
	private AudioStream charSound;

	public Array<PackedScene> enemyArray = [];
	public PackedScene item;


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
	public string CurrentDoorDestinationPath = "res://zones/killZone1.tscn";
	Dictionary<string, Dictionary<string, Variant>> DBItems;
	Dictionary<string, Dictionary<string, Variant>> DBAffixes;
	public PlayerZone playerZone;

	public override void _Ready()
	{
		Gets();
		PreloadObjects();
		DBAffixes = DB.JsonToDict("res://DBAffixes.json");
		DBItems = DB.JsonToDict("res://DBItems.json");
        // Dictionary<string, Variant> conditions = new()
        // {
        // 	{ "type", "suffix" }
        // };
        // GD.Print(DB.SelectFiltered(DBAffixes, conditions));
        SlotData randomItem = new()
        {
            itemData = GenerateItem(),
			Quantity = 1
		
        };
        inventory.inventoryDatas[0].PickUpSlotData(randomItem);
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Pause") && pauseMenu != null && !dead) TogglePause();
		if (inDialogue && Input.IsActionJustPressed("Space")) ProgressDialogue();
		if (Input.IsActionJustPressed("inventory") && inventory != null && !dead) ToggleInv();
	}
		
	public void PreloadObjects()
	{
		enemyArray.Add(GD.Load<PackedScene>("res://enemies/chaser.tscn"));
		enemyArray.Add(GD.Load<PackedScene>("res://enemies/shooter.tscn"));
		item = GD.Load<PackedScene>("res://inventory/ground_item.tscn");
	}

	public ItemEquipmentData GenerateItem()
	{
		// Select random item from database
		string baseItemType = DB.SelectFiltered(DBItems);

		ItemEquipmentData item = new();

		ItemAffix prefix = null;
		if (GD.Randf() >= 0.25f)
		{
			Dictionary<string, Variant> conditions = new()
			{
				{ "type", "prefix" }
			};
			string prefixID = DB.SelectFiltered(DBAffixes, conditions);
			prefix = GenerateAffix(prefixID);
		}

		ItemAffix suffix = null;
		if (GD.Randf() >= 0.25f)
		{
			
			Dictionary<string, Variant> conditions = new()
			{
				{ "type", "suffix" }
			};
			string suffixID = DB.SelectFiltered(DBAffixes, conditions);
			suffix = GenerateAffix(suffixID);
			
		}
		item.name = (string)DBItems[baseItemType]["name"];
		if (prefix != null) item.name = prefix.Name + " " + (string)DBItems[baseItemType]["name"];
		if (suffix != null) item.name += " " + suffix.Name;
		
		item.affixes = [prefix, suffix];
		item.description = (string)DBItems[baseItemType]["description"];
		item.Type = ParseEnum<ItemType>((string)DBItems[baseItemType]["type"]);
		item.texture = GD.Load<Texture2D>("res://textures/227.png");
		return item;
	}

	public ItemAffix GenerateAffix(string id)
	{
		ItemAffix affix = new();
        AttributeModifier modifier = new()
        {
            Value = (int)GD.RandRange((double)DBAffixes[id]["valueMin"], (double)DBAffixes[id]["valueMax"]),
			ModType = ParseEnum<AttributeModType>((string)DBAffixes[id]["modifier"])

        };

		GD.Print(modifier.Value);
		affix.Name = (string)DBAffixes[id]["title"];
		affix.TargetType = ParseEnum<AttributeType>((string)DBAffixes[id]["target"]);
		affix.attributeModifier = modifier;
        return affix;
	}

	public static T ParseEnum<T>(string value)
	{
		return (T) Enum.Parse(typeof(T), value, true);
	}

	private void Gets()
	{
		Viewport root = GetTree().Root;
		currentScene = root.GetChild(root.GetChildCount() - 1);
		currentZone = currentScene is Zone zone ? zone : null;

		if (currentScene is Zone)
		{
			player = currentScene.GetNodeOrNull<Player>("player");
			hud = currentScene.GetNodeOrNull<Hud>("UI/hud");
			inventory = currentScene.GetNodeOrNull<Inv>("UI/inventory");
			deathScreen = currentScene.GetNodeOrNull<DeathScreen>("UI/deathScreen");
			pauseMenu = currentScene.GetNodeOrNull<Pause>("UI/pause");
			
			foreach (Chest n in GetTree().GetNodesInGroup("chests").Cast<Chest>()) { n.ToggleInventory += OnChestInventoryToggle; }
			inventory.DropSlotDataFromInventory += OnDropSlotDataFromInventory;
			inventory.SetPlayerInventoryData(player.inventoryData);
			inventory.SetAttributeLabels(player.attributeData);
			foreach (PlayerAttribute att in player.attributeData.playerAttributes.Values)
			{
				att.AttributesUpdated += () => inventory.OnAttributeDataUpdated(player.attributeData);
			}

			if (playerZone is null && currentScene is PlayerZone z) playerZone = z; // set player zone reference
        }
	}

    private void OnChestInventoryToggle(Chest inventoryOwner) { ToggleInv(inventoryOwner); }
	
	private void OnDropSlotDataFromInventory(SlotData slotData)
	{
		GroundItem groundItem = (GroundItem)item.Instantiate();
		groundItem.slotData = slotData;
		groundItem.Position = player.GetDropPosition();
		GetTree().CurrentScene.CallDeferred("add_child", groundItem);
    }

	public void ToggleInv(Chest externalInventoryOwner = null)
	{
		if (invOpen)
		{
			inventory.Hide();
			Input.MouseMode = Input.MouseModeEnum.Captured;
			invOpen = false;
		}
		else
		{
			inventory.Show();
			Input.MouseMode = Input.MouseModeEnum.Visible;
			invOpen = true;
		}

		if (externalInventoryOwner != null)
		{ inventory.SetExternalInventory(externalInventoryOwner); }
		else
		{ inventory.ClearExternalInventory(); }
	}

	public void TogglePause()
	{
		if (dead) return;
		if (paused)
		{
			if (!inDialogue) player.ResumeInput();
			pauseMenu.Hide();
			Input.MouseMode = Input.MouseModeEnum.Captured;
			Engine.TimeScale = 1;
		}
		else
		{
			player.PauseInput();
			pauseMenu.Show();
			pauseMenu.container.Visible = true;
			Input.MouseMode = Input.MouseModeEnum.Visible;
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
		GD.Print("zone loaded");
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

	public void ShowTooltip(ItemData item)
	{
		hud.itemTooltip.SetItem(item);
		hud.itemTooltip.Show();
    }

    public void CloseTooltip() => hud.itemTooltip.Hide();

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

		foreach (char c in text)
		{
			if (!animatingDialogue) break;
			//PlaySound2D(charSound);
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
		float damage = (float)GD.RandRange(equippedWeapon.damageMin, equippedWeapon.damageMax);
		//crit
		float critValue = equippedWeapon.critChance;
		float critBase = Mathf.Ceil(critValue);
		float critRoll = GD.Randf();
		bool crit = critRoll <= critValue;
		bool critExcess = critRoll <= (critValue - MathF.Truncate(critValue));
		damage *= critExcess ? critBase : 1;
		return Damage.InitDamage(damage, crit, player);
	}

	public void Die()
	{
		hud.reticle.Visible = false;
		player.PauseInput();
		player.handSprite.Visible = false;
		dead = true;
		player.SetCollisionLayerValue(1, false);
		deathScreen.Show();
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
