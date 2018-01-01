using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Xml;

public abstract class GameCharacter {
	//Basis information about unit
	string characterId { get; set; }
	string characterName { get; set; }
	string description { get; set; }

	//Base stats of unit
	int hitPoints { get; set; }
	int baseDamage { get; set; }
	int baseDefense { get; set; }
	int movementPoints { get; set; }
	int viewRange { get; set; }
	int attackRange { get; set; }

	//Abilities of unit
	string characterAbility;
	List<string> abilities;
	string characterType;
	Type gameCharacterType;
	Dictionary<string, double> minorBonuses;
	Dictionary<string, double> majorBonuses;

	//Statistics of unit
	int battleEntered { get; set; }
	bool isUnlocked { get; set; }
	Dictionary<string, int> costToBuild { get; set; }
	int costToUnlock { get; set; }
	int researchId { get; set; }
	int isReplacedBy { get; set; }

	//Position & interaction
	int x { get; set; }
	int y { get; set; }

	//Constructor
	public GameCharacter(string type, int x = 0, int y = 0){
		//TODO: Actually generate unit with number from scenario
		this.x = x;
		this.y = y;
		gameCharacterType = this.GetType();
		getXMLData (type);
	}

	//Executes the given ability
	private void abilityCallBack(string ability){
		MethodInfo executeAbility = gameCharacterType.GetMethod(ability);
		executeAbility.Invoke(this, null);
	}


	public void Pillage(int x, int y){
		//Remove building from tile if it is an Farm
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
				this.minorBonuses.Add(bonus.InnerXml, 1.33);
			} else if (bonus.Name == "major") {
				this.majorBonuses.Add(bonus.InnerXml, 1.50);
			}
		}

		Debug.Log (this.minorBonuses["Siege"]);
	}
}
