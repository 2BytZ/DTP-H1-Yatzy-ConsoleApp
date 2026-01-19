using System;
using System.Collections;
using System.Collections.Generic;

namespace DTP_Yatzy_ConsoleApp
{
    internal class Program
    {
        static int rerolls = 0;
        static List<int> dice = new List<int>(); //List to hold the rolled dice values
        static Random rng = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("¤*#---- Simulation Start ----#*¤");

            Console.WriteLine("-Player 1 Roll-");
            for (int i = 1; i <= 5; i++)
            {
                RollDice();
            }
            do
            {
                ConsoleKeyInfo key;
                int option = 0;
                bool isConfirmed = false;
                (int left, int top) = Console.GetCursorPosition();
                string cursor = ">";
                string choiceMark = "*";
                bool[] selected = new bool[5];

                while (!isConfirmed)
                {
                    Console.SetCursorPosition(left, top);

                    // Render all five dice in a single row, showing cursor and selection marks
                    for (int i = 0; i < 5; i++)
                    {
                        string cursorMark = (option == i) ? cursor : " ";
                        string selMark = selected[i] ? choiceMark : " ";
                        // fixed width items so we overwrite previous content reliably
                        Console.Write($"{cursorMark}{selMark}{dice[i]}    ");
                    }
                    Console.WriteLine();
                    key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.RightArrow:
                            option = (option == 4 ? 0 : option + 1);
                            break;
                        case ConsoleKey.LeftArrow:
                            option = (option == 0 ? 4 : option - 1);
                            break;
                        case ConsoleKey.Spacebar:
                            // toggle selection for the current option
                            selected[option] = !selected[option];
                            break;
                        case ConsoleKey.Enter:
                            // confirm selections
                            isConfirmed = true;
                            break;
                    }

                    // move cursor up one line so next render overwrites the same row
                    Console.SetCursorPosition(left, top);
                }
                if (selected.Length != 0)
                {
                    rerolls += 1;
                    foreach (bool number in selected)
                    {
                        if (number)
                        {
                            dice.RemoveAt(Array.IndexOf(selected, number));
                            RollDice();
                        }
                    }
                }
            }
            while (rerolls < 2);

            static void RollDice()
            {
                dice.Add(rng.Next(1, 7));
            }

            static void PrintScore()
            {
                Console.Write("|[player_name] Score card\r\n|--------------------------\r\n| Ones\t\t\t[i]\r\n| Twos\t\t\t[i]\r\n| Threes\t\t[i]\r\n| Fours\t\t\t[i]\r\n| Fives\t\t\t[i]\r\n| Sixes\t\t\t[i]\r\n| Bonus\t\t\t[X]\r\n|--------------------------\r\n| One Pair\t\t[i]\r\n| Two Pair\t\t[i]\r\n| Three of a Kind\t[i]\r\n| Four of a Kind\t[i]\r\n| Small Straight\t[i]\r\n| Straight\t\t[i]\r\n| Full House\t\t[i]\r\n| Chance\t\t[X]\r\n| YATZY\t\t\t[i]\r\n|==========================\r\n| SUM\t\t\t{X}\r\n---------------------------");
            }
        }
    }
}