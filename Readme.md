# Weaver Bot

A bot to find optimal solutions to [Weaver - A daily word ladder game](https://wordwormdormdork.com/)

# Usage
On a computer with .net 6 sdk installed, navigate to the top level directory of this repository and run the following command:

```sh
dotnet run <start-word> <end-word>
```

# Algorithm

Using the allowed 4 letter words taken from the Weaver source code (stored in `words.txt`), `Graph.cs` creates a graph describing which words can be followed by which other words. Using this graph, a Breadth First Search (BFS) algorithm is executed to until a path is found between the start word and then end word. The BFS algorithm continues until all paths with the same lengths are checked, and the resulting paths are returned. 
