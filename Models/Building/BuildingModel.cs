//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class BuildingModel {

    //Variable where the game object of this unit is stored
    protected GameObject bldModel;
    //Get the game object of this unit
    public GameObject getGameObject() {
        return this.bldModel;
    }

    //Constructor
    public BuildingModel() {

    }

    //Creates empty game object and returns it
    public void createGameObject(string charName) {
        this.bldModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this.bldModel.name = charName;
    }

    //set Model for the object
    public void setModel() {
        //TODO: add the actual 3D model to the MeshRenderer
        this.bldModel.AddComponent<MeshRenderer>();
    }

    //Function to set the position of the object (ignoring any tiles / movement cost)
    public void setGameObjectPosition(int x, int z) {
        float xPosition = (float)x;
        float zPosition = (float)z;
        this.bldModel.transform.DOMove(new Vector3(xPosition, 0.2f, zPosition), 0.2f);
    }
}
