## Purpose
The assumptions, constraints, and requirements of a real-time first-person shooter multiplayer game are specified and laid out in this document.
These will serve as a guideline throughout the project's development process. To ensure the quality of the software produced, the final product will be tested against the requirements.

## Scope
The scope of this system is to build an online FPS game that makes use of the Unity 3D engine and Photon Unity Networking.
This multiplayer game will be a deathmatch-style game in which players compete on a map. Submachine guns, handguns, and riffles are among the weapons available to players. The goal of the game is to score more points by killing other players. Each player will connect to the same server using a computer. The server will then handle the game's networking, allowing players to interact with one another in real-time. The server will also track the progress of the game and relay information between players.
 
## Functional Requirements
* Each match must be playable by two to ten players at the same time.
* The game shall include the main menu with options such as Create a Room, Browser a Room, Settings, and Exit Game.
* The players shall compete against each other over the network.
* The match shall finish when the player reaches a point limit, or if time runs out.
* The game interface should show how much health the players have left as well as how much ammunition they currently have.
* The screen's field of view must be fixed; the camera must not zoom in and out.

## Non-functional Requirements

### User Interface
* The user interface should be simple, clear, and compatible with widescreen displays.
* The user interface must be controlled by a mouse and a keyboard.

### Performance
* Starting the game and getting to the menu screen should take no more than ten seconds.
* At all times, the game should have a refresh rate of at least 30 frames per second.

### Availability
* The system must be operational 24 hours a day, seven days a week. There is no room for prolonged downtime, especially when the game is played globally.
* In the event of a failure that causes the system to fail, repair work should be completed as soon as possible.

### Reliability
* The system must allow for smooth execution and data integrity.

### Security
* Since this game does not create user profiles, it does not require access to user credentials. As a result, security is not an issue in this system.


## Constraints
* Cheating attempts
* Screen size and resolution
* Syncing clients and servers over the internet.
* Internet connection speed
* Software compatibility

## Assumptions and Dependencies
* The game is dependent on the availability of an Internet Connection. Any connection loss from a client will result in a loss in the game. 
* The game will be developed to run on Windows (2007 and up)
* The game will be based on a 3D engine.


## Initial Design

### Main Menu Options
These are the options that will be available on the main menu:
Create a Room: This option allows the player to create a room.
Browser a Room: This option allows the player to browse existing rooms and join them.
Settings: This option allows the player to customize the game settings such as sound, graphics, and controls.
Exit Game: This option allows the player to exit the game.


### How the game will be controlled:
The controls in a first-person shooter (FPS) game are designed to give the player a realistic experience of what it's like to be in the game. The player controls their character with a combination of the keyboard and mouse.
The WASD keys are used to move the character, while the mouse provides more precise control. Players can look up, down, and all around the environment by using the mouse to control the character's view.
The player's weapons are fired by using the left mouse button.
Sprinting is done by pressing the shift key. Jumping is accomplished by pressing the space bar.


### Two characters shoot simultaneously (Fairness)

There are a few different ways that the game can deal with this situation. The initial approach should be taking into consideration the characters' stats, such as health or accuracy, to determine which character is killed. Another alternative should be having both characters killed at the same time. This could be done by having both characters' health reduced to 0 simultaneously. Or even allowing both characters to survive the encounter but with reduced health.
# Synchronization
The Photon package provides us with access to the PhotonView MonoBehaviour component. When a component is set to "observe," you can easily make certain game objects "network aware."
This effectively states that the game object is the same across all clients. That is, if there is a mine, anything we do to it in my instance of the game will be synced with the same mine in your instance of the game. If my mine explodes, it can instruct all replicated versions of itself in other clients to do the same.
Another component, PhotonTransformView, allows the game object's position, rotation, and scale to be automatically synced across clients. This means that each player controls their own character's movement, and those properties will be updated in other versions of the game.
PhotonViews communicate using RPC (Remote Procedure Call) functions. If a function is designated as an RPC, it can be called by other versions of itself in different clients.
RPC will be used only when data does not change frequently, as it may cause weak clients to break due to excessive buffering.
An example of RPC usage would be when an enemy is killed.
When it comes to frequent updates, such as player positions, which are likely to be quickly replaced by a new update, a PhotonView can be configured to send "Unreliable on Change." This prevents updates from being sent when the character, for example, is not moving.
