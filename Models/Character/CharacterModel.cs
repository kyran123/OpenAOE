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

public class CharacterModel {

	//Variable where the game object of this unit is stored
	protected GameObject unitModel;
	//Get the game object of this unit
	public GameObject getGameObject(){
		return this.unitModel;
	}

	//Constructor
	public CharacterModel(){

	}

	//Creates empty game object and returns it
	public void createGameObject(string charName){
		this.unitModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.unitModel.name = charName;

    }

	//set Model for the object
	public void setModel(){
		//TODO: add the actual 3D model to the MeshRenderer
		this.unitModel.AddComponent<MeshRenderer>();
	}

	//Function to set the position of the object (ignoring any tiles / movement cost)
	public void setGameObjectPosition(int x, int z){
		float xPosition = (float)x;
		float zPosition = (float)z;
		this.unitModel.transform.DOMove(new Vector3(xPosition, 0.8f, zPosition), 0.2f);
	}

	//Move unit on map
	// - Param 1: List of GameTile classes
	public void setGameObjectPosition(List<GameTile> path, GameCharacter gc){
		//Create new Sequence of animations
		Sequence movementSequence = DOTween.Sequence();

		//Loop through all the tiles within the list
		foreach(GameTile nextTile in path) {
			//Get the X and Z coordinates and convert them to floats
			float xPosition = (float)nextTile.X;
			float zPosition = (float)nextTile.Z;
            //Check if it isn't the original tile the unit was already on (which doesn't count against the movement cost
            if(path[0] != nextTile) {
                //Add cost to the movementCost variable
                gc.addMovementCost(nextTile.TerrainModifier);
            }
            //Set the object position to the next tile in the list
            movementSequence.Append(this.unitModel.transform.DOMove(new Vector3(xPosition, 0.8f, zPosition), 0.2f));
            //Update unit stats on information panel
            gc.gameInformationInstance.showCharacterOnTile(gc);
        }
	}
}

