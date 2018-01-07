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
using System.Xml;

public class GameCharacter {
	//Get the instance of the ActionMenu stored here, since we want to add abilities to it depending on the situation
	protected ActionMenu am;
	//The getter of the variable
	public ActionMenu getMenuModel(){
		return this.am;
	}

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


	//Variable where the Keyboardcontrol class will be stored
	protected KeyBoardControls kbc;
	//The property of the variable
	public KeyBoardControls keyboardControls {
		get {
			//Check if the keyboard controls instance is empty
			if(kbc == null) {
				//Get instance of KeyboardControls class
				kbc = KeyBoardControls._instance;
			}
			return kbc;
		}
		set {
			kbc = value;
		}
	}

	//Basis information about unit
	// 1. Character id stores the id of the unit (This id is unique to the unit)
	// 2. The name of the unit
	// 3. The description of the unit (Only used for the library of units)
	public string characterId { get; protected set; }
	public string characterName { get; protected set; }
	public string description { get; protected set; }

	//Base stats of unit
	// 1. The hit points the unit has
	// 2. Base damage. The damage without any multiplier (static)
	// 3. Base defense. The defense without any multiplier (static)
	// 4. Amount of points for moving (static)
	// 5. Base range for view. This is without any multipliers (static)
	// 6. Base range for attacking. this is without multipliers (static)
	public int hitPoints { get; protected set; }
	public int baseDamage { get; protected set; }
	public int baseDefense { get; protected set; }
	public int movementPoints { get; protected set; }
	public int viewRange { get; protected set; }
	public int attackRange { get; protected set; }

	//Abilities of unit
	// 1. The passive abilities, that allows / disallows certain things (movement etc.)
	// 2. List of special abilities that the user can activate under certain conditions
	// 3. Type of character, which ties with the passive ability (Pikemen have bonus against cavalry, for example)
	// 4. List of minor bonuses (Multipliers for attack, defense etc.)
	// 5. List of major bonuses (Multipliers for attac,k defense etc.)
	public string characterAbility { get; protected set; }
	public List<string> abilities { get; protected set; }
	public List<string> abilityList { get; protected set; }
	public string characterType { get; protected set; }
	public Type gameCharacterType { get; protected set; }
	public Dictionary<string, double> minorBonuses { get; protected set; }
	public Dictionary<string, double> majorBonuses { get; protected set; }

	//Statistics of unit
	// 1. Keep track of how many battles it has entered (Possible level up system?)
	// 2. Boolean that keeps track if the unit has been unlocked to play
	// 3. Lst of resources with cost to create the unit
	// 4. Id of the technology that needs to be researched to create this character
	// 5. Id of the unit that replaces this unit
	public int battleEntered { get; set; }
	public bool isUnlocked { get; set; }
	public Dictionary<string, int> costToBuild { get; set; }
	public int costToUnlock { get; protected set; }
	public int researchId { get; protected set; }
	public int isReplacedBy { get; protected set; }

	//Constructor
	// Param 1: name of the data file in string format
	public GameCharacter (string type) {
		string fileName = type + ".xml";
		//Get Game Character Data from XML file
		this.getXMLData(fileName);
		//Instantiate CharacterModel class
		this.charModel = new CharacterModel();
		this.getCharacterObject().name = this.characterName;
		//Instantiate CharacterInteraction class
		this.charInteraction = new CharacterInteraction();
		//Set start coordinates of unit (This will depend on scenario later)
		this.charInteraction.x = 5;
		this.charInteraction.z = 5;
		//Set previous Coordinates of unit
		this.charInteraction.backupPosition();
		//Instantiate CharacterAI class
		this.charAI = new CharacterAI();
		//Get instance of ActionMenu class
		this.am = GameObject.Find("actionMenu").GetComponent<ActionMenu>();
	}
		
	// Properties of the X Position
	public int getXPosition() { return this.charInteraction.x; }
	public void setXPosition(int x) { 
		this.charInteraction.backupX();
		this.charInteraction.x = x; 
		this.charModel.setGameObjectPosition(x, this.charInteraction.z);
	}

