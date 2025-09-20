# Console Snake Multiplayer

This project is a modern implementation of the classic Snake game in a .NET console application. It features a complex, multi-snake environment where player-controlled snakes can compete against intelligent AI opponents. The core focus is on robust software architecture and advanced algorithm implementation, making it a powerful demonstration of C# and software design principles.



## Key Features

* **Intelligent Snake AI**: The `SnakeBot` uses a weighted **A\* pathfinding algorithm** to dynamically calculate the most efficient route to the nearest apple.
* **Strategic Decision Making**: The AI evaluates all potential targets, prioritizing apples with the shortest *actual path distance*. As a tie-breaker, it assesses the "safety" of a target by measuring the open space around it.
* **Asynchronous & Parallel Processing**: The game loop is built with modern C# `async/await` features to handle game logic, AI calculations, and collision detection for multiple snakes in parallel. This ensures a smooth, non-blocking gameplay experience even with numerous AI bots.
* **Dynamic & Extensible Map System**: Game maps are loaded from simple `.txt` files, allowing for easy creation of new and complex levels. The `MapLoader` is designed with robust parsing and error handling to ensure map integrity.
* **Object-Oriented Design**: The project follows strong OOP principles with a clear separation of concerns. An `ISnake` interface allows for different snake implementations, making the system highly modular and easy to extend.

## Technical Stack

* **Language**: C# 13
* **Framework**: .NET 9.0
* **Core Features**:
    * Asynchronous Programming (`async`/`await`, `Task`)
    * LINQ for complex data manipulation
    * Advanced Data Structures (`PriorityQueue`, `HashSet`, `Dictionary`)
    * Object-Oriented Programming (Interfaces, Inheritance)
* **Development Environment**: Visual Studio / VS Code

## How to Run the Project

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Derto12/snake-multi
    cd <repository-folder>
    ```

2.  **Restore dependencies and run:**
    The project is configured to run with the .NET CLI.
    ```bash
    dotnet run
    ```

3.  **Controls:**
    * Define your `SnakeControl`objects and add them to your snakes.
    * Press `ESC` to exit the game.

## Creating Custom Maps

You can design your own levels by creating `.txt` files in the `Map_files` directory.

* `A`, `B`, etc.: The body of a snake.
* `0`, `1`, etc.: The head of the corresponding snake (`0` for `A`, `1` for `B`, and so on). The head *must* be placed adjacent to a body part.
* Any character that is not mentioned above (e.g. `#`): A wall or obstacle.

The `MapLoader` will automatically parse the file and set up the game.
