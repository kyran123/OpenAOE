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
	// - 2nd key is the Y coordinate
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

	//Creates a new tile instance
	public void setTile(int x, int y){
		//Stores the new tile Instance in the tiles array with the X and Y coordinates as key
		tiles [x, y] = new GameTile (this, x, y);
	}

	//returns the tile instance found on X and Y coordinates
	public GameTile getTileAt(int x, int y){
		//Checks if the X and Y coordinate exceeds map limits
		if (x > width || x < 0 || y > height || y < 0) {
			//Log that the tile is out of range
			Debug.LogError ("Tile (" + x + ", " + y + ") is out of range.");
			//Return null instead of a tile
			return null;
		}
		//Return the tile based on the X and Y coordinate
		return tiles [x, y];
	}

	//Test function that randomizes tile types.
	public void RandomizeTiles(){
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int rand = Random.Range (0, 7);
				if (rand == 0) {
					tiles [x, y].Type = GameTile.TileType.Grass;
				} else if (rand == 1) {
					tiles [x, y].Type = GameTile.TileType.Mountains;
				} else if (rand == 2) {
					tiles [x, y].Type = GameTile.TileType.Desert;
				} else if (rand == 3) {
					tiles [x, y].Type = GameTile.TileType.Hills;
				} else if (rand == 4) {
					tiles [x, y].Type = GameTile.TileType.River;
				} else if (rand == 5) {
					tiles [x, y].Type = GameTile.TileType.Sea;
				} else if (rand == 6) {
					tiles [x, y].Type = GameTile.TileType.Stone_Road;
				}
			}
		}
	}
}
