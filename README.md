# 4TB-Unity

4TB-Unity is the core component of a game project that combines Unity with a web-based interface and a backend system to deliver an interactive quiz game experience. Players join the game through a smartphone browser, where they answer questions and view real-time game progress and scores. The game is powered by Unity, with the frontend developed in React (check 4TB-Frontend) and the backend developed using Node.js and Express (check 4TB-backend).

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [How It Works](#how-it-works)
3. [Game Flow](#game-flow)
4. [Key Components](#key-components)
5. [Project Files](#project-files)

---

## Project Overview

The 4TB-Unity repository serves as the game master of the quiz game, coordinating with both the frontend and backend to ensure smooth communication and gameplay. The backend, located in the `4TB-backend` repository, handles the logic for managing game sessions and player interactions. The frontend, found in the `4TB-frontend` repository, is responsible for presenting the game interface to the players.

## How It Works

Players join the quiz game using their chosen ID and participate by answering questions presented through a web browser interface. The game consists of multiple rounds, with players earning scores after each round. In the end, the game displays the total scores and concludes.

## Game Flow

1. **Join Game**: Players enter the game by choosing an ID.
2. **Answer Questions**: Players respond to quiz questions within a time limit.
3. **Display Scores**: Scores are shown after each round.
4. **End Game**: After the final round, the game concludes and shows the final scores.

---

## Key Components

The `4TB-Unity` repository contains several core components that handle different aspects of the game logic:

- **Singleton**: The game master that directs the overall game flow, similar to a CEO. It manages how the game progresses and delegates tasks to other handlers.
  
- **APIHandler**: Manages communication between the Unity game and the backend.

- **Player**: An object class representing each player in the game, storing relevant data like player ID and score.

- **PlayerHandler**: Manages all player-related tasks, such as adding/removing players, tracking their progress, and updating scores.

- **QuestionHandler**: Retrieves and manages the quiz questions from the backend and presents them to the players.

- **ScreenHandler**: Responsible for updating the game’s visual elements on the screen based on player actions and game phases.

- **SessionHandler**: Manages the creation and ending of game sessions, including communicating with the backend to start and end the game, and determining what needs to be displayed to the players.

- **GameSituationHandler**: Tracks the current state of the game, including the number of questions answered, which player is currently active, and which player won the current turn.

- **AnswerHandler**: Handles the collection and evaluation of answers submitted by players.

- **Timer**: Controls the game’s timing, such as limiting how long players have to answer questions and how long they can view their scores.

---

## Project Files

The project is divided into the following files and components:

- **`Singleton.cs`**: Contains the game master logic, orchestrating the game flow.
- **`APIHandler.cs`**: Manages communication with the backend API.
- **`Player.cs`**: Defines the Player class with attributes like ID and score.
- **`PlayerHandler.cs`**: Handles player-related operations, including adding/removing players.
- **`QuestionHandler.cs`**: Manages the quiz questions and retrieves them from the backend.
- **`ScreenHandler.cs`**: Handles visual changes on the game screen.
- **`SessionHandler.cs`**: Manages the creation and termination of game sessions.
- **`GameSituationHandler.cs`**: Tracks the ongoing game situation, such as the turn and current question.
- **`AnswerHandler.cs`**: Handles players' answers and validates them.
- **`Timer.cs`**: Controls the timing of each game phase.

---
