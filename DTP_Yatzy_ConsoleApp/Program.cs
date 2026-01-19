using System.Collections;

namespace DTP_Yatzy_ConsoleApp
{
    internal class Program
    {
        
        static List<int> dice = new List<int>(); //List to hold the rolled dice values
        static void Main(string[] args)
        {
            Console.WriteLine("¤*#---- Simulation Start ----#*¤");



            Console.WriteLine("-Player 1 Roll-");
            for (int i = 1; i <= 5; i++)
            {
                RollDice();
            }
            ConsoleKeyInfo key;
            int option = 1;
            bool isSelected = false;
            (int left, int top) = Console.GetCursorPosition();
            string selected = ">";
            string choice = "*";
            while (!isSelected)
            {
                Console.SetCursorPosition(left, top);

                Console.Write($"{(option == 1 ? selected : "")}{dice[0]}    ");
                Console.Write($"{(option == 2 ? selected : "")}{dice[1]}    ");
                Console.Write($"{(option == 3 ? selected : "")}{dice[2]}    ");
                Console.Write($"{(option == 4 ? selected : "")}{dice[3]}    ");  
                Console.Write($"{(option == 5 ? selected : "")}{dice[4]}\n");
                key = Console.ReadKey(true);


                switch (key.Key)
                {
                    case ConsoleKey.RightArrow:
                        option = (option == 5 ? 1 : option + 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        option = (option == 1 ? 5 : option - 1);
                        break;
                }
            }
            Console.WriteLine($"Rerolled {dice[option-1]}");
        }


        static void RollDice()
        {
                Random roll = new Random();
                dice.Add(roll.Next(1, 7));
        }





        static void PrintScore()
        {
            Console.Write("|[player_name] Score card\r\n|--------------------------\r\n| Ones\t\t\t[i]\r\n| Twos\t\t\t[i]\r\n| Threes\t\t[i]\r\n| Fours\t\t\t[i]\r\n| Fives\t\t\t[i]\r\n| Sixes\t\t\t[i]\r\n| Bonus\t\t\t[X]\r\n|--------------------------\r\n| One Pair\t\t[i]\r\n| Two Pair\t\t[i]\r\n| Three of a Kind\t[i]\r\n| Four of a Kind\t[i]\r\n| Small Straight\t[i]\r\n| Straight\t\t[i]\r\n| Full House\t\t[i]\r\n| Chance\t\t[X]\r\n| YATZY\t\t\t[i]\r\n|==========================\r\n| SUM\t\t\t{X}\r\n---------------------------");
        }
    }
}