//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterInteraction {

	//Store X position of character
	int xPos;
	//Property of xPos variable
	public int x {
		get {
			return xPos;
		}
		set {
			xPos = value;
		}
	}

	//Store Y position of character
	int zPos;
	//Property of zPos variable
	public int z {
		get {
			return zPos;
		}
		set {
			zPos = value;
		}
	}

	//Function that checks if Path to target is within the amount of movement points the unit has
	public bool isReachable(int mp, List<GameTile> path){
		//Create variable that will store the amount of points needed to get to target tile and set it to 0
		int totalPointsNeeded = 0;
		//Loop through all tiles the unit will traverse
		foreach(GameTile tile in path) {
			//Add the points needed to walk through the tile
			totalPointsNeeded += tile.TerrainModifier;
		}
		//Check if the unit has enough movement points needed to get to target tile
		if(mp < totalPointsNeeded) {
			//Can't make this move (out of range)
			return false;
		} else {
			//Can make move
			return true;
		}
	}
}

