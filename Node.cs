namespace WeaverBot
{
    /// <summary>Tree node representing a vertex in a back track tree</summary>
    internal class Node
    {
        /// <summary>Vertex that this current node represents</summary>
        public int Vertex { get; }
        /// <summary>List of children nodes of this node. null means no children or no children defined yet.</summary>
        private Node[]? mChildren = null;
        /// <summary>List of children nodes of this node. null means no children or no children defined yet.</summary>
        public Node[]? Children { get { return mChildren; } }
        /// <summary>Create node from vertex</summary>
        public Node(int vertex)
        {
            Vertex = vertex;
        }

        /// <summary>Create children nodes from list of vertices</summary>
        /// <param name="vertexList">List of vertices to be added as children nodes</summary>
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