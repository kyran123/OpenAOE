//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//
// The tile class of the ingame map
//
public class GameTile {

	//Object of GameTile
	public GameObject thisTile { get; set; }
	public GameObject tileOverlay { get; set; }

	//The type of the game tile. 
	//This is only for the type of ground, not the buildings or ores above it.
	public enum TileType { 
		Empty,
		Grass, 
		Forest,
		Mountains, 
		River, 
		Desert,
		Stone_Road,
		Sea,
		Hills
	};

	//Action stores one or mutltiple call(s) to one or more function(s)
	//We use this to call a function in the Game class to show that tile type has changed!
	private Action<GameTile> cbTileTypeChanged;

	//The actual Enum variable that's accessable and changable from outside this class.
	private TileType type;
	public TileType Type {
		get {
			return type;
		}
		set {
			//Store the old type before changing to the new one
			TileType oldType = type;
			//Change type
			type = value;
			//Check if the action has any callbacks and if the old type isn't the same as the new one.
			//So we don't have to call the function more than we need to.
			if (cbTileTypeChanged != null && oldType != type) {
				//Call all the functions stored in this Action
				cbTileTypeChanged (this);
			}
		}
	}

	//The variable that keeps track if a tile is passable.
	//Rivers and Seas are currently not passable.
	private bool passableTile = true;
	public bool PassableTile {
		get {
			return passableTile;
		}
		set {
			passableTile = value;
		}
	}

	//The variable that keeps track of how many movement points it cost to traverse it.
	//Mountains cost the most movement, while grassland the least.
	private int terrainModifier = 0;
	public int TerrainModifier {
		get {
			return terrainModifier;
		}
		set {
			terrainModifier = value;
		}
	}

	//Variable that keeps track of the actual map of tiles.
	private GameMap gameMap;

	//Variable that keeps track of the X coord.
	//You are not allowed to change them as soon as the map has been built.
	//When game ends, all tile references will be destroyed/removed.
	private int x;
	public int X {
		get {
			return x;
		}
	}

	//Variable that keeps track of the Y coord.
	//You are not allowed to change them as soon as the map has been built.
	//When game ends, all tile references will be destroyed/removed.
	private int z;
	public int Z {
		get {
			return z;
		}
	}

	//Neighbour tiles of this tile
	private List<GameTile> neighbourTiles;
	public List<GameTile> getNeighbourTiles {
		get { 
			return neighbourTiles;
		}
		set { 
			this.neighbourTiles = value; 
		}
	}

	//Initializes a new instance of the class
	//Gamemap, is the GameMap instance
	//X = X coordinate
	//Y = Y coordinate
	//NeighbourTiles = List of Tiles that border this tile
	public GameTile(GameMap gameMap, int x, int z) {
		this.gameMap = gameMap;
		this.x = x;
		this.z = z;
		this.neighbourTiles = new List<GameTile>();
	}

	public void emptyNeighbourTiles(){
		this.neighbourTiles = new List<GameTile>();
	}

	//Calculate and return the distance between 2 tiles
	public float distanceTo(GameTile neighbour){
		//Check if the given parameter is not null
		if(neighbour != null) {
			//Calculate and return result
			return Vector2.Distance(
				new Vector2(this.X, this.Z),
				new Vector2(neighbour.X, neighbour.Z)
			);
		} else {
			//Error if parameter is null
			Debug.LogError("No neighbour found?");
			//Return 10 (movement cost) so that the code can continue without any errors
			return 10;
		}
	}

	//Register a function to be called back when our tile type changes.
	public void registerTileTypeChangedCallback(Action<GameTile> callback){
		cbTileTypeChanged += callback;
	}

	//Unregister a callback
	public void unRegisterTileTypeChangedCallback(Action<GameTile> callback){
		cbTileTypeChanged -= callback;
	}

	//Sets the type of tile seperately, to make sure it gets updated with the callback
	public void setTileType(string type){
		if(this != null){
			switch (type) {
				case "Grass":
					this.Type = GameTile.TileType.Grass;
					break;
				case "Mountain":
					this.Type = GameTile.TileType.Mountains;
					break;
				case "Desert":
					this.Type = GameTile.TileType.Desert;
					break;
				case "Forest":
					this.Type = GameTile.TileType.Forest;
					break;
				case "Hill":
					this.Type = GameTile.TileType.Hills;
					break;
				case "River":
					this.Type = GameTile.TileType.River;
					break;
				case "Sea":
					this.Type = GameTile.TileType.Sea;
					break;
				case "Road":
					this.Type = GameTile.TileType.Stone_Road;
					break;			
				case "Empty":
				default:
					this.Type = GameTile.TileType.Empty;
					break;
			}
		}
	}

    //Get the string of the tile type to display to the user
    public string getTileTypeName() {
        switch (this.type) {
            case TileType.Grass:
                return "Grass";
            case TileType.Mountains:
                return "Mountains";
            case TileType.Desert:
                return "Desert";
            case TileType.Forest:
                return "Forest";
            case TileType.Hills:
                return "Hills";
            case TileType.River:
                return "River";
            case TileType.Sea:
                return "Sea";
            case TileType.Stone_Road:
                return "Road";
            default:
            case TileType.Empty:
                return "";
        }
    }

	public void changeTile(string type) {

		GameTile oldTile = gameMap.getTileAt(this.X, this.Z);
		oldTile.thisTile.name = "Old_Tile_Pls_Remove";

		//Gets the tile class based on the X and Y position
		gameMap.setTile(this.X, this.Z);
		GameTile tile_data = gameMap.getTileAt(this.X, this.Z);


		//Sets the actual tile type
		tile_data.setTileType(type);

		Game._GameInstance.OnTileTypeChange(tile_data);

		//Set position of tile
		//It's position is based of the X and Z axes because this is supposed to be a flat board in a 3D build!
		//This means the camera is creating the Isometric view by angling it 50 degrees on the X and 45 on the Y axes.
		tile_data.thisTile.transform.position = new Vector3(tile_data.X, 0, tile_data.Z);
		tile_data.thisTile.transform.SetParent(Game._GameInstance.transform, true);
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
		tile_sr.sprite = Game._GameInstance.tileOverlay;
		//Set drawmode, because otherwise the tile would be too small (0.125)
		tile_sr.drawMode = SpriteDrawMode.Tiled;
		//Set the color to invisible so you don't see the overlay until the color is actually changed
		tile_sr.color = new Color(1f, 1f, 1f, 0f);

		//Register a callback to the 'OnTileTypeChange' function
		tile_data.registerTileTypeChangedCallback((tile) => {
			Game._GameInstance.OnTileTypeChange(tile_data);
		});


		UnityEngine.Object.Destroy(oldTile.thisTile);


		//Regenerate path finding graph
		gameMap.emptyPathFindingGraph();
	}

}
