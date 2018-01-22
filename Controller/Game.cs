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
using System.Xml.Serialization;

//
// The main controller
//
public class Game : MonoBehaviour {

	//Private instance variable
	protected static Game _GameInstance { get; set; }

	//Save sprites here

	//Color Palette:
	//Grass - #3CC453
	//Tree (Bright green) - #40C224
	//Tree (Dark) - #38A61F
	public Sprite tileOverlay;
	//Store instance of the map
	private GameMap gameMap;
	//Store instances of GameCharacters
	public List<GameCharacter> unitList;

	// Use this for initialization
	void Start () {

		//Test random tiles
		//gameMap.RandomizeTiles();
		Game._GameInstance = this;

		//Code to load map from XML
		//TODO: The file changes depending on chosen map!
		//creates new XmlDocument instance
		XmlDocument rawXML = new XmlDocument();

		//Loads file into the instance of XmlDocument
		rawXML.Load (Application.dataPath + "/Data/Maps/flatlandmap.xml");

		//Get all elements within the XML with the tag 'Tile'
		XmlNodeList TileList = rawXML.GetElementsByTagName ("Tile");

		//Get the required map dimensions from the XML file.
		//NOTE: These have to be placed in the top of the document!
		int mapWidth = Int32.Parse(rawXML.GetElementsByTagName ("Width")[0].ChildNodes[0].Value);
		int mapHeight = Int32.Parse(rawXML.GetElementsByTagName ("Height")[0].ChildNodes[0].Value);

		//Create gameMap instance with the required dimensions
		gameMap = new GameMap (mapWidth, mapHeight);

		//Loop through XML nodes in the TileList
		foreach (XmlNode tileInfo in TileList) {
			//reset xPos, yPos and TileType to default for the new Tile
			int xPos = 0;
			int yPos = 0;
			string tileType = "Empty";

			//Get the child nodes from the Tile element
			XmlNodeList tileContent = tileInfo.ChildNodes;
			//Loop through child nodes
			foreach (XmlNode contentNode in tileContent) {
				
				//Check if child node has the tag 'X'
				if (contentNode.Name == "X") {
					//Set the xPos with the data from the XML file
					xPos = Int32.Parse(contentNode.InnerXml);
				}

				//Check if child node has the tag 'Y'
				if (contentNode.Name == "Y") {
					//Set the yPos with the data from the XML file
					yPos = Int32.Parse(contentNode.InnerXml);
				}

				//Check if child node has the tag 'Type'
				if (contentNode.Name == "Type") {
					//Set the tile type with the data from the XML file
					tileType = contentNode.InnerXml;
				}
			}

			//Gets the tile class based on the X and Y position
			gameMap.setTile(xPos, yPos);

			GameTile tile_data = gameMap.getTileAt(xPos, yPos);

			//Sets the actual tile type
			tile_data.setTileType(tileType);

			OnTileTypeChange(tile_data);

			//Set position of tile
			//It's position is based of the X and Z axes because this is supposed to be a flat board in a 3D build!
			//This means the camera is creating the Isometric view by angling it 50 degrees on the X and 45 on the Y axes.
			tile_data.thisTile.transform.position = new Vector3(tile_data.X, 0, tile_data.Z);
			tile_data.thisTile.transform.SetParent(this.transform, true);
			//Give object name
			tile_data.thisTile.name = "Tile_" + tile_data.X + "_" + tile_data.Z;

			//Set overlay object
			//This object is only visible when there is something to overlay.
			//Think of attacks, moves etc.
			tile_data.tileOverlay = new GameObject();
			//Set name of overlay Objects
			tile_data.tileOverlay.name = "Overlay";
			//Set the parent of the object
			tile_data.tileOverlay.transform.SetParent(tile_data.thisTile.transform);
			//Flip overlay
			tile_data.tileOverlay.transform.Rotate(-90, 0, 0);
			//Set the scale of the object
			tile_data.tileOverlay.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
			//Set the position of the object
			tile_data.tileOverlay.transform.localPosition = new Vector3(0f, 1.4f, 0f);

			//Add sprite renderer to the tileoverlay object
			SpriteRenderer tile_sr = tile_data.tileOverlay.AddComponent<SpriteRenderer>();
			//Set sprite
			tile_sr.sprite = tileOverlay;
			//Set drawmode, because otherwise the tile would be too small (0.125)
			tile_sr.drawMode = SpriteDrawMode.Tiled;
			//Set the color to invisible so you don't see the overlay until the color is actually changed
			tile_sr.color = new Color(1f, 1f, 1f, 0f);

			//Register a callback to the 'OnTileTypeChange' function
			tile_data.registerTileTypeChangedCallback((tile) => {
				OnTileTypeChange(tile_data);
			});


		}

		//Generate graph for pathfinding
		//Technically it finds all the neighbours of every tile and saves those
		//Because dijkstra's algorithm requires for each tile to know it's neighbours
		gameMap.generatePathfindingGraph();

		//------//
		// TEST //
		//------//
		//Generate 1 character pikemen
		GameCharacter pikemen1 = new GameCharacter("Pikemen");
		GameCharacter unit = new GameCharacter("Pikemen");
		unit.setXPosition(6);
		unit.setZPosition(6);
		GameCharacter unit2 = new GameCharacter("Pikemen");
		unit2.setXPosition(7);
		unit2.setZPosition(5);
		GameCharacter villager1 = new GameCharacter("Villager");
		villager1.setXPosition(8);
		villager1.setZPosition(8);
		//Create new List
		unitList = new List<GameCharacter>();
		//Add pikemen to list
		unitList.Add (pikemen1);
		unitList.Add(unit);
		unitList.Add(unit2);
		unitList.Add(villager1);
		//----------//
		// END TEST //
		//----------//
	}

