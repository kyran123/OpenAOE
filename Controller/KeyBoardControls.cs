//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

//
// The keyboard controls class that handles all input from the keyboard.
//
public class KeyBoardControls : MonoBehaviour {

	//Instance of this class
	public static KeyBoardControls _instance;

	//main camera object
	private Camera cam;
	//Vector 3 of the position where the selector was at
	private Vector3 oldPosition;
	//Vector 3 where the selector was when enter was pressed most recently
	private Vector3 currentPosition;
	//Vector 3 of the new position of the selector
	private Vector3 newPosition;
	//Vector 3 of the velocity, which is always Vector3.Zero
	private Vector3 velocity;

	//Variables to keep track of little pieces of information that get re-used regularly
	private int attackRange;
	private int selectableIndex;

	//Variable that keeps track of the menu game object
	private GameObject actionMenu;
	//Checks if the focus should be on menu
	private bool inMenu = false;
	//Checks if the focus should be on selecting an unit to attack, debuff or buff.
	private bool selectMode = false;
	//List of buttons that might need to be accessed
	List<GameObject> buttonList;
	//List of selectable units within range
	List<GameCharacter> selectable;

	//The game character that has been selected
	public GameCharacter selectedUnit { get; set; }

	// Use this for initialization
	void Start() {
		//Get main camera object
		this.cam = Camera.main;
		//Get zero'd Vector3
		this.velocity = Vector3.zero;
		//Get the game object of the action menu and store it
		this.actionMenu = GameObject.FindGameObjectWithTag("actionMenu");
		//Set the actionMenu to unactive
		this.actionMenu.SetActive(false);
		//Add this class to the instance variable
		KeyBoardControls._instance = this;
		//Get the list of action buttons
		this.buttonList = ActionMenu._instance.buttons;
		//initialize selectable list
		this.selectable = new List<GameCharacter>();
	}
	
