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
		//spawner?.Spawn(10, Global.Singleton.enemyArray);
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
		Node3D startingTile = Global.Singleton.tileArray[0].Instantiate<Node3D>();
		Vector3 spawnPoint = new(500,30,500);
		float spawnRot = 0;
		AddMapToArray(startingTile.GetNode<GridMap>("NavigationRegion3D/GridMap"), tileArray, spawnPoint, spawnRot);
		SpawnRooms(startingTile, 40, tileArray, startingTile.Transform);
		baseZone.AddChild(startingTile);
		Global.Singleton.GotoZone(baseZone);
	}

	public static void SpawnRooms(Node3D tile, int credits, int[,,] tileArray, Transform3D currentTransform)
	{
		if (credits < 0) return;

		foreach (Node3D connector in tile.GetNode<Node>("connectionPoints").GetChildren())
			{
				// find the transform of the connector in global space
			Transform3D newtrans = (currentTransform * connector.Transform).Orthonormalized();

			float connectorRotation = Mathf.Atan2(newtrans.Basis.Z.X, newtrans.Basis.Z.Z);

			Vector3 newArrayPosition = new Vector3(500,30,500) + newtrans.Origin/2;

			Node3D childTile = Global.Singleton.tileArray[GD.RandRange(0, Global.Singleton.tileArray.Count - 1)].Instantiate<Node3D>();
			
			if (AddMapToArray(childTile.GetNode<GridMap>("NavigationRegion3D/GridMap"), tileArray, newArrayPosition, connectorRotation))
			{
				GD.Print("spawned room");
				SpawnRooms(childTile, credits - 10, tileArray, newtrans);
				tile.AddChild(childTile);
				childTile.Transform = connector.Transform;
			}
		}
	}

	public static bool AddMapToArray(GridMap map, int[,,] tileArray, Vector3 pos, float rot)
	{
		bool canPlaceRoom = true;
		foreach (Vector3 cell in map.GetUsedCells())
		{
			//GD.Print(cell);
			Vector3 transformedCellPos = new Vector3(cell.X + 1, cell.Y, cell.Z).Rotated(Vector3.Up, rot) + pos;
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
				//GD.Print("tile already exists: " + (transformedCellPos - new Vector3(500,30,500)));
				canPlaceRoom = false;
				break;
			}
			else
			{
				//GD.Print("tile placed at: " + (transformedCellPos - new Vector3(500,30,500)));
				tileArray[(int)transformedCellPos.X,(int)transformedCellPos.Y, (int)transformedCellPos.Z] = 1;
			}
		}
		GD.Print(canPlaceRoom);
		return canPlaceRoom;
	}
}
