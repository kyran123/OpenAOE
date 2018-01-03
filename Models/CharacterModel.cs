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
		//Create empty object
		this.unitModel = createEmptyGameObject();
		setGameObjectPosition(5, 5);
	}

	//Creates empty game object and returns it
	public GameObject createEmptyGameObject(){
		GameObject goModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//TODO: Get name from GameCharacter class
		goModel.name = "Pikemen";
		return goModel;
	}

	//set Model for the object
	public void setModel(){
		//TODO: add the actual 3D model to the MeshRenderer
		this.unitModel.AddComponent<MeshRenderer>();
	}


	public void setGameObjectPosition(int x, int z){
		float xPosition = (float)x;
		float zPosition = (float)z;
		this.unitModel.transform.DOMove(new Vector3(xPosition, 0.5f, zPosition), 0.2f);
	}

	//Set position of object
	public void setGameObjectPosition(List<GameTile> path){
		Sequence movementSequence = DOTween.Sequence();

		//Loop through all the tiles within the list
		foreach(GameTile nextTile in path) {
			float xPosition = (float)nextTile.X;
			float zPosition = (float)nextTile.Z;
			//Debug.Log(nextTile.X + "_" + nextTile.Z);
			//Set the object position to the next tile in the list
			movementSequence.Append(this.unitModel.transform.DOMove(new Vector3(xPosition, 0.5f, zPosition), 0.2f));
		}
	}
}

