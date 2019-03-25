//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using UnityEngine.Events;

public class GameBuilding {

    //Variable where the Character model class will be stored
    protected BuildingModel bldModel;
    //The getter of the variable. There is no setter as it should never change!
    public BuildingModel getBuildingModel() {
        return this.bldModel;
    }
    //Variable where the Character Interaction class will be stored
    protected BuildingInteraction bldInteraction;
    //The getter of the variable. There is no setter as it should never change!
    public BuildingInteraction getBuildingInteraction() {
        return this.bldInteraction;
    }
    
    //Get the instance of this class
    public GameBuilding getInstance() { return this; }

    //Building instance id
    public int buildingInstanceID;

    // Basic building information
    // 1. Building ID
    // 2. Building name
    // 3. hitpoints of the building
    // 4. Base defense
    public int buildingId;
    public string buildingName;
    public int hitPoints;
    public int baseDefense;
    public int baseViewRange;

    // Building type information
    // 4. Building type string
    // 5. List of all possible units that can be created by this type of building
    public string buildingType;
    public List<int> buildableUnits;

    //Game management information
    // 1. Boolean that keeps track if the building is usable
    // 2. Boolean that keeps track if the building is unlocked to build
    // 3. List of resources with cost to build the building
    // 4. The amount of points it costs to unlock the building
    // 5. ID of the building that replacese this one
    public bool isLocked;
    public bool isUnLocked;
    public Dictionary<string, int> costToBuild { get; protected set; }
    public int costToUnlock { get; protected set; }
    public int isReplacedBy { get; protected set; }
    public int researchId { get; protected set; }


    //Constructor
    public GameBuilding(string bType, bool example = false) {
        this.buildingType = bType;
        //Get Building data from XML file
        this.getXMLData();
        //Update total entities amount
        Game.totalEntities++;
        //Get instance ID of building
        this.buildingInstanceID = Game.newUnitInstanceID();
        //Instantiate Building model class
        this.bldModel = new BuildingModel();
        //Instantiate building interaction class
        this.bldInteraction = new BuildingInteraction();
        if(example == false) {
            this.bldModel.createGameObject(this.buildingName);
            this.updatePositionOfModel();
        }
    }
   

    // Properties of the X Position
    public int getXPosition() { return this.bldInteraction.x; }
    public void setXPosition(int x) {
        this.bldInteraction.x = x;
        this.bldModel.setGameObjectPosition(x, this.bldInteraction.z);
    }

    //Properties of the Z Position
    public int getZPosition() { return this.bldInteraction.z; }
    public void setZPosition(int z) {
        this.bldInteraction.z = z;
        this.bldModel.setGameObjectPosition(this.bldInteraction.x, z);
    }

    //Update location of the model
    public void updatePositionOfModel() {
        this.getBuildingModel().setGameObjectPosition(this.getXPosition(), this.getZPosition());
    }

    //Set the lock status of the unit
    public void setBuildingLocked(bool lockStatus) {
        this.isLocked = lockStatus;
    }
    //Toggle lock status of the unit
    public void toggleBuildingLocked() {
        if(this.isLocked) { this.isLocked = false; } 
        else { this.isLocked = true; }
    }
    //Get lock status
    public bool getBuildingLockAfterUse() {
        return this.isLocked;
    }

    //Set unlocked status
    public void unlockBuilding() {
        this.isUnLocked = true;
    }
    //Get unlocked status
    public bool isBuildingUnlocked() {
        return this.isUnLocked;
    }

    //Show building menu options
    public void showBuildingMenu() {

    }

    //Retreive building data from data files by type
    private void getXMLData() {
        //Code to load unit from XML
        //creates new XmlDocument instance
        XmlDocument rawXML = new XmlDocument();

        //Create the URL to the unit specific data file
        string dataURL = Application.dataPath + "/Data/Buildings/" + this.buildingType + ".xml";
        //Loads file into the instance of XmlDocument
        rawXML.Load(dataURL);

        //Get building ID and Parse to int
        this.buildingId = Int32.Parse(rawXML.GetElementsByTagName("id")[0].ChildNodes[0].Value);
        //Get building name
        this.buildingName = rawXML.GetElementsByTagName("name")[0].ChildNodes[0].Value;
        //Get building type
        this.buildingType = rawXML.GetElementsByTagName("type")[0].ChildNodes[0].Value;

        //Get all elements within the tag 'base'
        XmlNodeList baseTag = rawXML.GetElementsByTagName("base");
        XmlNodeList baseStats = baseTag[0].ChildNodes;
        //Loop through XML nodes in the Base stats node
        foreach(XmlNode stat in baseStats) {
            //Check if child node has the tag 'hitpoints'
            if(stat.Name == "hitpoints") {
                //Set hitpoints to the amount in Data file
                this.hitPoints = Int32.Parse(stat.InnerXml);
            }
            //Check if child node has the tag 'defense'
            if(stat.Name == "defense") {
                //Set defense to the amount in Data file
                this.baseDefense = Int32.Parse(stat.InnerXml);
            }
            //Check if child node has the tag 'viewrange'
            if(stat.Name == "viewrange") {
                //Set viewrange to the amount in Data file
                this.baseViewRange = Int32.Parse(stat.InnerXml);
            }
        }

        //Create the buildables list
        this.buildableUnits = new List<int>();
        //Get all elements within the tag 'buildable'
        XmlNodeList buildableTag = rawXML.GetElementsByTagName("buidable");
        XmlNodeList buildables = baseTag[0].ChildNodes;
        //Loop through XML nodes in the Base stats node
        foreach(XmlNode buildable in buildables) {
            //Add the buildable to the list
            this.buildableUnits.Add(Int32.Parse(buildable.InnerXml));
        }

        //Get IsUnlocked value
        this.isLocked = Boolean.Parse(rawXML.GetElementsByTagName("isunlocked")[0].ChildNodes[0].Value);
        //Get costToUnlock value
        this.costToUnlock = Int32.Parse(rawXML.GetElementsByTagName("costtounlock")[0].ChildNodes[0].Value);
        //Get at what research this building is buildable
        this.researchId = Int32.Parse(rawXML.GetElementsByTagName("research")[0].ChildNodes[0].Value);

        //Set how much it costs to build
        //Create new cost List
        this.costToBuild = new Dictionary<string, int>();
        //Get the list of cost tags
        XmlNodeList costList = rawXML.GetElementsByTagName("cost");
        XmlNodeList costContent = costList[0].ChildNodes;
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
    }
}
