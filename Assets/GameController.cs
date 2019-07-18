using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Random = UnityEngine.Random;

namespace ConnectFour
{
	public class GameController : MonoBehaviour 
	{
		// Below are all the variables we will use:

		// These are GameObjects that represent pieces and the fiducial marker
		public GameObject redPiece;
		public GameObject bluePiece;
		public GameObject ImageTarget;

		// These are what we spawn whenever someone puts in a piece
		GameObject redClone;
		GameObject blueClone;

		// The variable "Mode" details who is playing
		// 0 is PvP
		// 1, 2, and 3 are different levels of CPU difficulty - defaults to hardest
		// 4 is watching the computer play itself
		int mode = modeKeeper.Mode;

		// These are related to the board. We need to initialze them here and not in the start function,
		// so we can't use a loop
		int[][] board = new int[6][];
		int[] row1 = { 0, 0, 0, 0, 0, 0, 0 };
		int[] row2 = { 0, 0, 0, 0, 0, 0, 0 };
		int[] row3 = { 0, 0, 0, 0, 0, 0, 0 };
		int[] row4 = { 0, 0, 0, 0, 0, 0, 0 };
		int[] row5 = { 0, 0, 0, 0, 0, 0, 0 };
		int[] row6 = { 0, 0, 0, 0, 0, 0, 0 };

		// Self-explanitory
		int turnNumber = 0;
		bool playerTurn = false;
		
		// 0 winner means no winner, 1 and 2 means player 1 and 2 respectively won 
		int winner = 0;

		// The column to place the piece in
		int insertColumn = 0;

		// When someone takes their turn, loaded is false. When they finish, loaded is true.
		// This prevents more than one person going at a time
		bool loaded = true;
		
		// The x and y coordinates to place the piece on the board
		float x;
		float y;

		// Waits for you to make an input
		 UnityEvent m_MyEvent = new UnityEvent();

		// THE START FUNCTION - Use this for initialization
		void Start () 
		{
			// Tells you what game mode you are playing
			Debug.Log("Mode "+mode);

			// Initialize the board
			for (int i = 0; i < 6; i++)
			{
				board[i] = new int[7];
				
			}
			board[0] = row1;
			board[1] = row2;
			board[2] = row3;
			board[3] = row4;
			board[4] = row5;
			board[5] = row6;
			
		

			// To make it challenging, the computer will always go first. If it is PVP, then we need to disable that
			if (mode == 0)
			{
				playerTurn = true;
			}

			// Initialize listener
			m_MyEvent.AddListener(MyAction);
		}


		// THE UPDATE FUNCTION - This function is called every frame
		void Update ()
		{ 

			if (Input.anyKeyDown)
        	{
            	// Perform a turn of the game when you push something
				m_MyEvent.Invoke();
        	}
		}
			

// Below this is all of the game logic ------------------------------------------------------------------- 

		// This method runs a full turn
		void MyAction()
    	{
			// If there is no winner, put down a piece. If there is a winner, block moves and print a win message
			if (winner == 0)
            {
				// Prevents you from taking a turn if someone else is taking one
                if (!loaded)
                {
                    return; 
                }

                // Now we set loaded to false since we are about to assign a new move
                loaded = false;

				// If PvP, make a player move. We don't need to specifiy who since it is based on turn number.
                if (mode == 0)
                {
                    playerMove();
                }
				// If CPU vs CPU, then make a computer move
                else if (mode == 4)
                {
					if (UnityEngine.Random.Range(1,3) == 1)
					{
						CPUMedMove();
						Debug.Log("Med");
					}
					else
					{
						CPUHardMove();
						Debug.Log("Hard");
					}
                    
                }
				// If you are fighting a CPU, then make a move based on their difficulty
                else if (mode < 4)
                {
                    if (playerTurn == true)
                    {
                        playerMove();	
                    }
                    else if (mode == 1)
                    {
                        CPUHardMove();
                        playerTurn = true;
                    }
                    else if (mode == 2)
                    {
                        CPUMedMove();
                        playerTurn = true;
                    }
                    else
                    {
                        CPUEasyMove();
                        playerTurn = true;
                    }
                }
				checkWinner();
				loaded = true;
			}
			if(winner != 0)
            {
				// Print win message to Console if someone wins
				if (winner == 1)
				{
					Debug.Log("The winner is purple!");
				}
				if (winner == 2)
				{
					Debug.Log("The winner is gray!");
				}
            }
            return;
    	}

// Here are all the helper methods : ----------------------------------------------------------------------------- 

