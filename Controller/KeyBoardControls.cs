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
public class KeyBoardControls : MonoBehaviour {

	private Camera cam;
	private Vector3 oldPosition;
	private Vector3 currentPosition;
	Vector3 newPosition;
	Vector3 velocity;

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
			Vector3 oldPosition = this.transform.position;
			//Call update position function with the updated position
			updatePosition(oldPosition.x, oldPosition.y, (oldPosition.z + 1));
		}
		if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
			//Save the original position of the selector
			Vector3 oldPosition = this.transform.position;
			//Call update position function with the updated position
			updatePosition(oldPosition.x, oldPosition.y, (oldPosition.z - 1));
		}
		if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
			//Save the original position of the selector
			Vector3 oldPosition = this.transform.position;
			//Call update position function with the updated position
			updatePosition((oldPosition.x + 1), oldPosition.y, oldPosition.z);
		}
		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
			//Save the original position of the selector
			Vector3 oldPosition = this.transform.position;
			//Call update position function with the updated position
			updatePosition((oldPosition.x - 1), oldPosition.y, oldPosition.z);
		}
		//Check if the old position is the same as the new one.
		if (Vector3.Distance (oldPosition, newPosition) > 1.0) {
			//If the 2 positions aren't the same, move the position of the camera to follow the selector
			cam.transform.position = Vector3.SmoothDamp (cam.transform.position, newPosition, ref velocity, 0.3F);
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
