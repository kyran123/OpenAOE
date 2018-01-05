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

	//Store Z position of character
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


	//Store previous X position of character of this turn
	int oldXPos;
	//Property of oldXPos variable
	public int previousX {
		get {
			return oldXPos;
		}
		set {
			oldXPos = value;
		}
	}


	//Store previous Z position of character of this turn
	int oldZPos;
	//Property of oldZPos variable
	public int previousZ {
		get {
			return oldZPos;
		}
		set {
			oldZPos = value;
		}
	}

	//Function that moves the last saved unit position to the old position
	public void backupPosition(){
		this.previousX = this.x;
		this.previousZ = this.z;
	}

	//Function that backs up the X coordinate
	public void backupX(){
		this.previousX = this.x;
	}

	//Function that backs up the Z coordinate
	public void backupZ(){
		this.previousZ = this.z;
	}

	//Function the reset the previous position to the current position
	public void resetPosition(){
		this.x = this.previousX;
		this.z = this.previousZ;
	}
}

