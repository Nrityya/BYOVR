# BYOVR

* **Unity scene contains the prototype:** SampleScene  
* **Target device**: Android device (Google cardboard)  
* **GitHub**: [https://github.com/Nrityya/BYOVR](https://github.com/Nrityya/BYOVR)  
* **Demo:** [https://youtu.be/VJxdZengGds](https://youtu.be/VJxdZengGds)

**Advanced Requirements**

* Multiplayer with voice chat  
  * These were implemented using Photon Fusion 2 and Photon Voice.  
  * When a player joins, they join a shared lobby. The first player to join is the host.  
  * Within the lobby, players are able to see other players move around, and players can play games together (Uno, Beer Pong, Billiards, and Karaoke).  
  * All interactable items and menus are synced with all players.  
  * Voice chat is open mic, so everyone can hear everyone else talk like in real life.  
  * All movement and interactions are smooth thanks to Photon’s interpolation.  
* Avatar with animations  
  * Each player has a realistic humanoid avatar that represents them in the game.  
  * The avatar, rig, and animations were made by hand in Blender.  
  * The avatar has a standing pose and walking animation for when the player is moving. The avatar faces the direction of travel realistically.  
  * The player avatar has different gestures that can be used for communication. The player can wave using the Y button and can point using the A button.  
  * All animations are synced using the networking system

We chose these two advanced requirements because our project is designed to emulate a house party. Because parties include multiple people, we needed multiplayer, and since we want people to communicate, we added the open mic voice chat. The avatars and gestures increase the sense of immersion for the players by allowing them to realistically express themselves. These features allowed us to create a fun game that makes people feel like they are partying with other people.

**Interaction techniques and how they are used with Android phones and Controllers.**

* The controls for the player’s current context can be seen at any time by pressing the menu button  
* Standard controls (applies to all games)  
  * Using head mounted reticle, you can select items by hovering  
  * Grab/drop/select an item by using X  
  * Move with joystick, unless a game stops your movement (i.e. Uno)  
* Controls when not in a game  
  * Teleport using B  
  * Point using A  
  * Wave using Y  
* Controls when grabbing a throwable object (i.e. Beer Pong)  
  * Hold Y to throw it. A charge bar will appear to show how far it will go  
  * Hold OK and use the joystick to rotate objects  
* Controls when holding a pool cue  
  * Press A to use the cue on a hovered object  
  * Use the joystick to change the orientation of it around the object  
  * Hold Y to charge the pool cue to hit the selected object. A charge bar will appear to show how hard it will be hit  
* Controls for UNO  
  * The player can join/leave the game by pressing X on the join/leave button. The player cannot move when they are playing.  
  * Draw a card by pressing X on the deck.  
  * Switch between cards in your hand by pressing A and Y.  
  * Play a card by pressing X on the discard pile 

**Equipment and Operating Multiplayer**

* Our game only requires the standard equipment (Android phone, Google Cardboard, and Fortune Tech controller). For voice chat, the phone needs to have speakers and a microphone.  
* After the game is installed and opened, no other setup is required.  
* When the game is launched, the player will automatically join the lobby or start hosting if no one else is playing yet. All players will automatically join the same lobby, so other players on different devices just need to open the game.

**Demo**

* [https://youtu.be/VJxdZengGds](https://youtu.be/VJxdZengGds)