		// Takes in a column and puts in a piece
		void insertPiece(int col)
		{
			// Puts a piece in the lowest position
			bool valid = true;
			int row = -1;

			while (valid)
			{
				row = row + 1;

				if (board[row][col-1] == 0)
				{
					valid = false;
				}
			}

			// Puts a piece in the board that keeps track of all pieces
			board[row][col-1] = (turnNumber % 2) +1;

			// Puts a piece in the AR board
			x = (float)(.127*(col-4));
			y = (float)(.1905+.127*row);

			// Put it in the board
			if (turnNumber % 2 == 0)
			{
				blueClone = Instantiate(bluePiece, transform.position, Quaternion.identity);
				blueClone.transform.parent = ImageTarget.transform; 

				// Moves to x, y, 0
				Vector3 temp = new Vector3(x,y,0);
				blueClone.transform.localPosition = temp;

				//Shrinks
				temp = new Vector3(0.005f, 0.005f, 0.005f);
				blueClone.transform.localScale = temp;

				temp = new Vector3(0, 0, 0);
				blueClone.transform.rotation = Quaternion.Euler(temp);
			}
			else
			{
				redClone = Instantiate(redPiece, transform.position, Quaternion.identity);
				redClone.transform.parent = ImageTarget.transform; 

				// Moves to x, y, 0
				Vector3 temp = new Vector3(x,y,0);
				redClone.transform.localPosition = temp;


				//Shrinks
				temp = new Vector3(0.005f, 0.005f, 0.005f);
				redClone.transform.localScale = temp;

				temp = new Vector3(0, 0, 0);
				redClone.transform.rotation = Quaternion.Euler(temp);
			}
			
			turnNumber++;
			
		}

		void playerMove()
		{
			Debug.Log("Player Turn");
			insertColumn = 0;
			if (Input.GetKey("1"))
        	{
            	insertColumn = 1;
        	}
			if (Input.GetKey("2"))
        	{
            	insertColumn = 2;
        	}
			if (Input.GetKey("3"))
        	{
            	insertColumn = 3;
        	}
			if (Input.GetKey("4"))
        	{
            	insertColumn = 4;
        	}
			if (Input.GetKey("5"))
        	{
            	insertColumn = 5;
        	}
			if (Input.GetKey("6"))
        	{
            	insertColumn = 6;
        	}
			if (Input.GetKey("7"))
        	{
            	insertColumn = 7;
        	}

			if (insertColumn == 0 || board[5][insertColumn-1] != 0)
			{
				return;
			}
			insertPiece(insertColumn);
			playerTurn = false;
		}

		// This is where a CPU on easy plays a move
		void CPUEasyMove()
		{
			Debug.Log("CPU 1 Turn");

			// Play a random move (It is easy after all)
			bool valid = false;

			while(!valid)
			{
				insertColumn = Random.Range(1,8);

				if(board[5][insertColumn-1] == 0)
				{
					valid = true;
				}
			}
			insertPiece(insertColumn);

		}

