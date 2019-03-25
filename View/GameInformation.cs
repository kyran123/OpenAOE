//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//
// The Game Information View class, puts all information on screen
//
public class GameInformation : MonoBehaviour {

    //Tile information
    public GameObject tileImageObject;
    public Text TileTypeInformation;
    public Text TileName;
    //Tile images
    public Sprite tileGrass;
    public Sprite tileSea;
    public Sprite tileMountains;
    public Sprite tileDesert;
    public Sprite tileForest;

    //Character information
    public Text CharacterTypeInformation;
    public Text CharacterNameInformation;
    public Text CharacterHitPointsInformation;
    public Text CharacterMovementInformation;
    public Text CharacterBattlesEnteredInformation;
    //Character images
    public GameObject unitImageObject;
    public Sprite PikemenImage;
    public Sprite VillagerImage;

    //Building information
    public Text buildingTypeInformation;
    public Text buildingHitPointsInformation;
    //Building images
    public GameObject buildingImageObject;
    public Sprite townHallImage;
    public Sprite towerImage;
    public Sprite barracksImage;


    //The instance of this class
    public static GameInformation _instance;

    // Use this for initialization
    void Start () {
        //Set this class to the static instance
        GameInformation._instance = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showBuildingOnTile(GameBuilding building) {
        //Check if building isn't empty
        if(building == null) {
            //Set text to empty
            this.buildingTypeInformation.text = "";
            this.buildingHitPointsInformation.text = "";
            //Set image to empty
            buildingImageObject.GetComponent<Image>().sprite = null;
            //Stop function if building is empty
            return;
        }
        //Set basic building info
        this.buildingTypeInformation.text = building.buildingName.ToString();
        this.buildingHitPointsInformation.text = "Hitpoints: " + building.hitPoints.ToString();
        //Set building image
        switch(building.buildingType) {
            case "towncenter":
                this.buildingImageObject.GetComponent<Image>().sprite = this.townHallImage;
                break;
            case "barrack":

                break;

            case "tower":

                break;
        }
    }

    public void showCharacterOnTile(GameCharacter character) {
        //Check if character isn't empty
        if(character == null) {         
            //Set text to empty
            this.CharacterTypeInformation.text = "";
            this.CharacterNameInformation.text = "";
            this.CharacterHitPointsInformation.text = "";
            this.CharacterMovementInformation.text = "";
            this.CharacterBattlesEnteredInformation.text = "";
            //Set image to null
            unitImageObject.GetComponent<Image>().sprite = null;
            //Stop function if character is empty
            return;
        }
        //Set basic unit information
        this.CharacterTypeInformation.text = character.characterType;
        this.CharacterNameInformation.text = character.characterName;
        //Set unit stats
        this.CharacterHitPointsInformation.text = "Hitpoints: " + character.hitPoints.ToString();
        this.CharacterMovementInformation.text = "Movement: " + (character.movementPoints - character.movementPointsUsed).ToString();
        this.CharacterBattlesEnteredInformation.text = "Experience: " + character.battleEntered.ToString();
        //Set unit pictures
        switch(character.characterName.ToLower()) {
            case "pikemen":
                unitImageObject.GetComponent<Image>().sprite = this.PikemenImage;
                break;
            case "villager":
                unitImageObject.GetComponent<Image>().sprite = this.VillagerImage;
                break;
        }
    }

    //Public function that updates the information on the tile that the user is hovering over
    // - Param 1: Give the GameTile that the user is hovering over
    public void showTileInformation(GameTile tile) {
        //Check if the tile isn't empty
        if(tile == null) {
            //Stop function if tile is empty
            return;
        }

        //Set name of tile
        this.TileName.text = tile.getTileTypeName();
        //Set image panel
        switch(tile.getTileTypeName()) {
            case "Sea":
                tileImageObject.GetComponent<Image>().sprite = this.tileSea;
                break;
            case "Grass":
                tileImageObject.GetComponent<Image>().sprite = this.tileGrass;
                break;
            case "Mountains":
                tileImageObject.GetComponent<Image>().sprite = this.tileMountains;
                break;
            case "Desert":
                tileImageObject.GetComponent<Image>().sprite = this.tileDesert;
                break;
            case "Forest":
                tileImageObject.GetComponent<Image>().sprite = this.tileForest;
                break;
        }
        //Show movement cost in the footer of the information panel
        this.TileTypeInformation.text = "Movement cost: " + tile.TerrainModifier + "\n";
    }

    //Public function that shows building information, like the units you can create, upgrades you can apply
    public void showBuildingInterface() {

    }
}
