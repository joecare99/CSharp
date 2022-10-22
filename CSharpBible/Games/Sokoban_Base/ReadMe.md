# ++Sokoban Base++

	     #####            ###               ###
	   ###        ###    ###  ##    ###    #####      #####  #####
	    ###    ###  ##  ###  ##  ###  ##  ###  ##  ###  ##  ###  ##
	     ###  ###  ##  #####    ###  ##  ###  ##  ###  ##  ###  ##
	#####      ###    ###  ##    ###    #####      #####  ###  ##

## ++Description++

This project implements a basic Sokoban-engine (incl. a simple UI)
Sokoban is the game where the player has to move stones/boxes to a given destination.
the stones/boxes can only be moved by pusching. 

---
## Outline 

The Engine is divided mostly into 3 Parts:
* the Model (containing the basic engine-clases)
* the ViewModel (containing the game-logic)
* the View (containing the UI)
* Resources ( containing the InfoText & the other UI-strings)

---
## Model

In the model-part there are defined several classes and enumerations
* Direction
* FieldDef
* LabDef
* Field 
  * Wall
  * Floor
  * ...
* PlayObject
  * Player
  * Stone
* Playfield 

There is also a class-diagram showing the dependency and inheritence of these classes

---
## ViewModel

The viewmodel ist the interface between the model and the UI. It also contains classes and enumerations:
* Tiledef
* UserAction
* Game

There is also a class-diagram showing the dependency and inheritence of these classes

## View


