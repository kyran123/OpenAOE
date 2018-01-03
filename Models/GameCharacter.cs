using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class GameCharacter {

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

	//Constructor
	// Param 1: name of the data file in string format
	public GameCharacter (string type) {
		//Instantiate CharacterModel class
		charModel = new CharacterModel();
		//Instantiate CharacterInteraction class
		charInteraction = new CharacterInteraction();
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

	//Get objects / instances
	public GameCharacter getInstance() { return this; }
	public GameObject getCharacterObject() { return this.charModel.getGameObject(); }

}

