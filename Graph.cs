using System;
using System.Collections.Generic;

namespace WeaverBot
{
    class Graph
    {
        public int[][] Matrix;
        public string[] Words { get; }
        public Graph(string[] words)
        {
            Words = PrepareWords(words);
            Matrix = GenerateMatrix(Words);
        }

        public string[][] PlayWeaver(string startWord, string endWord)
        {
            int startVertex = LookupIndex(startWord);
            int endVertex = LookupIndex(endWord);

            List<int>[] prevVerts = DepthFirstSearch(startVertex, endVertex);
            
            Node root = BackTrackTree(prevVerts, endVertex);
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

        private List<int>[] DepthFirstSearch(int startVertex, int endVertex)
        {
            Queue<int> currentVertexQueue = new Queue<int>();
            Queue<int> nextVertexQueue = new Queue<int>();
            List<int>[] prevVerts = new List<int>[Words.Length];
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
                                if (prevVerts[nextVertex] == null)
                                {
                                    prevVerts[nextVertex] = new List<int>() { vertex };
                                }
                                else
                                {
                                    prevVerts[nextVertex].Add(vertex);
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
            return prevVerts;
        }

        private Node BackTrackTree(List<int>[] prevVerts, int endVertex)
        {
            int currentVertex = endVertex;
            Node root = new Node(endVertex);
            Queue<Node> q = new Queue<Node>();
            q.Enqueue(root);
            while (q.Count > 0)
            {
                Node node = q.Dequeue();
                node.AddChildren(prevVerts[node.Vertex]);
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

        private List<int[]> FindAllPathsFromBackTrackTree(Node root)
        {
            List<int[]> paths = new List<int[]>();
            FindPathFromTree(root, new int[0], paths);
            return paths;
        }

        private void FindPathFromTree(Node node, int[] previousPath, List<int[]> finalPaths)
        {
            int[] currentPath = new int[previousPath.Length + 1];
            Array.Copy(previousPath, currentPath, previousPath.Length);
            currentPath[previousPath.Length] = node.Vertex;

            if (node.Children == null)
            {
                Array.Reverse(currentPath);
                finalPaths.Add(currentPath);
            }
            else
            {
                foreach (Node child in node.Children)
                {
                    FindPathFromTree(child, currentPath, finalPaths);
                }
            }
        }

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