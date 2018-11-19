using System;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
internal class Player
{
	private static void Main(string[] args)
	{
		string[] inputs;
		inputs = Console.ReadLine().Split(' ');
		int width = int.Parse(inputs[0]);
		int height = int.Parse(inputs[1]);
		int myId = int.Parse(inputs[2]);

		// game loop
		while (true)
		{
			Log("Table:");
			for (int i = 0; i < height; i++)
			{
				string row = Console.ReadLine();
				Log(row);
			}

			Log("");
			int entities = int.Parse(Console.ReadLine());
			Log($"Entities: {entities}");
			Log("entityType owner x y param1 param2");
			for (int i = 0; i < entities; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int entityType = int.Parse(inputs[0]);
				int owner = int.Parse(inputs[1]);
				int x = int.Parse(inputs[2]);
				int y = int.Parse(inputs[3]);
				int param1 = int.Parse(inputs[4]);
				int param2 = int.Parse(inputs[5]);
				Log($"{entityType} {owner} {x} {y} {param1} {param2}");
			}

			Console.WriteLine("BOMB 6 5");
		}
	}

	private static void Log(string message, bool newLine = true)
	{
		if (newLine)
		{
			Console.Error.WriteLine(message);
		}
		else
		{
			Console.Error.Write(message);
		}
	}
}