	// Update is called once per frame
	void Update () {
		
	}

	//Function that gets the unit based on location
	//So when user presses enter the Keyboardcontrols class will call this function to find any units on that location
	// - Param 1: X coordinate
	// - Param 2: Z coordinate
	public static GameCharacter getCharacterOnPosition(int x, int z){
		//Loop through the all the units in game
		foreach (GameCharacter character in Game._GameInstance.unitList) {
			//Check if this unit position is equal to the given coordinates
			if (
				character.getXPosition () == x &&
				character.getZPosition () == z
			) {
				//Return this unit if there is indeed an Unit on that tile
				return character.getInstance ();
			}
		}
		//Return null when no unit has been found.
		//This often means that the user wants to move an already selected unit.
		return null;
	}


	//Function that gets the building based on location
	//So when the menu is created, the Character class will call this to check if there are any pillagable buildings on this tile
	// - Param 1: X coordinate
	// - Param 2: Z coordinate
	//TODO: Actually make buildings possible
	public static bool getBuildingOnPosition(int x, int z) {
		return false;
	}

	//Function that gets called as soon as the type of a tile changes.
	// - Param 1: Tile_data is basically a reference to the tile class
	void OnTileTypeChange(GameTile tile_data){
		//Every if statement checks if the data type is which one and set the appropriote sprite
		if (tile_data.Type == GameTile.TileType.Mountains) {
			//Grab the prefab from the resources folder and store it in thisTile variable
			tile_data.thisTile = Instantiate(Resources.Load("dirt-low", typeof(GameObject))) as GameObject;
			//Set the scale of the object
			tile_data.thisTile.transform.localScale = new Vector3(0.125f, 0.12f, 0.125f);

			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 4;
		} else if (tile_data.Type == GameTile.TileType.Grass) {
			//Grab the prefab from the resources folder and store it in thisTile variable
			tile_data.thisTile = Instantiate(Resources.Load("grass", typeof(GameObject))) as GameObject;
			//Set the scale of the object
			tile_data.thisTile.transform.localScale = new Vector3(0.125f, 0.2f, 0.125f);

			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 2;
		} else if (tile_data.Type == GameTile.TileType.Desert) {
			//Grab the prefab from the resources folder and store it in thisTile variable
			tile_data.thisTile = Instantiate(Resources.Load("sand", typeof(GameObject))) as GameObject;
			//Set the scale of the object
			tile_data.thisTile.transform.localScale = new Vector3(0.125f, 0.2f, 0.125f);

			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 2;
		} else if (tile_data.Type == GameTile.TileType.River) {
			//Grab the prefab from the resources folder and store it in thisTile variable
			tile_data.thisTile = Instantiate(Resources.Load("water", typeof(GameObject))) as GameObject;
			//Set the scale of the object
			tile_data.thisTile.transform.localScale = new Vector3(0.125f, 0.2f, 0.125f);

			//Terrain modifier sets the amount of movement the character needs to move through the tile
			//0 means it takes no cost at all, but usually means the tile isn't traversable
			tile_data.TerrainModifier = 0;
			//Disables the tile, so units can't walk over them.
			tile_data.PassableTile = false;
		} else if (tile_data.Type == GameTile.TileType.Sea) {
			//Grab the prefab from the resources folder and store it in thisTile variable
			tile_data.thisTile = Instantiate(Resources.Load("water", typeof(GameObject))) as GameObject;
			//Set the scale of the object
			tile_data.thisTile.transform.localScale = new Vector3(0.125f, 0.2f, 0.125f);

			//Terrain modifier sets the amount of movement the character needs to move through the tile
			//0 means it takes no cost at all, but usually means the tile isn't traversable
			tile_data.TerrainModifier = 0;
			//Disables the tile, so units can't walk over them.
			tile_data.PassableTile = false;
		} else if (tile_data.Type == GameTile.TileType.Hills) {
			//Grab the prefab from the resources folder and store it in thisTile variable
			tile_data.thisTile = Instantiate(Resources.Load("grass", typeof(GameObject))) as GameObject;
			//Set the scale of the object
			tile_data.thisTile.transform.localScale = new Vector3(0.125f, 0.2f, 0.125f);

			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 3;
		} else if (tile_data.Type == GameTile.TileType.Forest) {
			//Grab the prefab from the resources folder and store it in thisTile variable
			tile_data.thisTile = Instantiate(Resources.Load("grass", typeof(GameObject))) as GameObject;
			//Set the scale of the object
			tile_data.thisTile.transform.localScale = new Vector3(0.125f, 0.2f, 0.125f);

			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 3;
		} else if (tile_data.Type == GameTile.TileType.Stone_Road) {
			//Grab the prefab from the resources folder and store it in thisTile variable
			tile_data.thisTile = Instantiate(Resources.Load("grass", typeof(GameObject))) as GameObject;
			//Set the scale of the object
			tile_data.thisTile.transform.localScale = new Vector3(0.125f, 0.2f, 0.125f);

			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 1;
		} else {
			//Error if none of the types match.
			//This means the tile is either Empty or Null!
			Debug.LogError("Unrecognized tileType");
		}
	}
}
