//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// The keyboard controls class that handles all input from the keyboard.
//
using System;


public class KeyBoardControls : MonoBehaviour {

	private Camera cam;
	private Vector3 oldPosition;
	private Vector3 currentPosition;
	Vector3 newPosition;
	Vector3 velocity;

	protected GameCharacter selectedUnit;

	// Use this for initialization
	void Start () {
		//Get main camera object
		cam = Camera.main;
		//Get zero'd Vector3
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		//Check what input is given and send the new position to the updatePosition function
		if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
			//Save the original position of the selector
			oldPosition = this.transform.position;
			//Call update position function with the updated position
			updatePosition(oldPosition.x, oldPosition.y, (oldPosition.z + 1));
		}
		if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
			//Save the original position of the selector
			oldPosition = this.transform.position;
			//Call update position function with the updated position
			updatePosition(oldPosition.x, oldPosition.y, (oldPosition.z - 1));
		}
		if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
			//Save the original position of the selector
			oldPosition = this.transform.position;
			//Call update position function with the updated position
			updatePosition((oldPosition.x + 1), oldPosition.y, oldPosition.z);
		}
		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
			//Save the original position of the selector
			oldPosition = this.transform.position;
			//Call update position function with the updated position
			updatePosition((oldPosition.x - 1), oldPosition.y, oldPosition.z);
		}
		//Check if the old position is the same as the new one.
		if (Vector3.Distance(oldPosition, newPosition) > 1.0) {
			//If the 2 positions aren't the same, move the position of the camera to follow the selector
			cam.transform.position = Vector3.SmoothDamp (cam.transform.position, newPosition, ref velocity, 0.3F);
		}

		//Check if enter is pressed (Later also mouse click and tapping the screen)
		if(Input.GetKeyDown(KeyCode.Return) || Input.GetKey("enter")){
			int x = Mathf.RoundToInt(this.transform.position.x);
			int z = Mathf.RoundToInt(this.transform.position.z);

			//Select unit or building that is in that tile
			GameCharacter select = Game.getCharacterOnPosition(x, z);

			//If there has no unit been selected yet
			if(selectedUnit != null) {
				//Check if the tile that has been selected does have any units (Later also buildings)
				if(select == null) {
					//No units here, move the selected unit.
					selectedUnit.moveUnitTo(GameMap._GMinstance.getTileAt(selectedUnit.getXPosition(), selectedUnit.getZPosition()), GameMap._GMinstance.getTileAt(x, z));
					selectedUnit.getCharacterAI().removePossibleMovesGraph();
					selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.white;

					//TODO: Unit should get the option to click done, attack etc.
					//TODO: Unit should be frozen after clicking done and unable to move until next turn!

					selectedUnit = null;
					select = null;

				} else if(select.getXPosition() == selectedUnit.getXPosition() && select.getZPosition() == selectedUnit.getZPosition()) {
					//Same unit selected! Attack? powers?
				} else if(Game.getCharacterOnPosition(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.z)) != null) {
					//Select the new unit
					selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.white;
					selectedUnit = null;
				}
			} else {
				//When unit has not been selected yet, and the tile that has been pressed enter on has a unit. 
				if(select != null) {
					//Set that unit as the selected unit.
					selectedUnit = select;
					//Change color of the unit to indicate that the unit has been selected
					//In the future you might want to make this an glow behind it, instead.
					selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.blue;
					selectedUnit.getCharacterAI().generatePossibleMovesGraph(GameMap._GMinstance.getTileAt(x, z), selectedUnit.movementPoints);
				}
			}
		}
	}

	//Moves the selector to the new tile.
	//Params: X, Y and Z coordinates for the selector
	private void updatePosition(float x, float y, float z){
		//Checks if the user doesn't go out of bounds
		if (x < 0 || y < 0 || z < 0 || x > (GameMap._GMinstance.Width - 1) || z > (GameMap._GMinstance.Height - 1)) {
			//Log error that user want to go out of bounds
			Debug.LogError ("Out of bounds!");
			//Return to prevent the user from moving onto a non existing tile
			return;
		}

		//Save the vector as the current position;
		currentPosition = new Vector3 (x, y, z);
		//Set the position to the game object's transform
		this.transform.position = currentPosition;
		//Calculate and store new position for the camera
		newPosition = new Vector3 ((x - 9), (y + 12), (z - 9));
	}

	//TODO: Change camera settings like:
	//Possibly - Projection to Orthographic
	//The near clipping panes need to be adjusted
	//And change camera position
	//Do this when art has been already created

	//TODO: Mouseclick
	//Set the position of the selector to the mouse click position
}
