//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Player {

	//Player Class
	private int userid;
	private string username;

    //Gamestate to check if it is this players turn
	private bool isTurn;

    //List of all owned units by the player
	private List<GameCharacter> units = new List<GameCharacter>();
    //public function to add an unit to the list of owned units
    public void addUnit(GameCharacter unit) { units.Add(unit); }
    //Public function to remove an unit to the list of owned units
    public void removeUnit(GameCharacter unit) { units.Remove(unit); }
    //Public function to get the entire list
    public List<GameCharacter> getUnitList() { return units; }
    //Public function to check if unit is owned by player
    public bool checkIfUnit(GameCharacter unit) {
        //Loop through each unit
        foreach(GameCharacter checkUnit in this.units) {
            //Check if the unit is owned and return the boolean
            if(unit == checkUnit) return true;
        }
        //Return false if nothing comes up
        return false;
    }

    //List of all owned buildings by the player
    private List<GameBuilding> buildings = new List<GameBuilding>();
    //Public function to add building to  the list of owned buildings
    public void addbuilding(GameBuilding building) { buildings.Add(building); }
    //Public function to remove an building to the list of owned buildings
    public void removeBuilding(GameBuilding building) { buildings.Remove(building); }
    //Public function to get entire list of owned buildings
    public List<GameBuilding> getBuildingList() { return this.buildings; }
    //Public function to check if unit is owned by player
    public bool checkIfUnit(GameBuilding building) {
        //loop through each building
        foreach(GameBuilding checkbuilding in this.buildings) {
            //Check if the building is owned and return the bool
            if(building == checkbuilding) return true;
        }
        //Return false if nothing comes up
        return false;
    }


	public Player(){
		
	}

	public virtual void executeTurn(){

	}

}
