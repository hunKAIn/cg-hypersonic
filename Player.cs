using System;
using System.Collections.Generic;
using System.Linq;
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
		string[] inputs;
		inputs = Console.ReadLine().Split(' ');
		int width = int.Parse(inputs[0]);
		int height = int.Parse(inputs[1]);
		MyID = int.Parse(inputs[2]);

		var boxes = new List<Coordinate>();
		Robot myRobot = new Robot();
		var myBombs = new List<Bomb>();
		char[,] board = new char[HEIGHT, WIDTH];

		// game loop
		while (true)
		{
			boxes.Clear();
			myBombs.Clear();
			Bombs.Clear();

			Log("Board:");
			for (int i = 0; i < height; i++)
			{
				string row = Console.ReadLine();
				Log(row);
				for (int j = 0; j < width; j++)
				{
					board[i, j] = row[j];
					if (row[j].Equals(EMPTY_BOX) || row[j].Equals(RANGE_BOX) || row[j].Equals(BOMB_BOX))
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
							myRobot.X = x;
							myRobot.Y = y;
							myRobot.AvailableBombs = param1;
							myRobot.BombRange = param2;
						}
						break;

					case 1: // bomb
						var bomb = new Bomb(x, y, param1, param2);
						if (owner == MyID)
						{
							// owner, x, y, num. of rounds to explode, explosion range
							myBombs.Add(bomb);
						}
						Bombs.Add(bomb);
						break;
				}
			}

			int[,] scoreBoard = GetScoreBoard(board, myRobot.BombRange);
			PrintScoreBoard(scoreBoard);

			Dictionary<int, List<Coordinate>> scoredDict = GetScoreDictionary(scoreBoard);

			int closestDist = 0;
			Coordinate closestBox = null;
			List<Coordinate> places;
			for (int score = 4; score > 0; score--)
			{
				if (!scoredDict.TryGetValue(score, out places))
				{
					continue;
				}

				foreach (Coordinate place in places)
				{
					int tempDist = place.GetDistance(myRobot);
					if (closestBox == null || closestDist > tempDist)
					{
						closestDist = tempDist;
						closestBox = place;
					}
				}

				if (closestBox != null)
				{
					break;
				}
			}

			Log($"Closest: {closestBox.X}, {closestBox.Y}");
			if (closestBox == null)
			{
				Console.WriteLine($"MOVE {myRobot.X} {myRobot.Y}");
			}
			else if (closestDist == 0 && myRobot.AvailableBombs > 0)
			{
				Console.WriteLine($"BOMB {closestBox.X} {closestBox.Y}");
			}
			else
			{
				Console.WriteLine($"MOVE {closestBox.X} {closestBox.Y}");
			}
		}
	}

	private Dictionary<int, List<Coordinate>> GetScoreDictionary(int[,] scoreBoard)
	{
		var scoreDict = new Dictionary<int, List<Coordinate>>();
		for (int y = 0; y < HEIGHT; y++)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				int score = scoreBoard[y, x];
				List<Coordinate> coordinates;
				if (!scoreDict.TryGetValue(score, out coordinates))
				{
					coordinates = new List<Coordinate>();
					scoreDict.Add(score, coordinates);
				}

				coordinates.Add(new Coordinate(x, y));
			}
		}

		return scoreDict;
	}

	private int[,] GetScoreBoard(char[,] board, int range)
	{
		var scoreBoard = new int[HEIGHT, WIDTH];

		for (int y = 0; y < HEIGHT; y++)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				char cell = board[y, x];
				if (!cell.Equals(FLOOR_CELL))
				{
					scoreBoard[y, x] = 0;
					continue;
				}

				if (Bombs.Any(b => b.X == x && b.Y == y))
				{
					scoreBoard[y, x] = 0;
					continue;
				}

				scoreBoard[y, x] = GetHitCount(board, new Bomb(x, y, 8, range));
			}
		}

		return scoreBoard;
	}

	private int GetHitCount(char[,] board, Bomb bomb)
	{
		int rangeOffset = bomb.Range - 1;
		int hits = 0;

		// vertical to top
		for (int y = bomb.Y - 1; y >= bomb.Y - rangeOffset; y--)
		{
			if (y < 0)
			{
				continue;
			}

			if (CalcHit(board, new Coordinate(bomb.X, y)))
			{
				hits++;
				break;
			}

			//board[y, bomb.X] = 'v';
		}

		// vertical to bottom
		for (int y = bomb.Y + 1; y <= bomb.Y + rangeOffset; y++)
		{
			if (y >= HEIGHT)
			{
				break;
			}

			if (CalcHit(board, new Coordinate(bomb.X, y)))
			{
				hits++;
				break;
			}

			//board[y, bomb.X] = 'V';
		}

		// horizontal to left
		for (int x = bomb.X - 1; x >= bomb.X - rangeOffset; x--)
		{
			if (x < 0)
			{
				continue;
			}

			if (CalcHit(board, new Coordinate(x, bomb.Y)))
			{
				hits++;
				break;
			}

			//board[bomb.Y, x] = 'h';
		}

		// horizontal to right
		for (int x = bomb.X + 1; x <= bomb.X + rangeOffset; x++)
		{
			if (x >= WIDTH)
			{
				break;
			}

			if (CalcHit(board, new Coordinate(x, bomb.Y)))
			{
				hits++;
				break;
			}
			//board[bomb.Y, x] = 'H';
		}

		return hits;
	}

	private bool CalcHit(char[,] board, Coordinate hit)
	{
		switch (board[hit.Y, hit.X])
		{
			case EMPTY_BOX:
			case RANGE_BOX:
			case BOMB_BOX:
				return true;
		}

		return false;
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

internal class Robot : Coordinate
{
	public int AvailableBombs { get; set; }
	public int BombRange { get; set; }

	public Robot()
	{
		AvailableBombs = 1;
		BombRange = 3;
	}
}

internal class Bomb : Coordinate
{
	public int Rounds { get; set; }
	public int Range { get; }

	public Bomb(int rounds, int range)
		: this(0, 0, rounds, range)
	{
	}

	public Bomb(int x, int y, int rounds, int range)
		: base(x, y)
	{
		Rounds = rounds;
		Range = range;
	}
}

internal static class GameContext
{
	public const int WIDTH = 13;
	public const int HEIGHT = 11;

	public const char FLOOR_CELL = '.';
	public const char EMPTY_BOX = '0';
	public const char RANGE_BOX = '1';
	public const char BOMB_BOX = '2';

	public static int MyID { get; set; }

	public static List<Bomb> Bombs { get; } = new List<Bomb>();

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

	public static void PrintScoreBoard(int[,] map)
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
