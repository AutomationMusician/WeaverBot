namespace WeaverBot
{
    /// <summary>WeaverBot entrypoint class</summary>
    class Program
    {
        /// <summary>WeaverBot entrypoint method</summary>
        static void Main(string[] args)
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

        /// <summary>Load words from local file</summary>
        /// <returns>array of valid words</returns>
        private static string[] LoadWords()
        {
            return File.ReadAllLines("./words.txt");
        }
        
        /// <summary>Print the answers of a weaver game to the console</summary>
        /// <param name="answers">answers for a weaver game</param>
        private static void PrintAnswers(string[][] answers)
        {
            for (int i=0; i<answers.Length; i++)
            {
                Console.WriteLine($"Optimal path {i+1}: {string.Join(", ", answers[i])}");
            }
        }
    }
}