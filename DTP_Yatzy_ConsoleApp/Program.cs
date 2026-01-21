using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DTP_Yatzy_ConsoleApp
{
    public class Program
    {
        static List<int> dice = new List<int>(); //List to hold the rolled dice values

        static List<string> players = new List<string>();


        static void Main(string[] args)
        {
            Console.WriteLine("How many players will we be playing with today?");
            int playerCount = 0;
            if (!int.TryParse(Console.ReadLine(), out playerCount) || playerCount < 1)
            {
                Console.WriteLine("(!) Please enter a valid number.\n");
                Main(args);
            }

            for (int i = 1; i <= playerCount; i++)
            {
                Console.Write($"Enter Player {i} name: ");
                EnterName:
                string n = Console.ReadLine();
                string playerName = n.Substring(n.IndexOf(": ") + 1);
                
                if (playerName.IsWhiteSpace())
                {
                    Console.WriteLine("(!) Please enter a valid name.");
                    goto EnterName;
                }
                players.Add(playerName);
            }
            Console.Clear();
            var scoreCards = new List<Dictionary<string, int?>>();
            for (int i = 0; i < playerCount; i++)
            {
                scoreCards.Add(InitScoreValues());
            }

            for (int round = 1; round <= 13; round++)
            {
                Console.WriteLine($"***-----< Round {round}! >-----***\n");
                foreach (string player in players)
                {
                    Console.WriteLine($"<<-------(Player {players.IndexOf(player) + 1}){player}, your turn! ------->>\n");
                    for (int i = 1; i <= 5; i++)
                    {
                        RollDice();
                    }
                    Console.WriteLine("\tvvv Your roll vvv");
                    int rerolls = 0;
                    while (rerolls < 2)
                    {
                        rerolls = RerollSelection(rerolls);
                    }
                    Console.Clear();
                    ScoreSelection(player, scoreCards[players.IndexOf(player)], dice);
                    dice.Clear();
                    Console.Clear();
                }
            }
        }
        static void RollDice()
        {
            Random rng = new Random();
            dice.Add(rng.Next(1, 7));
        }

        static int RerollSelection(int rerolls)
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
                Console.WriteLine("(?) Which dice would you like to reroll? Press [Space] to select a die.\n(?) Press [Enter] to confirm your selection and reroll them!\n");
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
            if (selected[option] == true)
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
                return rerolls;
            }
            else
            {
                rerolls += 2; // skip remaining rerolls if none selected
                return rerolls;
            }
        }
        static void ScoreSelection(string playerName, Dictionary<string, int?> scoreValues, List<int> dice)
        {
            ConsoleKeyInfo key;
            int option = 0;
            bool isConfirmed = false;
            (int right, int top) = Console.GetCursorPosition();
            string cursor = "<";

            while (!isConfirmed)
            {
                Console.SetCursorPosition(right, top);

                Console.WriteLine("------------~+~------------");
                Console.WriteLine($"|\t{playerName} Score card");
                Console.WriteLine($"|--------------------------");
                Console.WriteLine($"| Ones\t\t\t[{scoreValues["Ones"]}]{(option == 0 ? cursor : " ")}");
                Console.WriteLine($"| Twos\t\t\t[{scoreValues["Twos"]}]{(option == 1 ? cursor : " ")}");
                Console.WriteLine($"| Threes\t\t[{scoreValues["Threes"]}]{(option == 2 ? cursor : " ")}");
                Console.WriteLine($"| Fours\t\t\t[{scoreValues["Fours"]}]{(option == 3 ? cursor : " ")}");
                Console.WriteLine($"| Fives\t\t\t[{scoreValues["Fives"]}]{(option == 4 ? cursor : " ")}");
                Console.WriteLine($"| Sixes\t\t\t[{scoreValues["Sixes"]}]{(option == 5 ? cursor : " ")}");
                if (scoreValues["Ones"] + scoreValues["Twos"] + scoreValues["Threes"] + scoreValues["Fours"] + scoreValues["Fives"] + scoreValues["Sixes"] >= 63)
                {
                    scoreValues["Bonus"] = 50;
                    Console.WriteLine($"| Bonus\t\t\t{scoreValues["Bonus"]}");
                }
                else
                {
                    scoreValues["Bonus"] = 0;
                    Console.WriteLine($"| Bonus\t\t\t{scoreValues["Bonus"]}");
                }
                Console.WriteLine($"|--------------------------");
                Console.WriteLine($"| Pair\t\t\t[{scoreValues["Pair"]}]{(option == 6 ? cursor : " ")}");
                Console.WriteLine($"| Two Pair\t\t[{scoreValues["TwoPair"]}]{(option == 7 ? cursor : " ")}");
                Console.WriteLine($"| Three of a Kind\t[{scoreValues["ThreeOfAKind"]}]{(option == 8 ? cursor : " ")}");
                Console.WriteLine($"| Four of a Kind\t[{scoreValues["FourOfAKind"]}]{(option == 9 ? cursor : " ")}");
                Console.WriteLine($"| Small Straight\t[{scoreValues["SmallStraight"]}]{(option == 10 ? cursor : " ")}");
                Console.WriteLine($"| Straight\t\t[{scoreValues["Straight"]}]{(option == 11 ? cursor : " ")}");
                Console.WriteLine($"| Full House\t\t[{scoreValues["FullHouse"]}]{(option == 12 ? cursor : " ")}");
                Console.WriteLine($"| Chance\t\t[{scoreValues["Chance"]}]{(option == 13 ? cursor : " ")}");
                Console.WriteLine($"| Yatzy\t\t\t[{scoreValues["Yatzy"]}]{(option == 14 ? cursor : " ")}");
                Console.WriteLine($"|==========================");
                Console.WriteLine($"| SUM\t\t\t{scoreValues.Values.Sum()}");
                Console.WriteLine("---------------------------");
                WriteRollValues(dice);
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                        option = (option == 14 ? 0 : option + 1);
                        break;
                    case ConsoleKey.UpArrow:
                        option = (option == 0 ? 14 : option - 1);
                        break;
                    case ConsoleKey.Enter:
                        isConfirmed = true;
                        break;
                }
                // move cursor up one line so next render overwrites the same row
                Console.SetCursorPosition(right, top);
            }
            CalculateScore(playerName, scoreValues, option, dice);
        }
        static void CalculateScore(string playerName, Dictionary<string, int?> scoreValues, int option, List<int> dice)
        {
            int result = 0;
            switch (option)
            {
                case 0:
                    if (IsSpaceFilled(scoreValues, "Ones"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    if (dice.Contains(1))
                    {
                        foreach (int number in dice)
                        {
                            if (number == 1)
                            {
                                result += number;
                            }
                        }
                        scoreValues["Ones"] = result;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;

                case 1:
                    if (IsSpaceFilled(scoreValues, "Twos"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    if (dice.Contains(2))
                    {
                        foreach (int number in dice)
                        {
                            if (number == 2)
                            {
                                result += number;
                            }
                        }
                        scoreValues["Twos"] = result;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;

                case 2:
                    if (IsSpaceFilled(scoreValues, "Threes"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    if (dice.Contains(3))
                    {
                        foreach (int number in dice)
                        {
                            if (number == 3)
                            {
                                result += number;
                            }
                        }
                        scoreValues["Threes"] = result;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;

                case 3:
                    if (IsSpaceFilled(scoreValues, "Fours"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    if (dice.Contains(4))
                    {
                        foreach (int number in dice)
                        {
                            if (number == 4)
                            {
                                result += number;
                            }
                        }
                        scoreValues["Fours"] = result;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;

                case 4:
                    if (IsSpaceFilled(scoreValues, "Fives"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    if (dice.Contains(5))
                    {
                        foreach (int number in dice)
                        {
                            if (number == 5)
                            {
                                result += number;
                            }
                        }
                        scoreValues["Fives"] = result;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;
                case 5:
                    if (IsSpaceFilled(scoreValues, "Sixes"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    if (dice.Contains(6))
                    {
                        foreach (int number in dice)
                        {
                            if (number == 6)
                            {
                                result += number;
                            }
                        }
                        scoreValues["Sixes"] = result;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;
                case 6:
                    if (IsSpaceFilled(scoreValues, "Pair"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    dice.Sort();
                    dice.Reverse();
                    for (int i = 0; i < dice.Count - 1; i++)
                    {
                        if (dice[i] == dice[i + 1])
                        {
                            result = dice[i] * 2;
                            break;
                        }
                    }
                    if (result == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    scoreValues["Pair"] = result;
                    Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    break;

                case 7:
                    if (IsSpaceFilled(scoreValues, "TwoPair"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    int pair1 = 0;
                    int pair2 = 0;
                    dice.Sort();
                    dice.Reverse();
                    for (int i = 0; i < dice.Count - 1; i++)
                    {
                        if (dice[i] == dice[i + 1])
                        {
                            // prevents having 2 pairs of the same number
                            if (pair1 != 0 && dice[i] != pair1)
                            {
                                result += dice[i] * 2;
                                pair2 = dice[i];
                                break;
                            }
                            else if (pair1 == 0)
                            {
                                result += dice[i] * 2;
                                pair1 = dice[i];
                            }
                            i += 1; // skip next index since it's part of the pair
                        }
                    }
                    if (pair1 != 0 && pair2 != 0)
                    {
                        scoreValues["TwoPair"] = result;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;

                case 8:
                    if (IsSpaceFilled(scoreValues, "ThreeOfAKind"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    dice.Sort();
                    dice.Reverse();
                    for (int i = 0; i < dice.Count - 1; i++)
                    {
                        if (dice[i] == dice[i + 1] && dice[i + 1] == dice[i + 2])
                        {
                            result = dice[i] * 3;
                            break;
                        }
                    }
                    if (result == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    scoreValues["ThreeOfAKind"] = result;
                    Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    break;

                case 9:
                    if (IsSpaceFilled(scoreValues, "FourOfAKind"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    dice.Sort();
                    dice.Reverse();
                    for (int i = 0; i < dice.Count - 1; i++)
                    {
                        if (dice[i] == dice[i + 1] && dice[i + 1] == dice[i + 2] && dice[i + 2] == dice[i + 3])
                        {
                            result = dice[i] * 4;
                            break;
                        }
                    }
                    if (result == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    scoreValues["FourOfAKind"] = result;
                    Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    break;

                case 10:
                    if (IsSpaceFilled(scoreValues, "SmallStraight"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    if (dice.Contains(1) && dice.Contains(2) && dice.Contains(3) && dice.Contains(4) && dice.Contains(5))
                    {
                        scoreValues["SmallStraight"] = 15;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;

                case 11:
                    if (IsSpaceFilled(scoreValues, "Straight"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    if (dice.Contains(2) && dice.Contains(3) && dice.Contains(4) && dice.Contains(5) && dice.Contains(6))
                    {
                        scoreValues["Straight"] = 20;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    break;

                case 12:
                    if (IsSpaceFilled(scoreValues, "FullHouse"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    dice.Sort();
                    dice.Reverse();
                    int j = 0;
                    if (dice[j] == dice[j + 1]) //check if 2 are the same
                    {
                        if (dice[j] == dice[j + 2]) //if so, check is next is also the same
                        {
                            result += dice[j] * 3;
                            j += 2;
                            if (dice[j] == dice[j + 1]) //then check the last 2
                            {
                                result += dice[j] * 2;
                                scoreValues["FullHouse"] = result;
                                Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                                break;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("(!) You can't do that.");
                                ScoreSelection(playerName, scoreValues, dice);
                            }
                        }
                        else //otherwise...
                        {
                            j += 2;
                            if (dice[j] == dice[j + 1] && dice[j] == dice[j + 2]) //check if the last 3 are the same
                            {
                                result += dice[j] * 3;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("(!) You can't do that.");
                                ScoreSelection(playerName, scoreValues, dice);
                            }
                        }
                    }
                    break;

                case 13:
                    if (IsSpaceFilled(scoreValues, "Chance"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    foreach (int number in dice)
                    {
                        result += number;
                    }
                    scoreValues["Chance"] = result;
                    Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    break;

                case 14:
                    if (IsSpaceFilled(scoreValues, "Yatzy"))
                    {
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    for (int i = 0; i < dice.Count - 1; i++)
                    {
                        if (dice[0] != dice[i])
                        {
                            break;
                        }
                    }
                    if (result == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("(!) You can't do that.");
                        ScoreSelection(playerName, scoreValues, dice);
                    }
                    else
                    {
                        scoreValues["Yatzy"] = 50;
                        Console.WriteLine(WriteScoreCard(playerName, scoreValues));
                    }
                    break;
            }
        }
        static Dictionary<string, int?> InitScoreValues()
        {
            Dictionary<string, int?> scoreValues = new Dictionary<string, int?>()
            {
                ["Ones"] = null,
                ["Twos"] = null,
                ["Threes"] = null,
                ["Fours"] = null,
                ["Fives"] = null,
                ["Sixes"] = null,
                ["Bonus"] = null,
                ["Pair"] = null,
                ["TwoPair"] = null,
                ["ThreeOfAKind"] = null,
                ["FourOfAKind"] = null,
                ["SmallStraight"] = null,
                ["Straight"] = null,
                ["FullHouse"] = null,
                ["Chance"] = null,
                ["Yatzy"] = null
            };
            return scoreValues;
        }

        static bool IsSpaceFilled(Dictionary<string, int?> scoreValues, string scoreType)
        {
            if (scoreValues[scoreType] != null)
            {
                Console.Clear();
                Console.WriteLine("(!) Space is already filled (Cannot fill space twice).");
                return true;
            }
            else
            {
                return false;
            }
        }

        static void WriteRollValues(List<int> dice)
        {
            Console.WriteLine("vvv   Your roll   vvv");
            for (int i = 0; i < 5; i++)
            {
                Console.Write($"{dice[i]}    ");
            }
            Console.WriteLine();
        }
        static string WriteScoreCard(string playerName, Dictionary<string, int?> scoreValues)
        {
            Console.Clear();
            return $"------------~+~------------\r\n|       {playerName} Score card\r\n|--------------------------\r\n| Ones                  [{scoreValues["Ones"]}]\r\n| Twos                  [{scoreValues["Twos"]}]\r\n| Threes                [{scoreValues["Threes"]}]\r\n| Fours                 [{scoreValues["Fours"]}]\r\n| Fives                 [{scoreValues["Fives"]}]\r\n| Sixes                 [{scoreValues["Sixes"]}]\r\n| Bonus                 [{scoreValues["Bonus"]}]\r\n|--------------------------\r\n| One Pair              [{scoreValues["Pair"]}]\r\n| Two Pair              [{scoreValues["TwoPair"]}]\r\n| Three of a Kind       [{scoreValues["ThreeOfAKind"]}]\r\n| Four of a Kind        [{scoreValues["FourOfAKind"]}]\r\n| Small Straight        [{scoreValues["SmallStraight"]}]\r\n| Straight              [{scoreValues["Straight"]}]\r\n| Full House            [{scoreValues["FullHouse"]}]\r\n| Chance                [{scoreValues["Chance"]}]\r\n| Yatzy                 [{scoreValues["Yatzy"]}]\r\n|==========================\r\n| SUM                   [{scoreValues.Values.Sum()}]\r\n---------------------------";
        }
    }
}