//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public abstract class GameCharacter {
	//Get the instance of the ActionMenu stored here, since we want to add abilities to it depending on the situation
	ActionMenu am;

	//Variable where the Character model class will be stored
	protected CharacterModel charModel;
	//The getter of the variable. There is no setter as it should never change!
	public CharacterModel getCharacterModel() {
		return this.charModel;
	}
	//Variable where the Character Interaction class will be stored
	protected CharacterInteraction charInteraction;
	//The getter of the variable. There is no setter as it should never change!
	public CharacterInteraction getCharacterInteraction(){
		return this.charInteraction;
	}

	//Variable where the Character AI will be stored
	protected CharacterAI charAI;
	//The getter of the variable. There is no setter as it never should change!
	public CharacterAI getCharacterAI(){
		return this.charAI;
	}

	//TEMP VARIABLE
	public int movementPoints = 7;
	private List<String> abilityList;

	//Constructor
	// Param 1: name of the data file in string format
	public GameCharacter (string type) {
		//Instantiate CharacterModel class
		this.charModel = new CharacterModel();
		//Instantiate CharacterInteraction class
		this.charInteraction = new CharacterInteraction();
		//Set start coordinates of unit (This will depend on scenario later)
		this.charInteraction.x = 5;
		this.charInteraction.z = 5;
		//Instantiate CharacterAI class
		this.charAI = new CharacterAI();
		//Get instance of ActionMenu class
		am = GameObject.Find("actionMenu").GetComponent<ActionMenu>();
		//Create new list of abilities
		//TODO: This list should be dynamically created when data is loaded
		this.abilityList = new List<string>();
	}
		
	// Properties of the X Position
	public int getXPosition() { return this.charInteraction.x; }
	public void setXPosition(int x) { 
		this.charInteraction.x = x; 
		this.charModel.setGameObjectPosition(x, this.charInteraction.z);
	}

	//Properties of the Y Position
	public int getZPosition(){ return this.charInteraction.z; }
	public void setZPosition(int z) { 
		this.charInteraction.z = z; 
		this.charModel.setGameObjectPosition(this.charInteraction.x, z);
	}

	//Function that passes through and adds all necessary information for showing update menu
	public void showAbilities() {
		this.abilityList.Clear();
		this.abilityList.Add("Pillage");
		this.abilityList.Add("Done");
		this.am.updateMenu(this, abilityList);
	}

	//Get objects / instances
	public GameCharacter getInstance() { return this; }
	public GameObject getCharacterObject() { return this.charModel.getGameObject(); }

	//Move the unit to the target position
	public void moveUnitTo(GameTile oldPosition, GameTile newPosition){
		//Get the list of tiles of the path from the AI.findPath function!
		List<GameTile> path = this.charAI.findPath(oldPosition, newPosition);
		//Check if the list has at least 1 tile in it
		if(path.Count > 0) {
			if(this.charAI.isReachable(newPosition)) {
				//Set the position
				this.charModel.setGameObjectPosition(path);
				//Update position in the interaction class
				this.charInteraction.x = newPosition.X;
				this.charInteraction.z = newPosition.Z;
			}
		} else {
			//ERROR
			Debug.Log("Can't reach this!");
		}
	}








	//Function that will be called when one of the ability buttons is clicked
	//Done is one of the basic unit abilities
	public void unitDone(){
		//TODO: lock unit
		ActionMenu._instance.removeButtonsFromMenu();
		KeyBoardControls._instance.updateMenuFocus();
	}

	//Function that will be called when one of the ability buttons is clicked
	//Pillage is one of the basic unit abilities
	public void unitPillage(){
		//TODO: Actually pillage tile
		ActionMenu._instance.removeButtonsFromMenu();
		KeyBoardControls._instance.updateMenuFocus();
	}
}

