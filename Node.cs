using System;
using System.Collections.Generic;

namespace WeaverBot
{
    class Node
    {
        public int Vertex { get; }
        private Node[]? mChildren = null;
        public Node[]? Children { get { return mChildren; } }
        public Node(int vertex)
        {
            Vertex = vertex;
        }

        public void AddChildren(List<int> vertexList)
        {
            if (vertexList == null) return;
            mChildren = new Node[vertexList.Count];
            for (int i=0; i<Children?.Length; i++)
            {
                Children[i] = new Node(vertexList[i]);
            }
        }
    }
}