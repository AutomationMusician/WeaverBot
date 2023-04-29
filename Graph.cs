namespace WeaverBot
{
    /// <summary>Represents a graph of the adjacent words in weaver game</summary>
    public class Graph
    {
        ///<summary>An adjacency matrix for the graph</summary>
        private int[][] Matrix { get; }
        ///<summary>A list of valid weaver words</summary>
        private string[] Words { get; }

        /// <summary>Graph constructor</summary>
        /// <param name="words">an array of words to use to generate the array</param>
        public Graph(string[] words)
        {
            Words = PrepareWords(words);
            Matrix = GenerateMatrix(Words);
        }

        /// <summary>Produce the a list of answers for a weaver game given the starting and ending words</summary>
        /// <returns>an array of answers, which is an array of strings representing the words to play in the weaver game</returns>
        public string[][] PlayWeaver(string startWord, string endWord)
        {
            int startVertex = LookupIndex(startWord);
            int endVertex = LookupIndex(endWord);

            List<int>[] prevVertices = BreadthFirstSearch(startVertex, endVertex);
            
            Node root = CreateBackTrackTree(prevVertices, endVertex);
            List<int[]> vertexPathsList = FindAllPathsFromBackTrackTree(root);
            return VertexPathsToWordsPaths(vertexPathsList);
        }

        /// <summary>Validate, clone, sort, and return input words array</summary>
        private string[] PrepareWords(string[] words)
        {
            ValidateArray(words);
            string[] clone = new string[words.Length];
            Array.Copy(words, clone, words.Length);
            Array.Sort(clone);
            return clone;
        }

        /// <summary>Check that all words have the same length and that there is at least one word in the array</summary>
        private static void ValidateArray(string[] words)
        {
            if (words.Length < 1)
                throw new ArgumentException($"{nameof(words)} array must have at least one element in it");
            int numCharacters = words[0].Length;
            for (int i=1; i<words.Length; i++)
            {
                if (words[i].Length != numCharacters)
                {
                    throw new ArgumentException($"{nameof(words)} array contains words with lengths of {numCharacters} and {words[i].Length}. All characters must be the same length.");
                }
            }
        }

        /// <summary>Generate matrix from list of words</summary>
        /// <param name="words">list of valid weaver words</param>
        /// <returns>adjacency matrix</returns>
        private int[][] GenerateMatrix(string[] words)
        {
            List<int>[] dynamicMatrix = new List<int>[Words.Length];

            for (int i=0; i<Words.Length; i++)
            {
                string currentWord = words[i];
                dynamicMatrix[i] = new List<int>();
                for (int j=0; j<i; j++)
                {
                    string existingWord = words[j];
                    if (IsAdjacent(currentWord, existingWord))
                    {
                        dynamicMatrix[i].Add(j);
                        dynamicMatrix[j].Add(i);
                    }
                }
            }

            int[][] fixedMatrix = new int[words.Length][];
            for (int i=0; i<Words.Length; i++)
            {
                fixedMatrix[i] = dynamicMatrix[i].ToArray();
            }
            return fixedMatrix;
        }

        /// <summary>Checks that two words can be used one after another in a weaver game.
        /// <return>true if adjacent, false if not</return>
        private static bool IsAdjacent(string word1, string word2)
        {
            int numDifferent = 0;
            for (int i=0; numDifferent <= 1 && i<word1.Length; i++)
            {
                if (word1[i] != word2[i])
                    numDifferent++;
            }
            return (numDifferent == 1);  
        }

        /// <summary>Binary Search for index of word</summary>
        /// <param name="word">word to search for</param>
        /// <returns>index of word in Words array</returns>
        private int LookupIndex(string word)
        {
            int minIndex = 0;
            int maxIndex = Words.Length - 1;

            while (minIndex <= maxIndex) {
                int midIndex = (minIndex + maxIndex) / 2;
                if (word == Words[midIndex])
                {
                    return midIndex;
                }
                else if (string.Compare(word, Words[midIndex]) < 0)
                {
                    maxIndex = midIndex - 1;
                }
                else {
                    minIndex = midIndex + 1;
                }
            }
            throw new ArgumentException($"{word} does not exist in word list");
        }

        /// <summary>Breadth First Search (BFS) of the adjacency matrix, starting from the startVertex and ending at the endVertex</summary>
        /// <return>A array of lists of integers, where the index of the array represents the vertex, and the index of the list represents which vertices preceded the current vertex.</return>
        private List<int>[] BreadthFirstSearch(int startVertex, int endVertex)
        {
            Queue<int> currentVertexQueue = new Queue<int>();
            Queue<int> nextVertexQueue = new Queue<int>();
            List<int>[] prevVertices = new List<int>[Words.Length];
            bool[] visited = new bool[Words.Length];
            nextVertexQueue.Enqueue(startVertex);
            visited[startVertex] = true;
            bool answerFound = false;

            bool[] newlyVisited = new bool[Words.Length];

            while (!answerFound && nextVertexQueue.Count > 0)
            {
                currentVertexQueue = nextVertexQueue;
                nextVertexQueue = new Queue<int>();

                while (currentVertexQueue.Count > 0)
                {
                    int vertex = currentVertexQueue.Dequeue();
                    if (vertex == endVertex)
                    {
                        answerFound = true;
                    }
                    else
                    {
                        foreach (int nextVertex in Matrix[vertex])
                        {
                            if (!visited[nextVertex])
                            {
                                if (prevVertices[nextVertex] == null)
                                {
                                    prevVertices[nextVertex] = new List<int>() { vertex };
                                }
                                else
                                {
                                    prevVertices[nextVertex].Add(vertex);
                                }

                                if (!newlyVisited[nextVertex])
                                {
                                    nextVertexQueue.Enqueue(nextVertex);
                                    newlyVisited[nextVertex] = true;
                                }
                            }
                        }
                    }
                }

                for (int i=0; i<Words.Length; i++)
                {
                    if (newlyVisited[i]) {
                        visited[i] = true;
                        newlyVisited[i] = false;
                    }
                }
            }
            return prevVertices;
        }

        /// <summary>Create a tree from the BreadFirstSearch array of lists (prevVertices) starting from the endVertex and backtracking to the startVertex (the vertex with no previous vertices)</summary>
        /// <param name="prevVertices">The result of the BreadthFirstSearch method</param>
        /// <param name="endVertex">vertex representing the end word</param>
        /// <returns>The root node of a tree describing the various paths from the endVertex to the startVertex. The root node represents the endVertex node.</returns>
        private Node CreateBackTrackTree(List<int>[] prevVertices, int endVertex)
        {
            int currentVertex = endVertex;
            Node root = new Node(endVertex);
            Queue<Node> q = new Queue<Node>();
            q.Enqueue(root);
            while (q.Count > 0)
            {
                Node node = q.Dequeue();
                node.AddChildren(prevVertices[node.Vertex]);
                if (node.Children != null)
                {
                    foreach (Node child in node.Children)
                    {
                        q.Enqueue(child);
                    }
                }
            }
            return root;
        }

        /// <summary>Creates list of arrays representing paths from the start vertex to the end vertex given the root Node of the back track tree produced by the CreateBackTrackTree method. Calls recursive FindPathFromTree method with starting parameters.</summary>
        /// <param name="root">root of back track tree</param>
        /// <returns>List of paths through the back track tree</returns>
        private List<int[]> FindAllPathsFromBackTrackTree(Node root)
        {
            List<int[]> paths = new List<int[]>();
            FindPathFromTree(root, new int[0], paths);
            return paths;
        }

        /// <summary>Recursive function to generate all the paths of the tree from the root to the leaf nodes</summary>
        /// <param name="node">current node</param>
        /// <param name="previousPath">path from root to the parent of the current node</param>
        /// <param name="completePaths">List to which complete paths are added</param>
        private void FindPathFromTree(Node node, int[] previousPath, List<int[]> completePaths)
        {
            int[] currentPath = new int[previousPath.Length + 1];
            Array.Copy(previousPath, currentPath, previousPath.Length);
            currentPath[previousPath.Length] = node.Vertex;

            if (node.Children == null)
            {
                Array.Reverse(currentPath);
                completePaths.Add(currentPath);
            }
            else
            {
                foreach (Node child in node.Children)
                {
                    FindPathFromTree(child, currentPath, completePaths);
                }
            }
        }

        /// <summary>Convert a list of paths of vertices (ints) to an array of paths of words (strings)</summary>
        /// <param name="vertexPaths">List of paths of vertices (ints)</param>
        /// <returns>array of paths of words</returns>
        private string[][] VertexPathsToWordsPaths(List<int[]> vertexPaths)
        {
            string[][] wordsPaths = new string[vertexPaths.Count][];
            for (int i=0; i<vertexPaths.Count; i++)
            {
                wordsPaths[i] = new string[vertexPaths[i].Length];
                for (int j=0; j<wordsPaths[i].Length; j++)
                {
                    wordsPaths[i][j] = Words[vertexPaths[i][j]];
                }
            }
            return wordsPaths;
        }
    }
}