using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinMaxAlgorithm
{
    class Program
    {
        private static string currentBoard;
        private static int bestValue = 0;
        private static int bestFrog = 0;
        private static bool isMakingMoves = false;

        static Random rnd = new Random();

        static void Main(string[] args)
        {
            var board = "T.T...F.F";
            int frogsAmount = GetFrogsAmount(board);

            var currentPlayer = rnd.Next(2) == 0 ? 'F' : 'T';

            Console.WriteLine(board);

            while (!IsGameOver(board))
            {
                Console.WriteLine("Current player: " + currentPlayer);
                int frog = 0;
                if (currentPlayer == 'T')
                    frog = int.Parse(Console.ReadLine());
                else
                {
                    bestValue = 0;
                    bestFrog = 0;
                    currentBoard = board;
                    frog = GetBestMove(board, frogsAmount);
                }
                if (MakeAMove(ref board, currentPlayer, frog))
                {
                    if (currentPlayer == 'F')
                        Console.WriteLine(GetAnswer());
                    currentPlayer = GetOpponent(currentPlayer);
                }
            }

            if(CheckWinner(board) == 1)
                Console.WriteLine(GetAnswer());
            Console.WriteLine("Game over. Winner: " + (CheckWinner(board) == 0 ? 'T' : 'F'));
            Console.ReadLine();
        }

        static bool MakeAMove(ref string board, char player, int frog)
        {
            var frogIndex = 0;
            bool validMove = false;

            Console.Clear();

            if (player == 'F')
            {
                if(isMakingMoves == true)
                    board = currentBoard;
                board = ReverseString(board);
            }

            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == player)
                    frogIndex++;

                if (frogIndex == frog)
                {
                    if ((i == board.Length - 1) || (board[i + 1] == player))
                    {
                        if(player == 'T')
                            Console.WriteLine("Ruch niemożliwy");
                        validMove = false;
                        break;
                    }
                    else if (board[i + 1] == '.')
                    {
                        board = board.Remove(i, 2).Insert(i, $".{player}");
                        validMove = true;
                        break;
                    }
                    else
                    {
                        if (i + 3 > board.Length || board[i + 2] != '.')
                        {
                            if(player == 'T')
                                Console.WriteLine("Ruch niemożliwy");
                            validMove = false;
                            break;
                        }
                        board = board.Remove(i, 3).Insert(i, $".{GetOpponent(player)}{player}");
                        validMove = true;
                        break;
                    }
                }
            }

            if (player == 'F')
                board = ReverseString(board);

            Console.WriteLine(board);
            return validMove;
        }

        static int GetBestMove(string board, int frogsAmount)
        {
            isMakingMoves = false;

            for (int frog = 1; frog <= frogsAmount; frog++)
            {
                int value = 0;

                board = currentBoard;

                bestFrog = Minimax(board, 4, 0, frog, frogsAmount, value);
            }

            isMakingMoves = true;
            return bestFrog;
        }

        static int Minimax(string board, int maxDepth, int currentDepth, int frog, int frogsAmount, int value)
        {
            while (currentDepth <= maxDepth)
            {
                if (MakeAMove(ref board, 'F', frog))
                {
                    value++;
                    currentDepth++;
                    return Minimax(board, maxDepth, currentDepth, frog, frogsAmount, value);
                }
                else
                {
                    bestValue = Math.Max(bestValue, value);
                    if (value >= bestValue)
                        bestFrog = frog;
                    return bestFrog;
                }
            }

            bestValue = Math.Max(bestValue, value);
            if (value >= bestValue)
                bestFrog = frog;
            board = currentBoard;
            return bestFrog;
        }

        static bool IsGameOver(string board)
        {
            if (board.StartsWith("FF") || board.EndsWith("TT"))
                return true;
            return false;
        }

        static int CheckWinner(string board)
        {
            if (board.StartsWith("FF"))
                return 0; //przeciwnik
            else if (board.EndsWith("TT"))
                return 1; //gracz
            return -1; //brak zwyciezcy
        }

        static char GetOpponent(char player)
        {
            return player == 'T' ? 'F' : 'T';
        }

        static string ReverseString(string board)
        {
            char[] charsTab = board.ToCharArray();
            Array.Reverse(charsTab);
            return new string(charsTab);
        }

        static int GetFrogsAmount(string board)
        {
            char[] charsTab = board.ToCharArray();
            int frogsAmount = 0;
            for(int i = 0; i < charsTab.Length; i++)
            {
                if (charsTab[i] == 'F')
                    frogsAmount++;
            }
            return frogsAmount;
        }

        static string GetAnswer()
        {
            int randomAns = rnd.Next(4);
            string answer = String.Empty;

            if (!IsGameOver(currentBoard))
            {
                switch (randomAns)
                {
                    case 0:
                        answer = "That was close!";
                        break;
                    case 1:
                        answer = "You almost got me!";
                        break;
                    case 2:
                        answer = "Did I hear you crying?";
                        break;
                    case 3:
                        answer = "Yeah. I can do that.";
                        break;
                }
            }
            else
            {
                if (CheckWinner(currentBoard) == 1)
                {
                    switch (randomAns)
                    {
                        case 0:
                            answer = "Why did you do that?! I was so nice to you!";
                            break;
                        case 1:
                            answer = "Sometimes even the greatest men fall.";
                            break;
                        case 2:
                            answer = "I would've won that if you weren't there!";
                            break;
                        case 3:
                            answer = "You'll never take me alive!";
                            break;
                    }
                }
                else
                {
                    switch(randomAns)
                    {
                        case 0:
                            answer = "I'll chew you like my bubblegum.";
                            break;
                        case 1:
                            answer = "BOOM! That was humiliating. For you, of course.";
                            break;
                        case 2:
                            answer = "Dear Sir, you have been defeated very easily by the most brilliant mind existing in this universe.";
                            break;
                        case 3:
                            answer = "Hey, there are no rewards for crying out loud!";
                            break;
                    }
                }
            }

            return answer;
        }
    }
}