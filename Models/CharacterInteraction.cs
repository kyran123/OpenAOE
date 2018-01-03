using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterInteraction {

	//Store X position of character
	int xPos;
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
	public int z {
		get {
			return zPos;
		}
		set {
			zPos = value;
		}
	}

	//TODO: Remove contructor
	public CharacterInteraction(){
		xPos = 5;
		zPos = 5;
	}
}

