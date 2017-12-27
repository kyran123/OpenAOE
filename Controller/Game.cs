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

//
// The main controller
//
public class Game : MonoBehaviour {

	//Save sprites here
	public Sprite MountainsSprite;
	public Sprite GrassSprite;
	public Sprite RiverSprite;
	public Sprite SeaSprite;
	public Sprite RoadSprite;
	public Sprite DesertSprite;
	public Sprite HillsSprite;
	//Store instance of the map
	private GameMap gameMap;

	// Use this for initialization
	void Start () {
		//Test random tiles
		//gameMap.RandomizeTiles();

		//Code to load map from XML
		//TODO: The file changes depending on chosen map!
		//creates new XmlDocument instance
		XmlDocument rawXML = new XmlDocument();

		//Loads file into the instance of XmlDocument
		rawXML.Load (Application.dataPath + "/Data/flatlandmap.xml");

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

			//Create new emtpy game object
			GameObject tile_go = new GameObject ();
			//Give object name
			tile_go.name = "Tile_" + xPos + "_" + yPos;

			//Set position of tile
			//It's position is based of the X and Z axes because this is supposed to be a flat board in a 3D build!
			//This means the camera is creating the Isometric view by angling it 50 degrees on the X and 45 on the Y axes.
			tile_go.transform.position = new Vector3 (xPos, 0, yPos);
			tile_go.transform.SetParent (this.transform, true);

			//Set Angle of the tile
			//The tile looks like it is standing up in the standard angle and we want it to lay down flat.
			//We rotate by the X coordinate by 90 degrees, although this can be done on the Y axes as well.
			Vector3 rotationVector = transform.rotation.eulerAngles;
			rotationVector.x = 90;
			tile_go.transform.rotation = Quaternion.Euler(rotationVector);

			//Add a new sprite renderer module on the game object and store it in a variable
			//The sprite renderer is used to show all tile spirtes in a seperate function
			SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer> ();

			//Call the function in the GameMap class, that creates the actual tiles in code.
			//NOTE: Tile type is not set here yet! That happens in the setTileType function!!!
			gameMap.setTile (xPos, yPos);

			//Gets the tile class based on the X and Y position
			GameTile tile_data = gameMap.getTileAt (xPos, yPos);

			//Register a callback to the 'OnTileTypeChange' function
			tile_data.registerTileTypeChangedCallback((tile) => {
				OnTileTypeChange(tile, tile_sr);
			});

			//Sets the actual tile type
			tile_data.setTileType (tileType);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	//Function that gets called as soon as the type of a tile changes.
	// - Param 1: Tile_data is basically a reference to the tile class
	// - Param 2: The sprite renderer so we can add the sprite to it
	void OnTileTypeChange(GameTile tile_data, SpriteRenderer tile_sr){
		//Every if statement checks if the data type is which one and set the appropriote sprite
		if (tile_data.Type == GameTile.TileType.Mountains) {
			//Sets mountain sprite
			tile_sr.sprite = MountainsSprite;
			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 4;
		} else if (tile_data.Type == GameTile.TileType.Grass) {
			//Set grass sprite
			tile_sr.sprite = GrassSprite;
			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 2;
		} else if (tile_data.Type == GameTile.TileType.Desert) {
			//Set desert sprite
			tile_sr.sprite = DesertSprite;
			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 2;
		} else if (tile_data.Type == GameTile.TileType.River) {
			//Set river sprite
			tile_sr.sprite = RiverSprite;
			//Terrain modifier sets the amount of movement the character needs to move through the tile
			//0 means it takes no cost at all, but usually means the tile isn't traversable
			tile_data.TerrainModifier = 0;
			//Disables the tile, so units can't walk over them.
			tile_data.PassableTile = false;
		} else if (tile_data.Type == GameTile.TileType.Sea) {
			//Set sea sprite
			tile_sr.sprite = SeaSprite;
			//Terrain modifier sets the amount of movement the character needs to move through the tile
			//0 means it takes no cost at all, but usually means the tile isn't traversable
			tile_data.TerrainModifier = 0;
			//Disables the tile, so units can't walk over them.
			tile_data.PassableTile = false;
		} else if (tile_data.Type == GameTile.TileType.Hills) {
			//Set hill sprite
			tile_sr.sprite = HillsSprite;
			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 3;
		}else if (tile_data.Type == GameTile.TileType.Stone_Road) {
			//Set road sprite
			tile_sr.sprite = RoadSprite;
			//Terrain modifier sets the amount of movement the character needs to move through the tile
			tile_data.TerrainModifier = 1;
		} else {
			//Error if none of the types match.
			//This means the tile is either Empty or Null!
			Debug.LogError("Unrecognized tileType");
		}

		//Set the draw mode to tiled, because it'd otherwise be way too small (0.125)
		tile_sr.drawMode = SpriteDrawMode.Tiled;
		//Set the size of the draw mode to 1.
		tile_sr.size = new Vector2 (1, 1);
	}
}
