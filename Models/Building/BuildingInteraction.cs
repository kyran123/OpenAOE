//=======================================================================//
// Copyright: Kyran Studios												 //
// Written by: Kyle Fransen												 //
// Https://resume.kylefransen.nl										 //
//=======================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class BuildingInteraction {

    //Store X position of character
    int xPos;
    //Property of xPos variable
    public int x {
        get {
            return xPos;
        }
        set {
            if(!isLocked) xPos = value;
        }
    }

    //Store Z position of character
    int zPos;
    //Property of zPos variable
    public int z {
        get {
            return zPos;
        }
        set {
            if(!isLocked) zPos = value;
        }
    }

    public BuildingInteraction() {

    }

    //Store whether unit is locked or not
    private bool isLocked = false;
    //Property of isLocked
    public bool IsLocked {
        get {
            return isLocked;
        }
        set {
            isLocked = value;
        }
    }
}
