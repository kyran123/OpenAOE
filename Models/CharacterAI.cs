using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class CharacterAI {

	//Variable where the game map will be stored
	protected GameMap gameMap;

	Dictionary<GameTile, float> distanceBetweenTiles;
	Dictionary<GameTile, GameTile> previousTile;
	List<GameTile> unvisited;

	public CharacterAI() {
		//Get the game map class instance
		this.gameMap = GameMap._GMinstance;

		distanceBetweenTiles = new Dictionary<GameTile, float>();
		previousTile = new Dictionary<GameTile, GameTile>();
		unvisited = new List<GameTile>();
	}

	//Finds a path of tiles needed to get to the destination.
	public List<GameTile> findPath(GameTile source, GameTile target){
		//TODO: Add movement points
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
			//If the node isn't the srouce
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

		while(unvisited.Count > 0) {
			//Unvisited node with the smallest distance
			GameTile tileNode = null;

			//Loop through all possible nodes
			foreach(GameTile possibleNode in unvisited) {
				//Check for the closest node
				if(tileNode == null || distanceBetweenTiles[possibleNode] < distanceBetweenTiles[tileNode]) {
					//Set the closest node
					tileNode = possibleNode;
				}
			}

			//If we find target exit the while loop
			if(tileNode == target) {
				break;
			}

			//Remove this node from the qeue
			unvisited.Remove(tileNode);

			//Loop through all neighbour tiles of node
			foreach(GameTile node in tileNode.getNeighbourTiles) {
				//Calculate distance
				float alt = distanceBetweenTiles[tileNode] + (tileNode.distanceTo(node) + node.TerrainModifier);
				//Check if the new calculate distance is smaller than the previous node
				if(alt < distanceBetweenTiles[node]) {
					distanceBetweenTiles[node] = alt;
					previousTile[node] = tileNode;
				}
			}
		}

		//We either found the shortest route to our target
		//Or we didn't find a route at all to our target

		if(previousTile[target] == null) {
			//No route for our target!
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
}

