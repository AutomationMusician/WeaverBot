using System;

namespace WeaverBot
{
    class Program {         
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Expected two parameters");
            }
            string[] words = LoadWords();
            Graph graph = new Graph(words);
            string[][] answers = graph.PlayWeaver(args[0].ToLower().Trim(), args[1].ToLower().Trim());
            PrintAnswers(answers);
        }

        private static string[] LoadWords()
        {
            return File.ReadAllLines("./words.txt");
        }
        
        private static void PrintAnswers(string[][] answers)
        {
            for (int i=0; i<answers.Length; i++)
            {
                Console.WriteLine($"Optimal path {i+1}");
                foreach (string word in answers[i])
                {
                    Console.WriteLine(word);
                }
                Console.WriteLine();
            }
        }
    }
}