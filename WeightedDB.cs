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
		} else
		{
			GD.Print("Invalid JSON path");
			return null;
		} 
	}

	public static Dictionary<string, Variant> SelectFiltered(Dictionary<string, Dictionary<string, Variant>> items, Dictionary<string, Variant> conditions)
	{
        float totalWeight = 0.0f;
        // create new dictionary where each item meets conditions
        Dictionary<string, Dictionary<string, Variant>> dict = [];
        foreach (string item in items.Keys)
        {
            foreach (string key in conditions.Keys)
            {
                if (!items[item].TryGetValue(key, out Variant value)) break; // item does not meet condition
                if (value.ToString() == conditions[key].ToString())
                {
                    totalWeight += (float)items[item]["rollWeight"];
                    dict.Add(item, items[item]);
                    dict[item]["accWeight"] = totalWeight;
                }
            }
        }
        //Roll the number
		double roll = GD.RandRange(0.0, totalWeight);
        GD.Print("roll: " + roll);
        
		//Now search for the first with acc_weight > roll
		foreach (Dictionary<string, Variant> item in dict.Values)
		{
			if ((float)item["accWeight"] > roll) return item;
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