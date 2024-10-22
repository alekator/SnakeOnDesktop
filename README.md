# SnakeOnDesktop

## Project Description
**SnakeOnDesktop** is a modern implementation of the classic game "Snake" for the Windows platform, developed in C#. This application is not just an ordinary "Snake"; it includes numerous unique features and enhancements that make the gameplay more engaging and exciting.

### Unique Features:
- **Game Field Anywhere:** Watching a movie? Reading a document? Play Snake right over it with full overlap of all open windows and a transparent game map.
- **Dynamic Difficulty Levels:** Players can choose the difficulty level before starting the game, allowing them to tailor the gameplay to their skill level and preferences.
- **Interactive Elements:** The game features various obstacles and portals that add new tactical possibilities and diversity to the gameplay.
- **Leaderboard:** The ability to track your achievements and compete with other players through a leaderboard system that stores high scores and usernames.
- **Sound Effects and Music:** Utilizing the NAudio library for playing music and effects, which enhances the atmosphere of the game.
- **Modern Interface:** The game interface, developed using Windows Forms, offers an intuitive and pleasant visual experience.
- **Minimize Windows:** Integration with the Win32 API allows players to minimize all open windows to focus on the game.

## Technology Stack
- **Programming Language:** C#
- **Platform:** .NET Framework
- **Graphics:** Windows Forms for visualizing the gameplay
- **Database:** Microsoft SQL Server for storing usernames and high scores
- **Sound Effects:** NAudio for playing music and sound effects
- **Windows API:** Using Win32 API for interacting with the system, including window management, minimizing all windows, and obtaining information about the current window
- **Thread Management:** Utilizing asynchronous methods to manage gameplay and interact with the database without blocking the user interface
- **Event Handling:** Implementing events and event handlers for user interaction with the game interface, including key presses and graphics updates
- **Object-Oriented Programming (OOP):** Applying OOP principles to structure the code, including creating classes for various game objects (e.g., snake, obstacles, portals)

## Key Features
- **Gameplay:** 
  - Control the snake using the arrow keys on the keyboard.
  - Collect food to grow and earn points.
- **Difficulty Levels:** 
  - Choose the difficulty level before starting the game (Easy, Medium, Hard).
- **Leaderboard System:** 
  - Save and display users' high scores.
  - Ability to check personal records.
- **Sound Effects:** 
  - Playback of background music and sounds when collecting food and ending the game.
- **Portals:** 
  - Interactive elements that move the snake to the opposite side of the game field.

## Installation
1. **Cloning the Repository:**
   ```bash
   git clone https://github.com/username/SnakeOnDesktop.git
   ```
## Building the Project:
- Open the project in Visual Studio.
- Build the project using the "Release" configuration.
  
## Running
- Run the resulting EXE file to start playing SnakeOnDesktop.
  
## Contribution
- If you would like to contribute to the project, please create a Pull Request with your changes. I welcome any improvements!

## License
- This project is licensed under the MIT License.
  
### Notes:
- Replace username in the repository cloning line with your GitHub username.
- Add any additional content or sections at your discretion to highlight the unique features of your project.





