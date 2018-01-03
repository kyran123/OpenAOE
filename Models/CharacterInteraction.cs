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
}

