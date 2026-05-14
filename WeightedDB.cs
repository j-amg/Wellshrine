using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DB : Resource
{

	public static Dictionary<string, Dictionary<string, Variant>> JsonToDict(string path)
	{

		FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read); // Open File
		string json_string = file.GetAsText(); // Open JSON as string

		Json JSON = new();
		var error = JSON.Parse(json_string); // Parsing
		if (error == Error.Ok)
		{
			return JSON.Data.AsGodotDictionary<string, Dictionary<string, Variant>>(); // Convert JSON String to Dictionary
		}
		else
		{
			GD.Print("Invalid JSON path");
			return null;
		}
	}

	public static string SelectFiltered(Dictionary<string, Dictionary<string, Variant>> items, Dictionary<string, Variant> conditions = null)
	{
		float totalWeight = 0.0f;
		// create new dictionary where each item meets conditions
		Dictionary<string, Dictionary<string, Variant>> dict = [];
		foreach (string item in items.Keys)
		{
			bool canAdd = true;
			if (conditions != null)
			{
				foreach (string key in conditions.Keys)
				{
					if (!items[item].TryGetValue(key, out Variant value)) canAdd = false; // item does not meet condition
					if (!value.ToString().Equals(conditions[key].ToString())) canAdd = false; // item does not meet condition
				}
			}

			if (canAdd)
			{
				totalWeight += (float)items[item]["rollWeight"];
				dict.Add(item, items[item]);
				dict[item]["accWeight"] = totalWeight;
			}
		}

		//Roll the number
		double roll = GD.RandRange(0.0, totalWeight);
		// GD.Print("roll: " + roll);
		// GD.Print("dictionary size: " + dict.Count);

		//Now search for the first with acc_weight > roll
		foreach (string item in dict.Keys)
		{
			if ((float)dict[item]["accWeight"] > roll)
			{
				GD.Print("printing from db" + item);
				return item;
			}
		}
		return null;
	}

	// public Dictionary<string, Variant> GetValueByName(string name)
	// {
	// 	if (items.TryGetValue(name, out Dictionary<string, Variant> value)) return value;
	// 	return null;	
	// }

	// public Variant GetRandomAffixOfType(string type)
	// {
	// 	if (DBAffixes.ContainsKey(type))
	// 	{

	// 		return DBAffixes[type].Keys.;
	// 	}
	// 	return 0;	
	// }

}