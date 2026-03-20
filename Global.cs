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


	public bool inDialogue = false;
	public bool inPopup = false;
	private bool animatingDialogue = false;
	public bool fullscreen = false;
	public bool disableTooltips = false;
	public bool disableObjectives = false;
	public bool invOpen = false;
	private AudioStream charSound;

	public Array<PackedScene> enemyArray = [];
	public Array<PackedScene> tileArray = [];
	public PackedScene item;

	private string currentAction;
	public string awaitedAction;
	public Node currentScene;
	public Zone currentZone;
	public int currentLevel;
	private string [] currentDialogue;
	private int currentDialogueStep;
	public string currentIdle = "idle";
	public bool sfx = true;
	Dictionary<string, Dictionary<string, Variant>> DBItems;
	Dictionary<string, Dictionary<string, Variant>> DBAffixes;
	Dictionary<string, Dictionary<string, Variant>> DBMaps;
	public PlayerZone playerZone;
	public Zone doorZone;

	public override void _Ready()
	{
		Gets();
		ConnectSignals();
		PreloadObjects();
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Pause") && pauseMenu != null && !dead) TogglePause();
		if (inDialogue && Input.IsActionJustPressed("Space")) ProgressDialogue();
		if (Input.IsActionJustPressed("inventory") && inventory != null && !dead) ToggleInv();

		if (Input.IsActionJustPressed("p"))
		{
			GD.Print("p pressed");
			if (playerZone != null)
			{
				//player.GlobalTransform.Origin = new(0,0,0);
				GotoZone(playerZone, player.Transform);
				
				//GD.Print("found play zone");
			}  
			else
			{
				GotoScene(GD.Load<PackedScene>("res://zones/startZone.tscn"));
				//GD.Print("loaded new player zone");
			} 
		} 
	}

	public void GotoScene(PackedScene nextScene) => CallDeferred(MethodName.DeferredGotoScene, nextScene);

	public void testFunction()
	{
		GD.Print("test function");
	}
	public void DeferredGotoScene(PackedScene nextScene)
	{
		currentZone?.CloseZone();
		currentScene.Free();
		currentScene = nextScene.Instantiate();
		currentZone = currentScene is Zone zone ? zone : null;
		GetTree().Root.AddChild(currentScene);
		GetTree().CurrentScene = currentScene;
		Gets();
		//GD.Print("zone loaded");
	}

	public void GotoZone(Zone zone, Transform3D respawnTransform)
	{
		GD.Print("travelling to: " + zone.Name);
		//GD.Print(zone.GetNodeOrNull<Player>("player").Position);
		
		
		CallDeferred(MethodName.DeferredGotoZone, zone, respawnTransform);
	}

	public void DeferredGotoZone(Zone zone, Transform3D respawnTransform)
	{
		bool isInstance = false;
		if (zone is null) return;
		if (currentZone is not PlayerZone && currentZone != doorZone)
		{
			//currentZone?.CloseZone();
			currentScene.Free();
		} else
		{
			isInstance = true;
			GetTree().Root.RemoveChild(currentScene);
			currentZone.ResetPlayer(respawnTransform);
		}
		currentScene = zone;
		currentZone = currentScene is Zone z ? z : null;
		GetTree().Root.AddChild(currentScene);
		GetTree().CurrentScene = currentScene;
		Gets();
		if (!isInstance) ConnectSignals(); // prevents connecting to same thing twice

	}

		
	public void PreloadObjects()
	{
		DBAffixes = DB.JsonToDict("res://DBAffixes.json");
		DBItems = DB.JsonToDict("res://DBItems.json");
		DBMaps = DB.JsonToDict("res://DBMaps.json");
		enemyArray.Add(GD.Load<PackedScene>("res://enemies/chaser.tscn"));
		enemyArray.Add(GD.Load<PackedScene>("res://enemies/shooter.tscn"));

		tileArray.Add(GD.Load<PackedScene>("res://zones/zoneDoor.tscn"));
		tileArray.Add(GD.Load<PackedScene>("res://zones/kztest2.tscn"));
		tileArray.Add(GD.Load<PackedScene>("res://zones/kztest.tscn"));
		tileArray.Add(GD.Load<PackedScene>("res://zones/kztest3.tscn"));

		item = GD.Load<PackedScene>("res://inventory/ground_item.tscn");
	}

	public void Gets()
	{
		Viewport root = GetTree().Root;
		currentScene = root.GetChild(root.GetChildCount() - 1);
		currentZone = currentScene is Zone zone ? zone : null;


		if (currentScene is Zone z1)
		{
			player = z1.player;
			if (player is null) GD.Print("player is null");
			hud = currentScene.GetNodeOrNull<Hud>("UI/hud");
			//GD.Print(hud);
			inventory = currentScene.GetNodeOrNull<Inv>("UI/inventory");
			if (inventory is null) GD.Print("inventory is null");
			deathScreen = currentScene.GetNodeOrNull<DeathScreen>("UI/deathScreen");
			pauseMenu = currentScene.GetNodeOrNull<Pause>("UI/pause");
			inventory.SetPlayerInventoryData(player.inventoryData);
			inventory.SetAttributeLabels(player.attributeData);

			 // set player zone reference
			if (currentScene is PlayerZone z)
			{
				playerZone = z;
			}
		}
	}

	public void ConnectSignals()
	{
		if (currentScene is Zone z1)
		{
			GD.Print("connected");
			foreach (Chest n in GetTree().GetNodesInGroup("chests").Cast<Chest>()) 
			{
				n.ToggleInventory += OnChestInventoryToggle;
			}
			inventory.DropSlotDataFromInventory += OnDropSlotDataFromInventory;
			foreach (PlayerAttribute att in player.attributeData.playerAttributes.Values)
			{
				att.AttributesUpdated += () => inventory.OnAttributeDataUpdated(player.attributeData);
			}
		}
	}

	// def bounding_box(points):
	// """returns a list containing the bottom left and the top right 
	// points in the sequence
	// Here, we traverse the collection of points only once, 
	// to find the min and max for x and y
	// """
	// bot_left_x, bot_left_y = float('inf'), float('inf')
	// top_right_x, top_right_y = float('-inf'), float('-inf')
	// for x, y in points:
	//     bot_left_x = min(bot_left_x, x)
	//     bot_left_y = min(bot_left_y, y)
	//     top_right_x = max(top_right_x, x)
	//     top_right_y = max(top_right_y, y)

	// return [(bot_left_x, bot_left_y), (top_right_x, top_right_y)]

	// public Vector3 BoundingBox(Vector3[] points)
	// {

	// 	float bot_left_x = (float)Mathf.Inf;
	// 	float bot_left_y = (float)Mathf.Inf;
	// 	float top_right_x = (float)Mathf.Inf;
	// 	float top_right_y = (float)Mathf.Inf;

	// 	for 


	// 	return new Vector3;
	// }

	public void AddToScene(Node3D node, Transform3D transform)
	{
		var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", node);
		node.Transform = transform;
	}

	public ItemData GenerateItem()
	{
		// Select random item from database
		string baseItemType = DB.SelectFiltered(DBItems);
		string itemType = DBItems[baseItemType]["type"].ToString();
		if (itemType == "equipment") return GenerateEquipment(baseItemType);
		if (itemType == "key") return GenerateKey(baseItemType);
		return null;
	}

	private ItemKeyData GenerateKey(string itemID)
	{
		string mapID = DB.SelectFiltered(DBMaps);
		return new()
		{
			name = (string)DBMaps[mapID]["name"] + " Key",
			description = (string)DBItems[itemID]["description"],
			zonePath = "res://zones/" + DBMaps[mapID]["path"] + ".tscn",
			Type = ParseEnum<ItemType>((string)DBItems[itemID]["subType"]),
			texture = GD.Load<Texture2D>("res://textures/227.png")
		};
	}

	public ItemEquipmentData GenerateEquipment(string itemID)
	{
		ItemEquipmentData item = new();

		if (GD.Randf() >= 0.25f)
		{
			Dictionary<string, Variant> conditions = new() { { "type", "prefix" } };
			item.prefix = GenerateAffix(DB.SelectFiltered(DBAffixes, conditions));
		}
		
		if (GD.Randf() >= 0.25f)
		{
			Dictionary<string, Variant> conditions = new() { { "type", "suffix" } };
			item.suffix = GenerateAffix(DB.SelectFiltered(DBAffixes, conditions));
		}
		item.name = (string)DBItems[itemID]["name"];
		if (item.prefix != null) item.name = item.prefix.Name + " " + (string)DBItems[itemID]["name"];
		if (item.suffix != null) item.name += " " + item.suffix.Name;
		item.description = (string)DBItems[itemID]["description"];
		item.rarity = item.prefix != null && item.suffix != null ? 1 : 0;
		item.Type = ParseEnum<ItemType>((string)DBItems[itemID]["subType"]);
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
		affix.Name = (string)DBAffixes[id]["title"];
		affix.TargetType = ParseEnum<AttributeType>((string)DBAffixes[id]["target"]);
		affix.attributeModifier = modifier;
		return affix;
	}

	public static T ParseEnum<T>(string value) => (T)Enum.Parse(typeof(T), value, true);

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

		GD.Print(externalInventoryOwner);
		if (invOpen)
		{
			hud.Show();
			inventory.Hide();
			Input.MouseMode = Input.MouseModeEnum.Captured;
			invOpen = false;
		}
		else
		{
			hud.Hide();
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
	// public Damage GetPlayerDamage()
	// {
	// 	//base damage
	// 	float damage = (float)GD.RandRange(equippedWeapon.damageMin, equippedWeapon.damageMax);
	// 	//crit
	// 	float critValue = equippedWeapon.critChance;
	// 	float critBase = Mathf.Ceil(critValue);
	// 	float critRoll = GD.Randf();
	// 	bool crit = critRoll <= critValue;
	// 	bool critExcess = critRoll <= (critValue - MathF.Truncate(critValue));
	// 	damage *= critExcess ? critBase : 1;
	// 	return Damage.InitDamage(damage, crit, player);
	// }

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