	//Properties of the Y Position
	public int getZPosition(){ return this.charInteraction.z; }
	public void setZPosition(int z) { 
		this.charInteraction.backupZ();
		this.charInteraction.z = z; 
		this.charModel.setGameObjectPosition(this.charInteraction.x, z);
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
				//Set the old position into the previous variables before updating the new one
				this.charInteraction.backupPosition();
				//Update position in the interaction class
				this.charInteraction.x = newPosition.X;
				this.charInteraction.z = newPosition.Z;
			}
		} else {
			//ERROR
			Debug.Log("Can't reach this!");
		}
	}

	//Set the lock status of the unit
	public void setUnitLocked(bool unitLock){
		this.charInteraction.IsLocked = unitLock;
	}
	//Get the lock status of the unit
	public bool isUnitLocked(){
		return this.charInteraction.IsLocked;
	}
		
	//Function that will be called when one of the ability buttons is clicked
	//Done is one of the basic unit movement abilities
	public void unitDone(){
		//Locks the unit in place (no more actions available)
		this.setUnitLocked(true);
		//Remove the buttons from the action menu
		ActionMenu._instance.removeButtonsFromMenu();
		//Set the menu focus to false, basically disable it
		this.keyboardControls.updateMenuFocus();
		//Change color of the unit
		this.charModel.getGameObject().GetComponent<MeshRenderer>().material.color = Color.gray;
		//Remove graph
		this.charAI.removePossibleMovesGraph();
		//Set selected unit to null
		this.keyboardControls.selectedUnit = null;
	}

	//Function that will be called when one of the ability buttons is clicked
	//Undo is one of the basic unit movement abilities
	public void unitUndo(){
		//Reset the position to the previous position
		this.charInteraction.resetPosition();
		//Sets the game object position to the restored position
		this.charModel.setGameObjectPosition(this.charInteraction.x, this.charInteraction.z);
		//Set the position of the selector and camera
		this.keyboardControls.updatePosition((float)this.charInteraction.x, keyboardControls.transform.position.y, (float)this.charInteraction.z);
		//Remove the buttons from the action menu
		ActionMenu._instance.removeButtonsFromMenu();
		//Set the menu focus to false, basically disable it
		this.keyboardControls.updateMenuFocus();
		//Reselect unit and show movement overlay
		this.keyboardControls.selectUnit(this.charInteraction.x, this.charInteraction.z);
	}

	//Function that will be called when one of the ability buttons is clicked
	//Pillage is one of the basic unit abilities
	public void unitPillage(){
		//TODO: Remove the actual farm
		ActionMenu._instance.removeButtonsFromMenu();
		this.keyboardControls.updateMenuFocus();
	}

	//Function that will be called when the Attack ability button is clicked
	//Attacks is one of the basic unit abilities
	public void unitAttack(){
		//TODO: Attack the unit
		ActionMenu._instance.removeButtonsFromMenu();
		this.keyboardControls.updateMenuFocus();
	}



























	//Function that passes through and adds all necessary information for showing update menu
	//Also checks if ability is valid where the unit is placed
	public void showAbilities() {
		//Clear list of buttons
		this.abilityList.Clear();

		//Check if there is any unit to attack
		if(CharacterPikemen.checkIfAttackIsValid(this)) {
			if(!this.abilityList.Contains("Attack")) this.abilityList.Add("Attack");
		}

		//Loop through abilities and check if they are valid for this position
		foreach(string ability in this.abilities) {
			//Check if the ability is usable in this spot
			//TODO: check which unit this is and check then if if the ability is available
			if(CharacterPikemen.checkIfAbilityIsValid(ability)) {
				//Add the ability to the list
				if(!this.abilityList.Contains(ability))	this.abilityList.Add(ability);
			}
		}

		//Check if there is a farm on the ground
		if(CharacterPikemen.checkIfAbilityIsValid("Pillage")) {
			if(!this.abilityList.Contains("Pillage")) this.abilityList.Add("Pillage");
		}

		//Add the undo and done as last
		//Always checking if the ability hasn't already been added
		if(!this.abilityList.Contains("Undo")) this.abilityList.Add("Undo");
		if(!this.abilityList.Contains("Done")) this.abilityList.Add("Done");

		//Show the actual menu on screen
		this.am.updateMenu(this, abilityList);
	}


	//Retrieves all data from the XML file by type
	private void getXMLData(string cType){
		//Code to load unit from XML
		//creates new XmlDocument instance
		XmlDocument rawXML = new XmlDocument();

		//Create the URL to the unit specific data file
		string dataURL = Application.dataPath + "/Data/Units/" + cType;
		//Loads file into the instance of XmlDocument
		rawXML.Load (dataURL);

		this.characterId = rawXML.GetElementsByTagName("id") [0].ChildNodes [0].Value;
		this.characterName = rawXML.GetElementsByTagName("name")[0].ChildNodes[0].Value;
		this.characterType = rawXML.GetElementsByTagName("type")[0].ChildNodes[0].Value;

		//Get all elements within the XML with the tag 'base'
		XmlNodeList baseTag = rawXML.GetElementsByTagName ("base");
		XmlNodeList baseStats = baseTag[0].ChildNodes;
		//Loop through XML nodes in the base node
		foreach (XmlNode stats in baseStats) {
			//Check if child node has the tag 'hitpoints'
			if (stats.Name == "hitpoints") {
				//Set hitpoints to the amount in Data file
				this.hitPoints = Int32.Parse(stats.InnerXml);
			}
			//Check if child node has the tag 'damage'
			if (stats.Name == "damage") {
				//Set damage to the amount in Data file
				this.baseDamage = Int32.Parse (stats.InnerXml);
			}
			//Check if child node has the tag 'defense'
			if (stats.Name == "defense") {
				//Set defense to the amount in Data file
				this.baseDefense = Int32.Parse (stats.InnerXml);
			}
			//Check if child node has the tag 'movement'
			if (stats.Name == "movement") {
				//Set movement to the amount in Data file
				this.movementPoints = Int32.Parse (stats.InnerXml);
			}
			//Check if child node has the tag 'viewrange'
			if (stats.Name == "viewrange") {
				//Set viewrange to the amount in Data file
				this.viewRange = Int32.Parse (stats.InnerXml);
			}
			//Check if child node has the tag 'attackrange'
			if (stats.Name == "attackrange") {
				//Set attackrange to the amount in Data file
				this.attackRange = Int32.Parse (stats.InnerXml);
			}
		}

		//Get special ability
		this.characterAbility = rawXML.GetElementsByTagName("special")[0].ChildNodes[0].Value;

		//Create new ability List
		this.abilities = new List<string>();
		this.abilityList = new List<string>();
		//Add basic abilities
		this.abilities.Add("Attack");

		//Get the list of ability tags
		XmlNodeList abilityList = rawXML.GetElementsByTagName ("abilities");
		XmlNodeList abilityContent = abilityList[0].ChildNodes;
		//Loop through XML nodes in the abilities node
		foreach(XmlNode ability in abilityContent) {
			//Check if the added ability node is actually an ability
			if(ability.Name == "ability") {
				//Add the ability to the list
				this.abilities.Add(ability.InnerXml);
			}
		}

		//Set wether the unit is unlocked to play
		string tempUnlocked = rawXML.GetElementsByTagName("isunlocked")[0].ChildNodes[0].Value;
		if (tempUnlocked == "true") {
			this.isUnlocked = true;
		} else {
			this.isUnlocked = false;
		}
		//Set how much it costs to unlock it (even if it is already unlocked by default)
		this.costToUnlock = Int32.Parse(rawXML.GetElementsByTagName("costtounlock")[0].ChildNodes[0].Value);
		//Set at what research this unit is buildable
		this.researchId = Int32.Parse(rawXML.GetElementsByTagName("research")[0].ChildNodes[0].Value);

		//Set how much it costs to build
		//Create new cost List
		this.costToBuild = new Dictionary<string, int>();
		//Get the list of cost tags
		XmlNodeList costList = rawXML.GetElementsByTagName ("cost");
		XmlNodeList costContent = abilityList[0].ChildNodes;
		//Loop through XML nodes in the cost node
		foreach(XmlNode cost in costContent) {
			//Check if the added cost node is actually a valid resource
			if(cost.Name == "food") {
				//Add the cost to the list
				this.costToBuild.Add("food", Int32.Parse(cost.InnerXml));
			}
			//Check if the added cost node is actually a valid resource
			if(cost.Name == "iron") {
				//Add the cost to the list
				this.costToBuild.Add("iron", Int32.Parse(cost.InnerXml));
			}
			//Check if the added cost node is actually a valid resource
			if(cost.Name == "gold") {
				//Add the cost to the list
				this.costToBuild.Add("gold", Int32.Parse(cost.InnerXml));
			}
		}

		//TODO: Make function that actually replaces the units when no longer necessary!
		this.isReplacedBy = Int32.Parse(rawXML.GetElementsByTagName("replacedby")[0].ChildNodes[0].Value);

		//create new bonuses lists
		this.minorBonuses = new Dictionary<string, double>();
		this.majorBonuses = new Dictionary<string, double>();
		//Get the list of bonuses
		XmlNodeList bonusList = rawXML.GetElementsByTagName("bonuses");
		XmlNodeList bonuses = bonusList[0].ChildNodes;
		//Loop through all bonuses
		foreach (XmlNode bonus in bonuses) {
			if (bonus.Name == "minor") {
				//Adds the minor damage bonus.
				//The damage variable will be multiplied with this bonus for the total damage result
				this.minorBonuses.Add(bonus.InnerXml, 1.33);
			} else if (bonus.Name == "major") {
				//Adds the major damage bonus.
				//The damage variable will be multiplied with this bonus for the total damage result
				this.majorBonuses.Add(bonus.InnerXml, 1.50);
			}
		}
	}
}

