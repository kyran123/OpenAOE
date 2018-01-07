//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class CharacterAI {

	//Variable where the game map will be stored
	protected GameMap gameMap;

	//Lists and Dictionaries of Tiles for the pathfinding
	//DistanceBetweenFiles stores the source Tile and how far it's bordering tiles are away from it (including movement cost)
	Dictionary<GameTile, float> distanceBetweenTiles;
	//previousTile is a List that potental Tiles will be added.
	//It creates a chain of Tiles that will ALWAYS lead back to the original source tile
	Dictionary<GameTile, GameTile> previousTile;
	//unvisited is a qeue for the path finding
	//Tiles in this list have yet to be checked for a route.
	List<GameTile> unvisited;


	//PossibleTiles for graph
	Dictionary<GameTile, int> possibleTiles;
	List<GameTile> allReachableTiles;

	//Constructor
	public CharacterAI() {
		//Get the game map class instance
		this.gameMap = GameMap._GMinstance;

		//Create the dictionaries and lists
		distanceBetweenTiles = new Dictionary<GameTile, float>();
		previousTile = new Dictionary<GameTile, GameTile>();
		unvisited = new List<GameTile>();
	}

	//Finds a path of tiles needed to get to the destination.
	// - Param 1: Source tile (The tile that the unit starts of with)
	// - Param 2: Target tile (The destination tile that unit wants to go to)
	public List<GameTile> findPath(GameTile source, GameTile target){
		//Dictionary of tiles, that stores the distance from eachother
		distanceBetweenTiles.Clear();
		//Dictionary of tiles that make up the path
		previousTile.Clear();
		//Setup the qeue --- the list of gametiles we haven't checked yet
		unvisited.Clear();

		//Set source distance to 0
		distanceBetweenTiles[source] = 0;
		//Set source previous tile to empty
		previousTile[source] = null;

		//Initialize everything to have INFINITY distance, since we don't know any better right now
		foreach(GameTile node in this.gameMap.getTileArray()) {
				
			//If the node isn't the source
			if(node != source) {
				//Set the distance to infinity (Highest number possible)
				//This is reasonable, because some tiles might not be accessable by some units
				distanceBetweenTiles[node] = Mathf.Infinity;
				//Set previous to empty, because we don't know which is the previous yet
				previousTile[node] = null;
			}

			//Add all GameTile nodes to the qeue.
			//We loop through the qeue, so go through all tiles, to figure out the shortest path
			unvisited.Add(node);

		}

		//Loop through the unvisited qeue as long as it has 1 or more items in it
		while(unvisited.Count > 0) {
			//Variable to store GameTile
			GameTile tileNode = null;

			//Loop through all possible Tiles in unvisited
			foreach(GameTile possibleNode in unvisited) {
				//Check for the closest Tile
				if(tileNode == null || distanceBetweenTiles[possibleNode] < distanceBetweenTiles[tileNode]) {
					//If it is the closest tile, set it as the current tile
					tileNode = possibleNode;
				}
			}

			//If the current tile, is also the target tile
			if(tileNode == target) {
				//Stop the loop
				break;
			}

			//If it isn't the target tile then remove this node from the qeue
			unvisited.Remove(tileNode);

			//Loop through all neighbour tiles of node
			foreach(GameTile node in tileNode.getNeighbourTiles) {
				//Calculate distance (including terrain modifier)
				float alt = distanceBetweenTiles[tileNode] + (tileNode.distanceTo(node) + node.TerrainModifier);
				//Check if the new calculated distance is smaller than the previous node
				if(alt < distanceBetweenTiles[node]) {
					//Override the old tile with the new one, which has smaller distance
					distanceBetweenTiles[node] = alt;
					previousTile[node] = tileNode;
				}
			}
		}

		//We either found the shortest route to our target
		//Or we didn't find a route at all to our target

		//Check if didn't find a route at all
		if(previousTile[target] == null) {
			//No route for our target and return an empty list!
			return new List<GameTile>();
		}

		//Create List with tiles for path
		List<GameTile> currentPath = new List<GameTile>();
		//Store the target position to the current tile
		GameTile current = target;

		//Loop as long as the current tile is not null
		while(current != null) {
			//Add the current tile to path
			currentPath.Add(current);
			//Get the next tile to check
			current = previousTile[current];
		}

		//Reverse the order of the list
		currentPath.Reverse();

		//Return the path in an array of tiles
		return currentPath;

	}

	//Function that checks if Path to target is within the amount of movement points the unit has
	public bool isReachable(GameTile destination){
		//Check if the destination tile is in the reachable tiles list
		if(allReachableTiles.Contains(destination)) {
			//Unit can make the move
			return true;
		}
		//Can't make this move, out of range
		return false;
	}

	//Function to show what possible moves you have
	// - Param 1: Source is the tile where the unit starts from
	// - Param 2: The amount of points the unit has to move
	public void generatePossibleMovesGraph(GameTile source, int movementPoints){
		//Create empty list of possible tiles
		//These tiles, are neighbours from previous tiles, that actually were reachable
		possibleTiles = new Dictionary<GameTile, int>();

		//Create new list of all reachable tiles
		//This list only contains tiles, that are actually reachable
		//We return this list later
		allReachableTiles = new List<GameTile>();

		bool allPathsChecked = false;

		//Check possible neighbours of the source tile and add them.
		//Because the source tile does not have a value right now, we don't pass on the totalPoints
		checkPossibleTiles(source, movementPoints);

		while(allPathsChecked == false) {
			//Loop through possible tiles to add to the reachable tiles list
			foreach(GameTile tile in possibleTiles.Keys.ToList()) {
				//Save already reachable tiles
				allReachableTiles.Add(tile);
				tile.tileOverlay.GetComponent<SpriteRenderer>().color = new Color(0.78f, 0.88f, 0f, 0.4f);
			}

			//Loop through new tiles to see if they are reachable for the unit
			foreach(GameTile tile in possibleTiles.Keys.ToList()) {
				//Check possible tiles in the neighbour of the given tile in the dictionary
				checkPossibleTiles(tile, movementPoints, possibleTiles[tile]);
				//Remove the tile from the possible tiles list
				//We only do this now, because we needed the reference to find it's neighbours.
				possibleTiles.Remove(tile);
			}

			//Checks if there is still any possible tiles left to check
			if(possibleTiles.Count < 1) {
				//If there isn't any tiles left to check, stop the while loop
				allPathsChecked = true;
			}
		}
	}

	//Funtion that checks whether the neighbours of a tile are reachable and passable
	// - Param 1: The tile we want to check the neighbours of
	// - Param 2: The max amount of movement points the unit has
	// - Param 3: The total points already acrued by the previous tile
	public void checkPossibleTiles(GameTile gameTile, int maxPoints, int totalPoints = 0){
		//Loop through all neighbour tiles
		foreach(GameTile tile in gameTile.getNeighbourTiles) {
			//Check if the tile is passable by any unit
			if(tile.PassableTile == true) {
				//Calculate the amount of points needed to get to this tile
				int tileTotalPoints = totalPoints + tile.TerrainModifier;
				//Check if the total points isn't over the maximum movement points of the unit is
				if(tileTotalPoints < maxPoints) {
					//Check if the tile hasn't already been added to the the possible tiles list
					if(possibleTiles.ContainsKey(tile) == false) {
						//Add the tile, with the amount of points needed to reach it, to the possible tiles dictionary
						possibleTiles.Add(tile, tileTotalPoints);
					}
				}
			}
		}
	}

	//Function that removes the overlay on tiles that were reachable by unit
	//We only remove this selection when user changes selected unit or moves the selected unit
	public void removePossibleMovesGraph(){
		//Check if the list with tiles isn't empty
		if(allReachableTiles.Count > 0) {
			//Loop through the list of overlayed tiles
			foreach(GameTile tile in allReachableTiles) {
				//Set the color back to transparent
				tile.tileOverlay.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
			}
			//Clear the lists to save some memory
			allReachableTiles.Clear();
			possibleTiles.Clear();
		}
	}
}