		// This is where a CPU on medium plays a move
		void CPUMedMove()
		{
			Debug.Log("CPU 2 Turn");

			// Get board height and width.
			int boardHeight = board.Length;
			int boardWidth = board[0].Length;

			// If there is a winning move, take it.
			// Check if a row can be completed to win.
			for (int i = 0; i < boardHeight; i++)
			{
				// Check right insertion.
				for (int j = 0; j < 4; j++)
				{
					if (board[i][j] == board[i][j+1] &&  board[i][j+1] == board[i][j+2] && board [i][j+3] == 0)
					{
						if (i == 0)
						{
							insertPiece(j+4);
							return;
						}

						else if (board[i-1][j+3] != 0)
						{
							insertPiece(j+4);
							return;
						}
					}
				}

				// Check left insertion
				for (int j = 6; j > 2; j--)
				{
					if (board[i][j] == board[i][j-1] && board[i][j-1] == board[i][j-2] && board [i][j-3] == 0)
					{
						if (i == 0)
						{
							insertPiece(j-2);
							return;
						}

						else if (board[i-1][j-3] != 0)
						{
							insertPiece(j-2);
							return;
						}
					}
				}
			}

			// Check if a column can be completed to win.
			for (int j = 0; j < boardWidth; j++)
			{
				for (int i = 0; i < 3; i++)
				{
					if (board[i][j] == board[i+1][j] && board[i+1][j] == board[i+2][j] && board[i+3][j] == 0)
					{
						insertPiece(j+1);
						return;
					}
				}
			}
			
			// Otherwise, pick middle if it's not full
			if (board[5][3] == 0)
			{
				insertPiece(4);
				return;
			}

			// Else, try 0 then 6 then 1 then 5 then 2 then 4
			if (board[5][0] == 0)
			{
				insertPiece(1);
				return;
			}

			if (board[5][6] == 0)
			{
				insertPiece(7);
				return;
			}

			if (board[5][1] == 0)
			{
				insertPiece(2);
				return;
			}

			if (board[5][5] == 0)
			{
				insertPiece(6);
				return;
			}

			if (board[5][2] == 0)
			{
				insertPiece(3);
				return;
			}

			if (board[5][4] == 0)
			{
				insertPiece(5);
				return;
			}   
		}

		// This is where a CPU on hard plays a move
		void CPUHardMove()
		{
			Debug.Log("CPU 3 Turn");
			int[][] b = (int[][]) board.Clone();
			insertColumn = placePiece(b, 4, (turnNumber%2+1), ((turnNumber+1)%2+1));
			insertPiece(insertColumn);
		}



