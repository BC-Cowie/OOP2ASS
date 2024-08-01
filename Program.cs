using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGames
{
    public class Die
    {
        private static readonly Random random = new Random();

        public int Roll()
        {
            // Generates a number between 1 and 13, inclusive
            return random.Next(1, 14);
        }
    }

    public class Game
    {
        private readonly IStatistics statistics;

        public Game(IStatistics stats)
        {
            statistics = stats;
        }

        public void PlaySevensOut()
        {
            SevensOut game = new SevensOut();
            game.Play();
            statistics.UpdateSevensOut(game.Score);
        }

        public void PlayThreeOrMore()
        {
            ThreeOrMore game = new ThreeOrMore();
            game.Play();
            statistics.UpdateThreeOrMore(game.Score);
        }

        public void ViewStatistics()
        {
            statistics.Display();
        }
    }

    public class SevensOut
    {
        private readonly Die die1;
        private readonly Die die2;
        public int Score { get; private set; }

        public SevensOut()
        {
            die1 = new Die();
            die2 = new Die();
            Score = 0;
        }

        public void Play()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Press Enter to roll the dice or 'E' to exit...");
                    var input = Console.ReadLine()?.Trim().ToUpper();

                    if (input == "E")
                    {
                        Console.WriteLine("Exiting Sevens Out...");
                        break;
                    }

                    int roll1 = die1.Roll();
                    int roll2 = die2.Roll();
                    int total = roll1 + roll2;

                    Console.WriteLine($"Roll: {GetDisplayValue(roll1)} + {GetDisplayValue(roll2)} = {total} (score = {Score})");

                    if (total == 7)
                    {
                        Console.WriteLine("You hit 7...WINNER!!!");
                        break;
                    }
                    else if (roll1 == roll2)
                    {
                        Score += total * 2;
                    }
                    else
                    {
                        Score += total;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR OCCURRED SEVENS OUT: {ex.Message}");
            }
        }

        private string GetDisplayValue(int roll)
        {
            return roll switch
            {
                1 => "A",
                11 => "J",
                12 => "Q",
                13 => "K",
                _ => roll.ToString(),
            };
        }
    }

    public class ThreeOrMore
    {
        private readonly Die[] dice;
        public int Score { get; private set; }

        public ThreeOrMore()
        {
            dice = new Die[5];
            for (int i = 0; i < 5; i++)
            {
                dice[i] = new Die();
            }
            Score = 0;
        }

        public void Play()
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    int[] values = new int[5];
                    for (int j = 0; j < 5; j++)
                    {
                        Console.WriteLine($"Press Enter to roll die {j + 1} or 'E' to exit...");
                        var input = Console.ReadLine()?.Trim().ToUpper();

                        if (input == "E")
                        {
                            Console.WriteLine("Exiting Three or More...");
                            return;
                        }

                        values[j] = dice[j].Roll();
                    }
                    Console.WriteLine($"Roll: {string.Join(", ", values.Select(v => GetDisplayValue(v)))}");

                    var counts = GetCounts(values);
                    bool foundTripleOrMore = false;

                    foreach (var count in counts)
                    {
                        if (count.Value >= 3)
                        {
                            if (count.Value == 3)
                                Score += 3;
                            else if (count.Value == 4)
                                Score += 6;
                            else if (count.Value == 5)
                                Score += 12;

                            values = values.Where(x => x != count.Key).ToArray();
                            foundTripleOrMore = true;
                            break;
                        }
                    }

                    if (!foundTripleOrMore)
                        continue;

                    if (values.Length > 0)
                    {
                        Console.WriteLine("You have a chance to score more points.");
                        Console.WriteLine("Press Enter to roll the remaining dice or 'E' to exit...");
                        var input = Console.ReadLine()?.Trim().ToUpper();

                        if (input == "E")
                        {
                            Console.WriteLine("Exiting Three or More...");
                            return;
                        }

                        for (int j = 0; j < values.Length; j++)
                        {
                            values[j] = dice[j].Roll();
                        }
                        Console.WriteLine($"Roll: {string.Join(", ", values.Select(v => GetDisplayValue(v)))}");

                        counts = GetCounts(values);
                        foreach (var count in counts)
                        {
                            if (count.Value >= 3)
                            {
                                if (count.Value == 3)
                                    Score += 3;
                                else if (count.Value == 4)
                                    Score += 6;
                                else if (count.Value == 5)
                                    Score += 12;

                                foundTripleOrMore = true;
                                break;
                            }
                        }

                        if (!foundTripleOrMore)
                        {
                            foreach (var value in values)
                            {
                                if (value == 1)
                                    Score += 10;
                                else if (value == 5)
                                    Score += 5;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR OCCURRED THREE OR MORE: {ex.Message}");
            }
        }

        private Dictionary<int, int> GetCounts(int[] values)
        {
            var counts = new Dictionary<int, int>();

            foreach (var value in values)
            {
                if (counts.ContainsKey(value))
                    counts[value]++;
                else
                    counts[value] = 1;
            }

            return counts;
        }

        private string GetDisplayValue(int roll)
        {
            return roll switch
            {
                1 => "A",
                11 => "J",
                12 => "Q",
                13 => "K",
                _ => roll.ToString(),
            };
        }
    }

    public interface IStatistics
    {
        void UpdateSevensOut(int score);
        void UpdateThreeOrMore(int score);
        void Display();
    }

    public class Statistics : IStatistics
    {
        private int sevensOutScore;
        private int threeOrMoreScore;

        public void UpdateSevensOut(int score)
        {
            sevensOutScore += score;
        }

        public void UpdateThreeOrMore(int score)
        {
            threeOrMoreScore += score;
        }

        public void Display()
        {
            Console.WriteLine("Statistics:");
            Console.WriteLine($"Sevens Out: {sevensOutScore}");
            Console.WriteLine($"Three or More: {threeOrMoreScore}");
        }
    }

    public class Testing
    {
        private readonly Game game;

        public Testing()
        {
            IStatistics stats = new Statistics();
            game = new Game(stats);
        }

        public void TestSevensOut()
        {
            SevensOut game = new SevensOut();
            game.Play();
            Assert(game.Score >= 0, "Sevens out: Score should be non-negative");
        }

        public void TestThreeOrMore()
        {
            ThreeOrMore game = new ThreeOrMore();
            game.Play();
            Assert(game.Score >= 0, "Three or More: Score should be non-negative after play");
        }

        public void RunTests()
        {
            TestSevensOut();
            TestThreeOrMore();
            Console.WriteLine("All tests passed!");
        }

        private void Assert(bool condition, string message)
        {
            if (!condition)
                throw new Exception(message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IStatistics stats = new Statistics();
            Game game = new Game(stats);

            while (true)
            {
                Console.WriteLine("Choose a game:");
                Console.WriteLine("1. Sevens Out");
                Console.WriteLine("2. Three or More");
                Console.WriteLine("3. View Statistics");
                Console.WriteLine("4. Run Tests");
                Console.WriteLine("5. Quit");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 5)
                {
                    Console.WriteLine("Invalid choice. Please choose again.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            game.PlaySevensOut();
                            break;
                        case 2:
                            game.PlayThreeOrMore();
                            break;
                        case 3:
                            game.ViewStatistics();
                            break;
                        case 4:
                            Testing testing = new Testing();
                            testing.RunTests();
                            break;
                        case 5:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
            }
        }
    }
}


