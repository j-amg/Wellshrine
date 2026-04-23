using System.Linq;
using Godot;
using Godot.Collections;

public partial class Zone : Node3D
{
	[Export]
	public string objective;
	[Export]
	public Door door;
	[Export]
	public Player player;

	[Export]
	public EnemySpawner spawner;
	public bool objectiveComplete = false;

	public override void _Ready()
	{
		SignalManager.Singleton.EmitSignal(SignalManager.SignalName.ZoneEntered);
		SpawnDelay()
	
	}

	public async void SpawnDelay()
	{
		await ToSignal(spawner, "ready");
		spawner?.Spawn(10, Global.Singleton.enemyArray);
	}

	public void ResetPlayer(Transform3D trans)
	{
		Player p = GD.Load<PackedScene>("res://player.tscn").Instantiate<Player>();
		player.QueueFree();	
		AddChild(p);
		player = p;
		p.Transform = trans;
		//Global.Singleton.Gets();
		//player.Position = new(0,0,0);
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

	public static Zone GenerateTileZone()
	{
		int[,,] tileArray = new int[1000,200,1000];


		Zone baseZone = GD.Load<PackedScene>("res://zones/testCombinedLevel.tscn").Instantiate<Zone>();
		Node3D startingTile = Global.Singleton.tileArray[1].Instantiate<Node3D>();
		Vector3 spawnPoint = new(500,100,500);
		float spawnRot = 0;
		AddMapToArray(startingTile, tileArray, spawnPoint, spawnRot);
		SpawnRooms(startingTile, 50, tileArray, startingTile.Transform);
		baseZone.AddChild(startingTile);
		return baseZone;
		
	}

	public static void SpawnRooms(Node3D tile, int credits, int[,,] tileArray, Transform3D currentTransform)
	{

		foreach (Node3D connector in tile.GetNode<Node>("connectionPoints").GetChildren())
			{

				// Node3D childTile = Global.Singleton.tileArray[GD.RandRange(1, Global.Singleton.tileArray.Count - 1)].Instantiate<Node3D>();
				// Area3D bounds = childTile.GetNode<Area3D>("Area3D");
				// tile.AddChild(bounds);

				

			
			//find the transform of the connector in global space
			Transform3D newtrans = (currentTransform * connector.Transform).Orthonormalized();

			float connectorRotation = Mathf.Atan2(newtrans.Basis.Z.X, newtrans.Basis.Z.Z);

			Vector3 newArrayPosition = new Vector3(500,100,500) + newtrans.Origin/2;

			Node3D childTile = Global.Singleton.tileArray[GD.RandRange(1, Global.Singleton.tileArray.Count - 1)].Instantiate<Node3D>();

			
			if (AddMapToArray(childTile, tileArray, newArrayPosition, connectorRotation) && credits > 0)
			{
				
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
				canPlaceRoom = false;
				break;
			}
			else if (tileArray[(int)transformedCellPos.X,(int)transformedCellPos.Y, (int)transformedCellPos.Z] == 1)
			{
				canPlaceRoom = false;
				break;
			}
			else
			{
				tileArray[(int)transformedCellPos.X,(int)transformedCellPos.Y, (int)transformedCellPos.Z] = 1;
			}
		}
		return canPlaceRoom;
	}
}