		public static int placePiece (int[][]board1, int length, int a, int o)
		{
			Array.Reverse(board1);
			
			// Find the dimensions of the board
			int boardHeight = board1.Length;
			int boardWidth = board1[0].Length;
		
			// What number on the board denotes an empty character? This number means it is not occupied.
			int empty = 0;
		
			// What number represents that the ai's currently owned piece?
			int ai = a;
			int opponent = o;
		
			// What are good moves that don't let the opponent win?
			List <int[]>possibleMoves = new List <int[]>();
		
			// Find the columns with empty placeable pieces
			for (int i = 0; i < boardWidth; i++)
			{
				if (board1[0][i] == empty)
				{
					// Find the first empty square on that column
					for (int j = boardHeight - 1; j >= 0; j--)
					{
						if (board1[j][i] == empty)
						{
							// Herein, j,i can be placed

							// Try placing it and checking if you will win, if so, move there.
							board1[j][i] = ai;
							if (checkBoard (board1, length) == ai)
							{
								Console.WriteLine("PLAYING WINNING MOVE");
								board1[j][i] = empty;
								return i+1;
							}
								board1[j][i] = opponent;
							if (checkBoard (board1, length) == opponent)
							{
								board1[j][i] = ai;
								Console.WriteLine("BLOCKING WINNING MOVE");
								board1[j][i] = empty;
								return i+1;
							}
							// Otherwise, check if it allows the opponent to directly win
							if (j - 1 > 0)
							{
								board1[j - 1][i] = opponent;
								if (checkBoard (board1, length) == -1)
								{
									possibleMoves.Add (new int[]{j,i});
								}
								board1[j - 1][i] = empty;
							}
							else
							{
								possibleMoves.Add (new int[]{j,i});
							}

							// Reset the change to empty
							board1[j][i] = empty;

							j= -1;
						}
					}
				}
			}
		
			// If there are no not-losing moves, do the first available move.
			if (possibleMoves.Count == 0)
			{
				// Find the first playable column
				for (int i = 0; i < boardWidth; i++)
				{
					if (board1[0][i] == empty)
					{
						// Find the first empty square on that column
						for (int j = boardHeight - 1; j >= 0; j--)
						{
							if (board1[j][i] == empty)
							{
								board1[j][i] = ai;
								board1[j][i] = empty;
								return i+1;
							}
						}
					}
				}
			}
			// If there is only one non-losing move, then play it.
			else if (possibleMoves.Count == 1)
			{
				int[] t = possibleMoves[0];
				board1[t[0]][t[1]] = ai;
				board1[t[0]][t[1]] = empty;
				return t[1]+1;
			}
			// If there is more than one non-losing move, then ...
			else
			{
				// Do the move that boarders most pieces, both yours and your opponents
		
				// Remember which is the best so far
				int borderMax = -1;
				int index = 0;
				int[] t;
		
				for (int k = 0; k < possibleMoves.Count; k++)
				{
					t = possibleMoves[k];
		
					int count = 0;
		
					/*
						000
						x00
						000
						*/
					if (t[1] > 0 && board1[t[0]][t[1]-1] != empty)
					{
						count++;
					}
		
					/*
						000
						x0x
						000
						*/
					if (t[1] <= boardWidth - 2 && board1[t[0]][t[1] + 1] != empty)
					{
						count++;
					}
		
					/*
						000
						x0x
						0x0
						*/
					if (t[0] <= boardHeight - 2 && board1[t[0] + 1][t[1]] != empty)
					{
						count++;
					}
		
					/*
						000
						x0x
						xx0
						*/
					if (t[0] <= boardHeight - 2 && t[1] > 0 && board1[t[0] + 1][t[1] - 1] != empty)
					{
						count++;
					}
		
					/*
						000
						x0x
						xxx
						*/
					if (t[0] <= boardHeight - 2 && t[1] <= boardWidth - 2 && board1[t[0] + 1][t[1] + 1] != empty)
					{
						count++;
					}
		
					/*
						x00
						x0x
						xxx
						*/
					if (t[0] > 0 && t[1] > 0 && board1[t[0] - 1][t[1] - 1] != empty)
					{
						count++;
					}
		
					/*
						x0x
						x0x
						xxx
						*/
					if (t[0] > 0 && t[1] <= boardWidth - 2 && board1[t[0] - 1][t[1] + 1] != empty)
					{
						count++;
					}	
		
					if (count > borderMax)
					{
							borderMax = count;
							index = k;
					}
					else if (count == borderMax)
					{
						if (Math.Abs(k - boardWidth/2) <= Math.Abs(index - boardWidth/2))
						{
							index = k;
						}
					}
				}
				// Play this "optimal" move!
				t = possibleMoves[index];
				board1[t[0]][t[1]] = ai;
				board1[t[0]][t[1]] = empty;
				return t[1]+1;
			}
			int temp = 0;
			bool valid = false;
			while(!valid)
			{
				temp = UnityEngine.Random.Range(1,8);

				if(board1[5][temp-1] == 0)
				{
					valid = true;
				}
			}
			return temp;
		}
 