	// Update is called once per frame
	void Update() {
		//Check if focus is on movement or selection
		if(!this.selectMode) {

			//Check if focus is on menu or game
			if(!this.inMenu) {
			
				//Check what input is given and send the new position to the updatePosition function
				if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
					//Save the original position of the selector
					this.oldPosition = this.transform.position;
					//Call update position function with the updated position
					updatePosition(this.oldPosition.x, this.oldPosition.y, (this.oldPosition.z + 1));
				}
				if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
					//Save the original position of the selector
					this.oldPosition = this.transform.position;
					//Call update position function with the updated position
					updatePosition(this.oldPosition.x, this.oldPosition.y, (this.oldPosition.z - 1));
				}
				if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
					//Save the original position of the selector
					this.oldPosition = this.transform.position;
					//Call update position function with the updated position
					updatePosition((this.oldPosition.x + 1), this.oldPosition.y, this.oldPosition.z);
				}
				if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
					//Save the original position of the selector
					this.oldPosition = this.transform.position;
					//Call update position function with the updated position
					updatePosition((this.oldPosition.x - 1), this.oldPosition.y, this.oldPosition.z);
				}
				//Check if the old position is the same as the new one.
				if(Vector3.Distance(this.oldPosition, this.newPosition) > 1.0) {
					//If the 2 positions aren't the same, move the position of the camera to follow the selector
					this.cam.transform.position = Vector3.SmoothDamp(this.cam.transform.position, this.newPosition, ref this.velocity, 0.3F);
				}

				//Check if enter is pressed (Later also mouse click and tapping the screen)
				if(Input.GetKeyDown(KeyCode.Return) || Input.GetKey("enter")) {
					int x = Mathf.RoundToInt(this.transform.position.x);
					int z = Mathf.RoundToInt(this.transform.position.z);

					//Select unit or building that is in that tile
					GameCharacter select = Game.getCharacterOnPosition(x, z);

					//If there has no unit been selected yet
					if(this.selectedUnit != null) {
						//Check if the tile that has been selected does have any units (Later also buildings)
						if(select == null) {
							//No units here, move the selected unit.
							this.selectedUnit.moveUnitTo(GameMap._GMinstance.getTileAt(selectedUnit.getXPosition(), selectedUnit.getZPosition()), GameMap._GMinstance.getTileAt(x, z));

							//Check if move was valid
							if(this.selectedUnit.getCharacterAI().isReachable(GameMap._GMinstance.getTileAt(x, z))) {
								//Set focus to menu
								this.inMenu = true;
								//Set menu to active
								this.actionMenu.SetActive(true);
								//Show buttons on menu
								this.selectedUnit.showAbilities();
							}
							//Remove the graph overlayed on the tiles
							this.selectedUnit.getCharacterAI().removePossibleMovesGraph();

							//Set the color back to white, to indicate that it isn't selected anymore
							this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.white;
						

						} else if(select.getXPosition() == this.selectedUnit.getXPosition() && select.getZPosition() == this.selectedUnit.getZPosition()) {
							//Same unit selected! Attack? powers?
							//Set inMenu variable to true, to indicate that the focus should be on
							this.inMenu = true;
							//Set menu to visible
							this.actionMenu.SetActive(true);
							//Show abilities of unit on menu
							this.selectedUnit.showAbilities();
							//Remove the graph of moves
							this.selectedUnit.getCharacterAI().removePossibleMovesGraph();

						} else if(Game.getCharacterOnPosition(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.z)) != null) {
							//Select the new unit
							if(this.selectedUnit.isUnitLocked()) {
								//Set unit color to gray
								this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.gray;
							} else {
								//Set unit color to white
								this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.white;
							}
							//Remove the graph of moves
							this.selectedUnit.getCharacterAI().removePossibleMovesGraph();
							//Set position of selector back to original position
							this.transform.localPosition = new Vector3(
								this.selectedUnit.getCharacterObject().transform.position.x,
								this.transform.position.y,
								this.selectedUnit.getCharacterObject().transform.position.z
							);
							//Empty the selected unit from the variable
							//So the user has to select the unit again
							this.selectedUnit = null;
						}
					} else {
						//When unit has not been selected yet, and the tile that has been pressed enter on has a unit. 
						if(select != null && select.isUnitLocked() == false) {
							//Set that unit as the selected unit.
							this.selectedUnit = select;
							//Get the attack range of the unit
							this.attackRange = this.selectedUnit.attackRange;
							//Call select unit
							this.selectUnit(x, z);
						}
					}
				}

			} 
		} else {
			//Prevents this from being called prematurely
			if(this.selectableIndex < 9000) {
				
				//When left arrow, A, down arrow or S is pressed
				if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
					//Check if index is valid
					if(this.selectableIndex > 0) {
						//Increase index
						this.selectableIndex--;
					} else {
						//Reset index
						this.selectableIndex = (this.selectable.Count - 1);
					}
					//highlight the newly selected unit
					this.highlightUnit();
				}
				//When right arrow, D, up arrow or W is pressed
				if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
					//Check if index is valid
					if(this.selectableIndex < (this.selectable.Count - 1)) {
						//Increase index
						this.selectableIndex++;
					} else {
						//Reset index
						this.selectableIndex = 0;
					}
					//highlight the newly selected unit
					this.highlightUnit();
				}

				//When enter is pressed
				if(Input.GetKeyDown(KeyCode.Return) || Input.GetKey("enter")) {
					//Set the target unit to red
					this.selectable[this.selectableIndex].getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.red;
					//Lock the unit that made the move
					this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.gray;
					this.selectedUnit.setUnitLocked(true);
					//Reset the modes back to normal movement
					this.inMenu = false;
					this.selectMode = false;
					this.selectedUnit = null;
				}

			} else {
				//Set the selectableIndex to something that is valid
				this.selectableIndex = 0;
				this.highlightUnit();
			}
		}

		//set inMenu variable to false when the action menu isn't active anymore
		if(!this.actionMenu.activeSelf) {
			this.inMenu = false;
		}
	}


	//Function that interacts with the units for selecting
	public void highlightUnit() {
		//Clear all units from color
		foreach(GameCharacter unit in this.selectable) {
			if(unit.isUnitLocked()) {
				unit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.gray;
			} else {
				unit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.white;
			}
		}
		//Set cursor to the position of the selectable unit
		this.transform.position = new Vector3(
			this.selectable[this.selectableIndex].getXPosition(), 
			this.transform.position.y, 
			this.selectable[this.selectableIndex].getZPosition()
		);
		//Set the color to yellow
		this.selectable[this.selectableIndex].getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.yellow;
	}

	//Moves the selector to the new tile.
	//Params: X, Y and Z coordinates for the selector
	public void updatePosition(float x, float y, float z){
		//Checks if the user doesn't go out of bounds
		if (x < 0 || y < 0 || z < 0 || x > (GameMap._GMinstance.Width - 1) || z > (GameMap._GMinstance.Height - 1)) {
			//Log error that user want to go out of bounds
			Debug.LogError("Out of bounds!");
			//Return to prevent the user from moving onto a non existing tile
			return;
		}

		//Save the vector as the current position;
		this.currentPosition = new Vector3(x, y, z);
		//Set the position to the game object's transform
		this.transform.position = currentPosition;
		//Calculate and store new position for the camera
		this.newPosition = new Vector3((x - 9), (y + 12), (z - 9));
	}

	//Function that hides action menu and continues game
	public void updateMenuFocus() {
		this.actionMenu.SetActive(false);
	}

	public void setSelectMode() {
		//Clear selectable list
		this.selectable.Clear();

		//Set select mode to true
		this.selectMode = true;

		//Hide menu
		updateMenuFocus();

		//Create the list of attackable units
		if(this.attackRange <= 1) {
			//Check for units in the 4 tiles around the unit
			if(Game.getCharacterOnPosition((this.selectedUnit.getXPosition() + 1), this.selectedUnit.getZPosition()) != null) {
				//Add unit to list of selectables
				this.selectable.Add(Game.getCharacterOnPosition((this.selectedUnit.getXPosition() + 1), this.selectedUnit.getZPosition()));
			}
			if(Game.getCharacterOnPosition((this.selectedUnit.getXPosition() - 1), this.selectedUnit.getZPosition()) != null) {
				//Add unit to list of selectables
				this.selectable.Add(Game.getCharacterOnPosition((this.selectedUnit.getXPosition() - 1), this.selectedUnit.getZPosition()));
			}
			if(Game.getCharacterOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() + 1)) != null) {
				//Add unit to list of selectables
				this.selectable.Add(Game.getCharacterOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() + 1)));
			}
			if(Game.getCharacterOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() - 1)) != null) {
				//Add unit to list of selectables
				this.selectable.Add(Game.getCharacterOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() - 1)));
			}

			//Set the index to start of with
			this.selectableIndex = 9999;

		} else if(this.attackRange > 1) {
			//Check the grid of the attack size for units
		}
	}

	//Function that selects unit and generates a moves graph for it
	public void selectUnit(int x, int z) {
		//Change color of the unit to indicate that the unit has been selected
		//In the future you might want to make this an glow behind it, instead.
		this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.blue;
		//Generate the possible moves there are for the selected unit
		this.selectedUnit.getCharacterAI().generatePossibleMovesGraph(GameMap._GMinstance.getTileAt(x, z), selectedUnit.movementPoints);
	}

	//TODO: Change camera settings like:
	//Possibly - Projection to Orthographic
	//The near clipping panes need to be adjusted
	//And change camera position
	//Do this when art has been already created

	//TODO: Mouseclick
	//Set the position of the selector to the mouse click position
}
