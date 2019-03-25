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

    //Instance of GameInformation
    protected GameInformation gi;

    //main camera object
    private Camera cam;
    //Vector 3 of the position where the selector was at
    public Vector3 oldPosition;
    //Vector 3 where the selector was when enter was pressed most recently
    public Vector3 currentPosition;
    //Vector 3 of the new position of the selector
    public Vector3 newPosition;
    //Vector 3 of the velocity, which is always Vector3.Zero
    private Vector3 velocity;

    //Variable that keeps track of the menu game object
    private GameObject actionMenu;
    //Checks if the focus should be on menu
    private bool inMenu = false;
    //Checks if the focus should be on changing tile type
    private bool inTileChangeMode = false;
    //Checks if the focus should be on moving the cursor
    private bool inMovementMode = true;
    //Checks if the focus should be on unit movement
    private bool inUnitMovementMode = false;
    //Checks if the focus should be on selecting the unit to attack
    private bool inAttackMode = false;
    //List of buttons that might need to be accessed
    List<GameObject> buttonList;
    //List of selectable units and buildings within range
    List<GameCharacter> selectableUnits;
    List<GameBuilding> selectableBuildings;

    //The game character that has been selected
    public GameCharacter selectedUnit { get; set; }
    //The game building that has been selected
    public GameBuilding selectedBuilding { get; set; }

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
        this.selectableUnits = new List<GameCharacter>();
        this.selectableBuildings = new List<GameBuilding>();
        //Get instance of GameInformation class
        this.gi = GameObject.Find("ViewObject").GetComponent<GameInformation>();
    }

    #region updateFunction

    // Update is called once per frame
    void Update() {
        //GAME STATE: InMovementMode
        //Check if focus is on movement or selection
        if(this.inMovementMode) {
            //When left arrow or A is pressed
            if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                //Get coordinates (adjusted for the new movement) from the Game object this script is attached to
                int x = (int)this.transform.position.x;
                int z = (int)(this.transform.position.z + 1);
                //Check if the location based on the X, and Z coordinates is valid
                if(this.isSelectorMovementValid(x, z)) {
                    //Save old position
                    this.oldPosition = this.transform.position;
                    //Move the actual selector object
                    this.proceedSelecterMovement(x, z);
                }
            }
            //When right arrow or D is pressed
            if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                //Get coordinates (adjusted for the new movement) from the Game object this script is attached to
                int x = (int)this.transform.position.x;
                int z = (int)(this.transform.position.z - 1);
                //Check if the location based on the X, and Z coordinates is valid
                if(this.isSelectorMovementValid(x, z)) {
                    //Move the actual selector object
                    this.proceedSelecterMovement(x, z);
                }
            }
            //When up arrow or W is pressed
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                //Get coordinates (adjusted for the new movement) from the Game object this script is attached to
                int x = (int)(this.transform.position.x + 1);
                int z = (int)this.transform.position.z;
                //Check if the location based on the X, and Z coordinates is valid
                if(this.isSelectorMovementValid(x, z)) {
                    //Move the actual selector object
                    this.proceedSelecterMovement(x, z);
                }
            }
            //When down arrow or S is pressed
            if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                //Get coordinates (adjusted for the new movement) from the Game object this script is attached to
                int x = (int)(this.transform.position.x - 1);
                int z = (int)this.transform.position.z;
                //Check if the location based on the X, and Z coordinates is valid
                if(this.isSelectorMovementValid(x, z)) {
                    //Move the actual selector object
                    this.proceedSelecterMovement(x, z);
                }
            }

            //When user presses Enter
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKey("enter")) {
                //Check if in unit movement mode
                if(!this.inUnitMovementMode) {
                    //When not in unit movement mode, select a unit
                    this.SelectionInteraction();
                } else {
                    //When in unit movement mode, move the unit
                    this.doUnitMovement();
                }
            }

            //Check if an unit is selected, so the unit information doesn't dissapear while performing an action with it
            if(this.selectedUnit == null) {
                //show unit information
                this.getPlayerOnTileInformation((int)this.transform.position.x, (int)this.transform.position.z);
            }
            //Show tile information
            this.getTileInformation((int)this.transform.position.x, (int)this.transform.position.z);
            //Show building information
            this.getBuildingOnTileInformation((int)this.transform.position.x, (int)this.transform.position.z);

        } else {
            //GAME STATE: InTileChangeMode
            //Check if the focus is on tile changing ability
            if(this.inTileChangeMode) {

            }
        }


        //set inMenu variable to false when the action menu isn't active anymore
        if(!this.actionMenu.activeSelf) {
            this.inMenu = false;
        }

        //Update camera position if necessary
        this.updateCameraPosition();
    }

    #endregion updateFunction



    #region MovementRegion

    //Function to check if X, Z coordinate of the selecter is valid
    // - Param 1: X coordinate
    // - Param 2: Z coordinate
    public bool isSelectorMovementValid(int x, int z) {
        //Check if the selector goes in negative coordinates
        if(GameMap._GMinstance.isTileValid(x, z) == true) {
            //When tile exists, move select
            return true;            
        } else {
            //When tile doesn't exist, prevent movement
            return false;
        }
    }

    //Function to move selector object to location
    // - Param 1: X coordinate
    // - Param 2: Z coordinate
    public void proceedSelecterMovement(int x, int z) {
        //Create new vector3 with the new location of the selector
        this.newPosition = new Vector3(x, 0.35f, z);
        //Set the position of the selector
        this.transform.position = this.newPosition;
    }

    //Function to update camera position
    public void updateCameraPosition() {
        //Check if the camera is out of position compared to the selector game object
        if(this.cam.transform.position.x != this.transform.position.x || this.cam.transform.position.z != this.transform.position.z) {
            //Create new camera position
            Vector3 newCameraPosition = new Vector3((this.transform.position.x - 9), (this.transform.position.y + 12), (this.transform.position.z - 9));
            //Move the position of the camera to follow the selector object
            this.cam.transform.position = Vector3.SmoothDamp(this.cam.transform.position, newCameraPosition, ref this.velocity, 0.3F);
        }
    }

    //Function that enables movement mode
    public void resetKbcState() {
        this.inMenu = false;
        this.inMovementMode = true;
        this.inTileChangeMode = false;
        this.inUnitMovementMode = false;
        this.inAttackMode = false;
    }

    public void setUnitMovementState() {
        this.inMenu = false;
        this.inMovementMode = true;
        this.inTileChangeMode = false;
        this.inUnitMovementMode = true;
        this.inAttackMode = false;
    }

    public void setAttackState() {
        this.inMenu = false;
        this.inMovementMode = false;
        this.inTileChangeMode = false;
        this.inUnitMovementMode = false;
        this.inAttackMode = true;
    }

    //Function that has all unit movement logic
    //This function only works if an unit is selected
    public void doUnitMovement() {
        //Set coordinates where the unit needs to be moved to
        int x = (int)this.transform.position.x;
        int z = (int)this.transform.position.z;

        //Check if unit is already on said position
        if(this.selectedUnit.getXPosition() == x && this.selectedUnit.getZPosition() == z) {
            //Change focus to menu
            this.inMovementMode = false;
            this.inUnitMovementMode = false;
            this.inMenu = true;
            //Show menu
            this.setMenuFocus(true);
            //Show buttons on the menu
            this.selectedUnit.showAbilities();
            return;
        }

        //Do the movement check
        if(this.checkUnitMovement(x, z)) {

            //Do movement
            this.selectedUnit.moveUnitTo(GameMap._GMinstance.getTileAt(this.selectedUnit.getXPosition(), this.selectedUnit.getZPosition()), GameMap._GMinstance.getTileAt(x, z));

            //Remove the graph overlayed on the tiles
            this.selectedUnit.getCharacterAI().removePossibleMovesGraph();
            //Set the color of the unit
            this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.white;

            //Change focus to menu
            this.inMovementMode = false;
            this.inUnitMovementMode = false;
            this.inMenu = true;
            //Show menu
            this.setMenuFocus(true);
            //Show buttons on the menu
            this.selectedUnit.showAbilities();
            return;
        }
    }

    //Function that checks if move is valid
    public bool checkUnitMovement(int x, int z) {
        //Check if there is an selected unit
        if(this.selectedUnit == null) return false;
        //Check if there is a unit on that location already
        if(Game.getCharactersOnPosition(x, z).Count > 0) {
            //Remove moves graph, change the unit to neutral state and remove all buttons
            this.selectedUnit.getCharacterAI().removePossibleMovesGraph();
            this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.white;
            ActionMenu._instance.removeButtonsFromMenu();
            //Select new unit
            this.SelectionInteraction();
            return false;
        }
        //Check if move is reachable
        if(!this.selectedUnit.getCharacterAI().isReachable(GameMap._GMinstance.getTileAt(x, z))) {
            //Remove moves graph, change the unit to neutral state and remove all buttons
            this.selectedUnit.getCharacterAI().removePossibleMovesGraph();
            this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.white;
            ActionMenu._instance.removeButtonsFromMenu();
            this.selectedUnit = null;
            return false;
        }
        //If none of the checks return false, then all requirements are met
        return true;
    }

    #endregion MovementRegion



    #region SelectRegion

    //Function that handles the selection interaction
    public void SelectionInteraction() {
        //Get the X and Z coordinates
        int x = Mathf.RoundToInt(this.transform.position.x);
        int z = Mathf.RoundToInt(this.transform.position.z);

        //Reset selected buildings and unit
        this.selectedUnit = null;
        this.selectedBuilding = null;

        //Add all entities on a tile. Need to know this so the system knows if a special character selection should pop up
        int totalEntitiesOntile = 0;
        totalEntitiesOntile += Game.getCharactersOnPosition(x, z).Count;
        GameBuilding gBuilding = Game.getBuildingOnPosition(x, z);
        if(gBuilding != null) totalEntitiesOntile++;

        //Check if no entities on tile are found
        if(totalEntitiesOntile == 0) return;
        //Check if only one entity is found
        if(totalEntitiesOntile == 1) {
            //Check if entity is unit or building
            if(Game.getBuildingOnPosition(x, z) != null) {
                //Check if building isn't already locked
                if(gBuilding.isLocked == false) {
                    //Set the building as the selected building
                    this.selectedBuilding = Game.getBuildingOnPosition(x, z);
                    //Show building menu
                    this.selectedBuilding.showBuildingMenu();
                } else {
                    this.resetKbcState();
                }
            } else {
                //Get the unit entity on position
                GameCharacter gChar = Game.getCharacterOnPosition(x, z);
                //Check if unit isn't already locked
                if(gChar.isUnitLocked() == false) {
                    //Set that unit as the selected unit.
                    this.selectedUnit = gChar;
                    //Call select unit
                    this.selectUnit(x, z);
                } else {
                    this.resetKbcState();
                }
            }
            
        }
        //Check if more than one entity is found, and selection menu should show up
        if(totalEntitiesOntile > 1) {
            //Create list where the characters will be stored temporary
            Dictionary<int, string> entityList = new Dictionary<int, string>();
            foreach(GameCharacter gChar in Game.getCharactersOnPosition(x, z)) {
                //check if character isn't already locked
                if(!gChar.isUnitLocked()) {
                    //Add the character to the list
                    entityList.Add(gChar.characterInstanceId, gChar.characterName);
                }
            }
            //Check if there was a building on this tile
            if(gBuilding != null) {
                //Add the building to the list
                entityList.Add(gBuilding.buildingInstanceID, gBuilding.buildingName);
            }

            if(entityList.Count > 0) {
                //Show menu with all the options
                ActionMenu._instance.showSelectMenu(entityList);
            } else {
                Debug.LogError("No entities to select");
            }
        }
    }

    //Function that shows select menu
    public void selectUnitFromMenu(int SelectableInstanceID) {
        //Get selectable unit from instanceID
        foreach(GameCharacter gChar in Game._GameInstance.player.getUnitList()) {
            if(gChar.characterInstanceId == SelectableInstanceID) {
                this.selectUnit(gChar);
            }
        }
        //Get selectable building from InstanceID
        foreach(GameBuilding gBuilding in Game._GameInstance.player.getBuildingList()) {
            if(gBuilding.buildingInstanceID == SelectableInstanceID) {
                this.selectbuilding(gBuilding);
            }
        }
    }
    
    //Function that selects unit and generates a moves graph for it
    public void selectUnit(int x, int z) {
        //Change color of the unit to indicate that the unit has been selected
        //In the future you might want to make this an glow behind it, instead.
        this.selectedUnit.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.blue;
        //Generate the possible moves there are for the selected unit
        this.selectedUnit.getCharacterAI().generatePossibleMovesGraph(GameMap._GMinstance.getTileAt(x, z), selectedUnit.movementPoints);
        //Set unit movement mode to true
        this.inUnitMovementMode = true;
    }

    //Function that selects unit and generates a moves graph for it
    public void selectUnit(GameCharacter gChar) {
        //Change color of the unit to indicate that the unit has been selected
        //In the future you might want to make this an glow behind it, instead.
        gChar.getCharacterObject().GetComponent<MeshRenderer>().material.color = Color.blue;
        //Generate the possible moves there are for the selected unit
        gChar.getCharacterAI().generatePossibleMovesGraph(GameMap._GMinstance.getTileAt(gChar.getXPosition(), gChar.getZPosition()), selectedUnit.movementPoints);
        //Set unit movement mode to true
        this.inUnitMovementMode = true;
    }

    //Funciton that select building
    public void selectbuilding(GameBuilding gBuilding) {
        //Change color of the building to indicate that the building is selected
        //In the future you might want to make this a glow behind it, instead
        gBuilding.getBuildingModel().getGameObject().GetComponent<MeshRenderer>().material.color = Color.blue;
        //Show building menu
        GameInformation._instance.showBuildingInterface();        
    }

    //Function to reset all highlights
    public void resetHighlightedUnit() {
        this.selectedUnit.unitDone();
        this.selectedUnit = null;
    }

    #endregion SelectRegion

    
    
    
    #region Menu

    //Function that shows/hides action menu and continues game
    public void setMenuFocus(bool focus) {
        this.actionMenu.SetActive(focus);
    }
    //Function that toggles action menu
    public void toggleMenuFocus() {
        if(this.actionMenu.activeSelf) {
            this.actionMenu.SetActive(false);
        } else {
            this.actionMenu.SetActive(true);
        }
    }
    //Function to find the characters the unit is able to fight
    public void setSelectMode() {
        //Clear lists before filling them
        this.selectableUnits.Clear();
        this.selectableBuildings.Clear();
        
        //TODO: Allow for ranged units to hit further away

        //Create the list of attackable units
        if(this.selectedUnit.attackRange == 1) {
            //Check for units in the 4 tiles around the unit
            if(Game.getCharacterOnPosition((this.selectedUnit.getXPosition() + 1), this.selectedUnit.getZPosition()) != null) {
                //Add unit to list of selectables
                this.selectableUnits.Add(Game.getCharacterOnPosition((this.selectedUnit.getXPosition() + 1), this.selectedUnit.getZPosition()));
            }
            if(Game.getCharacterOnPosition((this.selectedUnit.getXPosition() - 1), this.selectedUnit.getZPosition()) != null) {
                //Add unit to list of selectables
                this.selectableUnits.Add(Game.getCharacterOnPosition((this.selectedUnit.getXPosition() - 1), this.selectedUnit.getZPosition()));
            }
            if(Game.getCharacterOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() + 1)) != null) {
                //Add unit to list of selectables
                this.selectableUnits.Add(Game.getCharacterOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() + 1)));
            }
            if(Game.getCharacterOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() - 1)) != null) {
                //Add unit to list of selectables
                this.selectableUnits.Add(Game.getCharacterOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() - 1)));
            }
        }
        //Create the list of attackable buildings
        if(this.selectedUnit.attackRange == 1) {
            //Check for units in the 4 tiles around the unit
            if(Game.getBuildingOnPosition((this.selectedUnit.getXPosition() + 1), this.selectedUnit.getZPosition()) != null) {
                //Add unit to list of selectables
                this.selectableBuildings.Add(Game.getBuildingOnPosition((this.selectedUnit.getXPosition() + 1), this.selectedUnit.getZPosition()));
            }
            if(Game.getBuildingOnPosition((this.selectedUnit.getXPosition() - 1), this.selectedUnit.getZPosition()) != null) {
                //Add unit to list of selectables
                this.selectableBuildings.Add(Game.getBuildingOnPosition((this.selectedUnit.getXPosition() - 1), this.selectedUnit.getZPosition()));
            }
            if(Game.getBuildingOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() + 1)) != null) {
                //Add unit to list of selectables
                this.selectableBuildings.Add(Game.getBuildingOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() + 1)));
            }
            if(Game.getBuildingOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() - 1)) != null) {
                //Add unit to list of selectables
                this.selectableBuildings.Add(Game.getBuildingOnPosition(this.selectedUnit.getXPosition(), (this.selectedUnit.getZPosition() - 1)));
            }
        }

        if(this.selectableUnits.Count > 0 || this.selectableBuildings.Count > 0) {
            this.setAttackState();
        }
    }

    //Check if player is on the same tile and show said information
    public void getPlayerOnTileInformation(int x, int z) {
        //Get the unit from position
        GameCharacter selectedCharacter = Game.getCharacterOnPosition(x, z);
        //Show that unit information
        this.gi.showCharacterOnTile(selectedCharacter);
    }

    //Show tile information
    public void getTileInformation(int x, int z) {
        //Get tile on position
        GameTile tile = GameMap._GMinstance.getTileAt(x, z);
        //Show tile information
        this.gi.showTileInformation(tile);
    }

    //Show  building information
    public void getBuildingOnTileInformation(int x, int z) {
        //Get building on position
        GameBuilding building = Game.getBuildingOnPosition(x, z);
        //Show that building information
        this.gi.showBuildingOnTile(building);
    }

    #endregion Menu

}
