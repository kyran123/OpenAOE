//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// The map class of the game
//
public class GameMap {

	//Saves the instance of GameMap
	//We do this so other non related C# scripts can interact with the map
	public static GameMap _GMinstance { get; protected set; }

	//Array with 2 keys to store references to the tile classes
	// - 1st key is the X coordinate
	// - 2nd key is the z coordinate
	private GameTile[,] tiles;

	//The variable that keeps track of the width of the map
	//You are not allowed to change it as soon as the map has been built.
	//When game ends, all tile references will be destroyed/removed.
	private int width;
	public int Width {
		get {
			return width;
		}
	}

	//The variable that keeps track of the height of the map
	//You are not allowed to change it as soon as the map has been built.
	//When game ends, all tile references will be destroyed/removed.
	private int height;
	public int Height {
		get {
			return height;
		}
	}

	//Initializes a new instance of the class
	//Width = the amount of tiles that make up the width of the map
	//Height = the amount of tiles that make up the height of the map
	public GameMap(int width = 50, int height = 50){
		this.width = width;
		this.height = height;
		_GMinstance = this;

		//Initialize the game tile array with the length
		tiles = new GameTile[width, height];
	}

    //Checks if tile on Coordinates exists
    public bool isTileValid(int x, int z) {
        //Checks if the X and Y coordinate exceeds map limits
        if(x > width || x < 0 || z > height || z < 0) {
            //Log that the tile is out of range
            Debug.LogError("Tile (" + x + ", " + z + ") is out of range.");
            //Return false, that this is not a valid tile to move on
            return false;
        }
        //Check if tile type is empty
        if(tiles[x, z].Type == GameTile.TileType.Empty) {
            //Return false, that this is not a valid tile to move on
            return false;
        }
        //Return true, when the tile exists and is not empty
        return true;
    }

	//Creates a new tile instance
	public void setTile(int x, int z){
		//Stores the new tile Instance in the tiles array with the X and z coordinates as key
		tiles [x, z] = new GameTile (this, x, z);
	}

	//returns the tile instance found on X and Z coordinates
	public GameTile getTileAt(int x, int z){
		//Checks if the X and Y coordinate exceeds map limits
		if (x > width || x < 0 || z > height || z < 0) {
			//Log that the tile is out of range
			Debug.LogError ("Tile (" + x + ", " + z + ") is out of range.");
			//Return null instead of a tile
			return null;
		}
		//Return the tile based on the X and Z coordinate
		return tiles [x, z];
	}

	//Return the tile array itself
	public GameTile[,] getTileArray(){
		return tiles;
	}

	//Function that generates a graph for the path finding
	public void generatePathfindingGraph(){
		//Loop through all the tiles on the X and Z coords
		for(int x = 0; x < width; x++) {
			for(int z = 0; z < height; z++) {
				//Add neighbours to the west (and check if thez are within the bounds of the map
				if(x > 0) {
					addNeighbour((x - 1), z, x, z);
				}
				//Add neighbours to the east (and check if they are within the bounds of the map
				if(x < (this.Width - 1)) {
					addNeighbour((x + 1), z, x, z);
				}
				//Add neighbours to the north (and check if they are within the bounds of the map
				if(z > 0) {
					addNeighbour(x, (z - 1), x, z);
				}
				//Add neighbours to the south (and check if they are within the bounds of the map
				if(z < (this.Width - 1)) {
					addNeighbour(x, (z + 1), x, z);
				}
			}
		}
	}

	//Removes the entire path finding graph
	//WARNING: always use this when you change tile game object!!!
	public void emptyPathFindingGraph(){
		//Loop through all the tiles on the X and Z coords
		for(int x = 0; x < width; x++) {
			for(int z = 0; z < height; z++) {
				//Call for each tile, the empty neighbourtiles function, which creates a new, empty list of neighbourtiles
				getTileAt(x, z).emptyNeighbourTiles();
			}
		}
		//Generate new path finding graph
		generatePathfindingGraph();
	}

	//Add the neighbour of a tile when it is passable
	//This is necessary to create the graph for path finding
	public void addNeighbour(int neighbourX, int neighbourZ, int x, int z){
		//Checks if the Tile is passable or not
		if(tiles[neighbourX, neighbourZ].PassableTile) {
			//Add the tile to the List if it is passable
			tiles[x, z].getNeighbourTiles.Add(tiles[neighbourX, neighbourZ]);
		}
	}

	//Test function that randomizes tile types.
	public void RandomizeTiles(){
		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {
				int rand = Random.Range (0, 8);
				if (rand == 0) {
					tiles [x, z].Type = GameTile.TileType.Grass;
				} else if (rand == 1) {
					tiles [x, z].Type = GameTile.TileType.Mountains;
				} else if (rand == 2) {
					tiles [x, z].Type = GameTile.TileType.Desert;
				} else if (rand == 3) {
					tiles [x, z].Type = GameTile.TileType.Hills;
				} else if (rand == 4) {
					tiles [x, z].Type = GameTile.TileType.River;
				} else if (rand == 5) {
					tiles [x, z].Type = GameTile.TileType.Sea;
				} else if (rand == 6) {
					tiles [x, z].Type = GameTile.TileType.Stone_Road;
				} else {
					tiles [x, z].Type = GameTile.TileType.Forest;
				}
			}
		}
	}
}