		// Takes in a int double array of a board and the length to detect and returns the integer value of the winner.
		public static int checkBoard (int[][]board2, int length)
		{
		// Find the dimensions of the board
		int boardHeight = board2.Length;
		int boardWidth = board2[0].Length;
		// What number on the board denotes an empty character? This number cannot "win"
		int empty = 0;
	
		// For each row, check if the horizontal condition for winning is met
		for (int i = 0; i < boardHeight; i++)
			{
			for (int j = 0; j <= boardWidth - length; j++)
				{
				if (board2[i][j] == empty)
					{
						continue;
					}
				bool connected = true;
				for (int k = 1; k < length; k++)
					{
						if (board2[i][j + k] != board2[i][j])
						{
							connected = false;
						}
					}
				if (connected)
					{
						Console.WriteLine ("Horizontal Detected at" + i + " " + j);
						return board2[i][j];
					}
				}
			}
		// For each column check if the vertical condition is met
		for (int i = 0; i < boardWidth; i++)
			{
			for (int j = 0; j <= boardHeight - length; j++)
				{
				if (board2[j][i] == empty)
					{
						continue;
					}
				bool connected = true;
	
				for (int k = 1; k < length; k++)
					{
						if (board2[j + k][i] != board2[j][i])
						{
							connected = false;
						}
					}
				if (connected)
					{
						Console.WriteLine ("Vertical Detected at" + j + " " + i);
						return board2[j][i];
					}
				}
			}
		// For the board, check the diagonals this way / or this way \
		for (int i = 0; i < boardWidth; i++)
			{
			for (int j = 0; j < boardHeight; j++)
				{
				if (board2[j][i] == empty)
					{
						continue;
					}
	
				if (i + length -1 <= boardWidth - 1 && j + length - 1 <= boardHeight -1)
					{
						bool connected1 = true;
						for (var k = 1; k <= (length-1); k++)
						{
							if (board2[j + k][i + k] != board2[j][i])
							{
								connected1 = false;
							}
						}
					if (connected1)
						{
							Console.WriteLine ("Diagonal Detected at" + j + " " +
												i);
							return board2[j][i];
						}
					}
				if (i + length - 1 <= boardWidth - 1 && j - (length - 1) >= 0)
					{
						bool connected2 = true;
						for (int k2 = 1; k2 <= (length-1); k2++)
						{
							if (board2[j - k2][i + k2] != board2[j][i])
							{
								connected2 = false;
							}
						}
						if (connected2)
						{
							Console.WriteLine ("Diagonal Detected at" + j + " " +
												i);
							return board2[j][i];
						}
					}
				}
			}
	
		// No connections of length "length"
		return -1;
		}

		// By default, the winner is set to zero. This function will search for a 
		// four in a row, and if it finds one it will set the winner to the number
		// that represents the winning player on the board array of arrays
		void checkWinner()
		{
			int length = 4;

			// Find the dimensions of the board
			int boardHeight = board.Length;
			int boardWidth = board[0].Length;
			// What number on the board denotes an empty character? This number cannot "win"
			int empty = 0;
		
			// For each row, check if the horizontal condition for winning is met
			for (int i = 0; i < boardHeight; i++)
				{
				for (int j = 0; j <= boardWidth - length; j++)
					{
					if (board[i][j] == empty)
						{
							continue;
						}
					bool connected = true;
					for (int k = 1; k < length; k++)
						{
							if (board[i][j + k] != board[i][j])
							{
								connected = false;
							}
						}
					if (connected)
						{
							Debug.Log("Horizontal Detected at" + i + " " + j);
							winner = board[i][j];
							return;
						}
					}
				}
			// For each column check if the vertical condition is met
			for (int i = 0; i < boardWidth; i++)
			{
				for (int j = 0; j <= boardHeight - length; j++)
				{
					if (board[j][i] == empty)
					{
							continue;
					}
					bool connected = true;
		
					for (int k = 1; k < length; k++)
					{
						if (board[j + k][i] != board[j][i])
						{
							connected = false;
						}
					}
					if (connected)
						{
							Debug.Log("Vertical Detected at" + j + " " + i);
							winner = board[j][i];
							return;
						}
					}
			}
			// For the board, check the diagonals this way / or this way \
			for (int i = 0; i < boardWidth; i++)
				{
				for (int j = 0; j < boardHeight; j++)
					{
					if (board[j][i] == empty)
						{
							continue;
						}
		
					if (i + length -1 <= boardWidth - 1 && j + length - 1 <= boardHeight -1)
						{
							bool connected1 = true;
							for (int k = 1; k <= (length-1); k++)
							{
								if (board[j + k][i + k] != board[j][i])
								{
									connected1 = false;
								}
							}
						if (connected1)
							{
								Debug.Log("Diagonal Detected at" + j + " " +i);
								winner = board[j][i];
								return;
							}
						}
					if (i + length - 1 <= boardWidth - 1 && j - (length - 1) >= 0)
						{
							bool connected2 = true;
							for (int k2 = 1; k2 <= (length-1); k2++)
							{
								if (board[j - k2][i + k2] != board[j][i])
								{
									connected2 = false;
								}
							}
							if (connected2)
							{
								Debug.Log("Diagonal Detected at" + j + " " + i);
								winner = board[j][i];
								return;
							}
						}
					}
				}
		
			// No connections of length "length"
			winner = 0;
		}
	}
}
