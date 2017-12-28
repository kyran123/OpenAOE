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
	private int y;
	public int Y {
		get {
			return y;
		}
	}

	//Initializes a new instance of the class
	//Gamemap, is the GameMap instance
	//X = X coordinate
	//Y = Y coordinate
	public GameTile(GameMap gameMap, int x, int y) {
		this.gameMap = gameMap;
		this.x = x;
		this.y = y;
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
