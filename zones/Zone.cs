using System.Linq;
using Godot;
using Godot.Collections;

public partial class Zone : Node3D
{
	[Signal]
	public delegate void ZoneEnteredEventHandler(Zone zone);
	[Signal]
	public delegate void ZoneObjectiveCompleteEventHandler(Zone zone);
	[Export]
	public string objective;
	[Export]
	public Door door;

	[Export]
	public EnemySpawner spawner;
	public bool objectiveComplete = false;

	public override void _Ready()
	{
		CallDeferred("emit_signal", SignalName.ZoneEntered, this);
		SpawnDelay();
	
	}

	public async void SpawnDelay()
	{
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		spawner?.Spawn(10, Global.Singleton.enemyArray);
	}

	public virtual void UpdateObjective() => CompleteObjective();
	public void CompleteObjective()
	{
		objectiveComplete = true;
		objective = "Enter the next zone";
		EmitSignal(SignalName.ZoneObjectiveComplete, this);
		//door.Open();
	}

	public virtual void CloseZone()
	{

	}

	public static void GenerateTileZone()
	{
		int[,,] tileArray = new int[1000,200,1000];


		Zone baseZone = GD.Load<PackedScene>("res://zones/testCombinedLevel.tscn").Instantiate<Zone>();
		Node3D startingTile = Global.Singleton.tileArray[1].Instantiate<Node3D>();
		Vector3 spawnPoint = new(500,100,500);
		float spawnRot = 0;
		AddMapToArray(startingTile, tileArray, spawnPoint, spawnRot);
		SpawnRooms(startingTile, 20, tileArray, startingTile.Transform);
		baseZone.AddChild(startingTile);
		Global.Singleton.GotoZone(baseZone);
	}

	public static void SpawnRooms(Node3D tile, int credits, int[,,] tileArray, Transform3D currentTransform)
	{

		foreach (Node3D connector in tile.GetNode<Node>("connectionPoints").GetChildren())
			{
			//find the transform of the connector in global space
			Transform3D newtrans = (currentTransform * connector.Transform).Orthonormalized();

			float connectorRotation = Mathf.Atan2(newtrans.Basis.Z.X, newtrans.Basis.Z.Z);

			Vector3 newArrayPosition = new Vector3(500,30,500) + newtrans.Origin/2;

			Node3D childTile = Global.Singleton.tileArray[GD.RandRange(1, Global.Singleton.tileArray.Count - 1)].Instantiate<Node3D>();

			
			if (AddMapToArray(childTile, tileArray, newArrayPosition, connectorRotation) && credits > 0)
			{
				GD.Print("spawned room");
				SpawnRooms(childTile, credits - 10, tileArray, newtrans);
				tile.AddChild(childTile);
				childTile.Transform = connector.Transform;
			} else
			{
				Node3D door = Global.Singleton.tileArray[0].Instantiate<Node3D>();
				tile.AddChild(door);
				door.Transform = connector.Transform;
			}
		}
	}

	public static bool AddMapToArray(Node map, int[,,] tileArray, Vector3 pos, float rot)
	{
		bool canPlaceRoom = true;

		// BoxShape3D roomShape = (BoxShape3D)map.GetNode<CollisionShape3D>("Area3D/CollisionShape3D").Shape;

		// for (int x = 0; x < roomShape.Size.X/2; x++)
		// {
		// 	for (int y = 0; y < roomShape.Size.Y/2; x++)
		// 	{
		// 		for (int z = 0; z < roomShape.Size.Z/2; x++)
		// 		{
		// 			Vector3 transformedCellPos = new Vector3(x + 1, y, z + 1).Rotated(Vector3.Up, rot) + pos;
		// 			if (
		// 			transformedCellPos.X < 0 || transformedCellPos.X >= tileArray.GetLength(0)
		// 			|| transformedCellPos.Y < 0 || transformedCellPos.Y >= tileArray.GetLength(1)
		// 			|| transformedCellPos.Z < 0 || transformedCellPos.Z >= tileArray.GetLength(2)
		// 			)
		// 			{
		// 				// if it does, then break out of this loop as the selected room would cause an overlap
		// 				GD.Print("room out of bounds. Tile: " + transformedCellPos);
		// 				canPlaceRoom = false;
		// 				break;
		// 			}
		// 			else if (tileArray[(int)transformedCellPos.X,(int)transformedCellPos.Y, (int)transformedCellPos.Z] == 1)
		// 			{
		// 				GD.Print("tile already exists: " + (transformedCellPos - new Vector3(500,30,500)));
		// 				canPlaceRoom = false;
		// 				break;
		// 			}
		// 			else
		// 			{
		// 				GD.Print("tile placed at: " + (transformedCellPos - new Vector3(500,30,500)));
		// 				tileArray[(int)transformedCellPos.X,(int)transformedCellPos.Y, (int)transformedCellPos.Z] = 1;
		// 			}
		// 		}
		// 	}
		// }

		foreach (Vector3 cell in map.GetNode<GridMap>("GridMap").GetUsedCells())
		{
			//GD.Print(cell);


			Vector3 transformedCellPos = new Vector3(cell.X + 1, cell.Y, cell.Z + 1).Rotated(Vector3.Up, rot) + pos;
			// check if tile already exists in array
			if (
			transformedCellPos.X < 0 || transformedCellPos.X >= tileArray.GetLength(0)
			|| transformedCellPos.Y < 0 || transformedCellPos.Y >= tileArray.GetLength(1)
			|| transformedCellPos.Z < 0 || transformedCellPos.Z >= tileArray.GetLength(2)
			)
			{
				// if it does, then break out of this loop as the selected room would cause an overlap
				GD.Print("room out of bounds. Tile: " + transformedCellPos);
				canPlaceRoom = false;
				break;
			}
			else if (tileArray[(int)transformedCellPos.X,(int)transformedCellPos.Y, (int)transformedCellPos.Z] == 1)
			{
				GD.Print("tile already exists: " + (transformedCellPos - new Vector3(500,30,500)));
				canPlaceRoom = false;
				break;
			}
			else
			{
				GD.Print("tile placed at: " + (transformedCellPos - new Vector3(500,30,500)));
				tileArray[(int)transformedCellPos.X,(int)transformedCellPos.Y, (int)transformedCellPos.Z] = 1;
			}
		}
		GD.Print(canPlaceRoom);
		return canPlaceRoom;
	}
}
