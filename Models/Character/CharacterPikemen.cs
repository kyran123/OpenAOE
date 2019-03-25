//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CharacterPikemen {

	//Main function to call and checks if the interaction is valid
	// - Param 1: A string containing the ability
	// - Param 2: The game character class instance
	public static bool checkIfInteractionIsValid(string ability, GameCharacter gc){
		//Bool variable for the result of the checks
		bool result = false;

		//Check what ability it is
		switch(ability) {
			case "Pillage":
				//If it is pillage call the function
				result = checkIfPillageIsValid(gc);
				break;
			case "Attack":
				//If it is Attack call the function
				result = checkIfAttackIsValid(gc);
				break;
			default:
				//If it's anything else check if it is an ability specific to this unit
				result = checkIfAbilityIsValid(ability);
				break;
		}

		//Return the result
		return result;
	}

	//Function that checks if the ability of this unit is valid to use in this position
	// - Param 1: String with the ability name
	private static bool checkIfAbilityIsValid(string ability){
		//Check which ability
		switch(ability) {
			case "Drill":
				return true;
		}
		return false;
	}

	//Function that checks if pillaging is valid on this tile
	// - Param 1: GameCharacter instance to get the values needed
	private static bool checkIfPillageIsValid(GameCharacter gc){
		int gcX = gc.getCharacterInteraction().x;
		int gcZ = gc.getCharacterInteraction().z;
		//Check if there is a building on position
		if(Game.getBuildingOnPosition(gcX, gcZ) != null) {
			//TODO: Check if there IS an pillagable building
			return true;
		}
		return false;
	}

	//Function that checks if there is anyone the pikemen can attack
	// - Param 1: GameCharacter instance to get the values needed
	private static bool checkIfAttackIsValid(GameCharacter gc){
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
			return true;
		}
		return false;
	}


	//Function that actually executes the drill ability
	public static void performDrill() {

	}

	//Function that changes tileType
	public static void changeTileType(string type) {

		int x = Mathf.RoundToInt(KeyBoardControls._instance.selectedUnit.getCharacterObject().transform.position.x);
		int z = Mathf.RoundToInt(KeyBoardControls._instance.selectedUnit.getCharacterObject().transform.position.z);

		GameMap gmInstance = GameMap._GMinstance;
		GameTile gmTile = gmInstance.getTileAt(x, z);

		//Check if unit is allowed to change tileType
		switch(type) {
			case "Empty":
				if(gmTile.Type != GameTile.TileType.Empty) {
					gmTile.setTileType(type);
					Debug.LogError("Why set tile to empty?");
				}
				break;
			case "Grass":
				if(gmTile.Type != GameTile.TileType.Grass) {
					gmTile.changeTile(type);
				}
				break;
			case "Forest":
				if(gmTile.Type != GameTile.TileType.Forest) {
					gmTile.changeTile(type);
				}
				break;
			case "Mountain":
				if(gmTile.Type != GameTile.TileType.Mountains) {
					gmTile.changeTile(type);
				}
				break;
			case "River":
				if(gmTile.Type != GameTile.TileType.River) {
					gmTile.changeTile(type);
				}
				break;
			case "Desert":
				if(gmTile.Type != GameTile.TileType.Desert) {
					gmTile.changeTile(type);
				}
				break;
			case "Stone_Road":
				if(gmTile.Type != GameTile.TileType.Stone_Road) {
					gmTile.changeTile(type);
				}
				break;
			case "Sea":
				if(gmTile.Type != GameTile.TileType.Sea) {
					gmTile.changeTile(type);
				}
				break;
			case "Hill":
				if(gmTile.Type != GameTile.TileType.Hills) {
					gmTile.changeTile(type);
				}
				break;
			default:
				Debug.LogError("type not found -> [CharacterPikemen/changeTileType]");
				break;
		}
	}
}

