using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CharacterPikemen {
	//Function that checks if the ability of this unit is valid to use in this position
	// - Param 1: String with the ability name
	public static bool checkIfAbilityIsValid(string ability){
		//Check which ability
		switch(ability) {
			case "Drill":
				return true;
		}
		return false;
	}

	//Function that checks if pillaging is valid on this tile
	// - Param 1: GameCharacter instance to get the values needed
	public static bool checkIfPillageIsValid(GameCharacter gc){
		int gcX = gc.getCharacterInteraction().x;
		int gcZ = gc.getCharacterInteraction().z;
		//Check if there is a building on position
		if(Game.getBuildingOnPosition(gcX, gcZ)) {
			//TODO: Check if there IS an pillagable building
			return true;
		}
		return false;
	}

	//Function that checks if there is anyone the pikemen can attack
	// - Param 1: GameCharacter instance to get the values needed
	public static bool checkIfAttackIsValid(GameCharacter gc){
		int gcX = gc.getCharacterInteraction().x;
		int gcZ = gc.getCharacterInteraction().z;
		//Checks if there are any characters to attack
		if(
			Game.getCharacterOnPosition((gcX - 1), gcZ) != null ||
			Game.getCharacterOnPosition((gcX + 1), gcZ) != null ||
			Game.getCharacterOnPosition(gcX, (gcZ - 1)) != null ||
			Game.getCharacterOnPosition(gcX, (gcZ + 1)) != null
		) {
			//TODO: Check if unit isn't this players unit
			//TODO: Highlight possible attackable units
			return true;
		}
		return false;
	}
}

