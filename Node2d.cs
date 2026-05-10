using Godot;
using System;


public partial class TeachingPaladin : Node2D
{

	public string[] numbers = ["a","b","c","d","f"];

	int IndexPosition = 0;

	public override void _Ready()
	{
		for (int i = 0; i < numbers.Length; i++)
		{
			GD.Print(numbers[i]);
		}

		for (int i = numbers.Length; i >= 0; i--)
		{
			GD.Print(numbers[i]);
		}


		foreach (string number in numbers)
		{
			GD.Print(number);
		}
		
	}
}
