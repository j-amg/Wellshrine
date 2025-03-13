public partial class StatModifier : Zone
{
    public string name;
    public string description;
    public string modifier;
    public float value;
    public float baseValue;
    public int amount;


    public static StatModifier InitModifier(string name, string description, string modifier, float value,float baseValue, int amount)
    {
        StatModifier s = new() {name = name, description = description, modifier = modifier, value = value, baseValue = baseValue, amount = amount};
        return s;
    }
}