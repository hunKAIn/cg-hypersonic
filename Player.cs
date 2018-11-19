using System;
using System.Collections.Generic;
using static GameContext;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
internal class Player
{
	private static void Main(string[] args)
	{
		new Player().Run();
	}

	private void Run()
	{
		//var a = new Coordinate(0, 0);
		//var b = new Coordinate(1, 1);
		//var dif = a.GetDistance(b);
		//return;

		string[] inputs;
		inputs = Console.ReadLine().Split(' ');
		int width = int.Parse(inputs[0]);
		int height = int.Parse(inputs[1]);
		MyID = int.Parse(inputs[2]);

		var boxes = new List<Coordinate>();
		Coordinate myRobot = null;
		int[] myBomb = null;
		char[,] board = new char[HEIGHT, WIDTH];

		// game loop
		while (true)
		{
			boxes.Clear();
			myBomb = null;

			Log("Board:");
			for (int i = 0; i < height; i++)
			{
				string row = Console.ReadLine();
				Log(row);
				for (int j = 0; j < width; j++)
				{
					board[i, j] = row[j];
					if (row[j].Equals(EMPTY_BOX))
					{
						boxes.Add(new Coordinate(j, i));
					}
				}
			}

			Log("");
			int entities = int.Parse(Console.ReadLine());
			Log($"Entities: {entities}");
			Log("entityType owner x y param1 param2");
			for (int i = 0; i < entities; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int entityType = int.Parse(inputs[0]); // player: 0, bomb: 1
				int owner = int.Parse(inputs[1]);
				int x = int.Parse(inputs[2]);
				int y = int.Parse(inputs[3]);
				int param1 = int.Parse(inputs[4]);  // num. of bombs, num. of rounds
				int param2 = int.Parse(inputs[5]);  // explosion range
				Log($"{entityType} {owner} {x} {y} {param1} {param2}");

				switch (entityType)
				{
					case 0: // player
						if (owner == MyID)
						{
							// owner, x, y, num. of bombs, explosion range
							myRobot = new Coordinate(x, y);
						}
						break;

					case 1: // bomb
						if (owner == MyID)
						{
							// owner, x, y, num. of rounds to explode, explosion range
							myBomb = new int[] { owner, x, y, param1, param2 };
						}
						break;
				}
			}

			int closestDist = 0;
			Coordinate closestBox = null;
			foreach (Coordinate box in boxes)
			{
				int tempDist = box.GetDistance(myRobot);
				if (closestBox == null || closestDist > tempDist)
				{
					closestDist = tempDist;
					closestBox = box;
				}
			}

			if (closestBox == null)
			{
				Console.WriteLine("WAIT");
			}
			else if (closestDist == 1 && myBomb == null)
			{
				Console.WriteLine($"BOMB {closestBox.X} {closestBox.Y}");
			}
			else
			{
				Console.WriteLine($"MOVE {closestBox.X} {closestBox.Y}");
			}
		}
	}
}

internal class Coordinate
{
	public int X { get; set; }
	public int Y { get; set; }

	public Coordinate()
		: this(0, 0)
	{
	}

	public Coordinate(int x, int y)
	{
		X = x;
		Y = y;
	}

	public int GetDistance(Coordinate other)
	{
		return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
	}
}

internal static class GameContext
{
	public const int WIDTH = 13;
	public const int HEIGHT = 11;

	public const char FLOOR_CELL = '.';
	public const char EMPTY_BOX = '0';

	public static int MyID { get; set; }

	public static void PrintBoard(char[,] map)
	{
		for (int r = 0; r < HEIGHT; r++)
		{
			for (int c = 0; c < WIDTH; c++)
			{
				Log(map[r, c].ToString(), false);
			}
			Log("");
		}
	}

	public static void Log(string message, bool newLine = true)
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
