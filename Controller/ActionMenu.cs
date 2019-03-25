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

//
// The action menu class that handles all input / output of the menu for the unit
//
public class ActionMenu : MonoBehaviour {

	//The instance of this class
	public static ActionMenu _instance;
	//List of button objects, so we may interact with it from different classes
	public List<GameObject> buttons;

	//Gameobject that holds the prefab of the button
	public GameObject actionsMenuPrefab;

	//Basically the constructor of a monobehaviour
	void Start(){
		//Set this class to the static instance
		ActionMenu._instance = this;
		//Create new list instance
		this.buttons = new List<GameObject>();
	}

	//Function that updates the action menu with all the valid abilities on that tile
	// - Param 1: The unit that is selected to perform any of the abilities
	// - Param 2: The list of abilities from that unit
	public void updateMenu(GameCharacter unit, List<string> abilities){
        
		//Clear buttons list
		this.buttons.Clear();
		this.removeButtonsFromMenu();

		//Set size of button container
		this.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 0);

		//Loop through the list of abilities
		foreach(string ability in abilities) {
			//Create a new Game Object that instanciates with the prefab
			GameObject menuButtonObject = (GameObject)Instantiate(actionsMenuPrefab);
			//Set this monobehaviour as the parent
			menuButtonObject.transform.SetParent(this.transform);
			//Set the name of the game object
			menuButtonObject.name = "Ability" + ability;
			//Get the text component and set the text to the ability name
			menuButtonObject.transform.GetComponentInChildren<Text>().text = ability;

			//Get the RectTransform component from the Game object
			RectTransform rt = menuButtonObject.GetComponent<RectTransform>();
			//Set the scale to 1
			rt.localScale = new Vector3(1f, 1f, 1f);
			//Set the sizeDelta to the preferred width & height
			rt.sizeDelta = new Vector2(120, 20);

			//Get the button component from the game object
			Button menuButton = menuButtonObject.GetComponent<Button>();
			//Check what the ability is and call the SetListener function
			switch(ability){
				case "Attack": 
					SetListener(ability, menuButton, KeyBoardControls._instance.setSelectMode);
					break;
				case "Done":
					SetListener(ability, menuButton, unit.unitDone);
					break;
				case "Undo":
					SetListener(ability, menuButton, unit.unitUndo);
					break;
				case "Pillage":
					SetListener(ability, menuButton, unit.unitPillage);
					break;
				default:
					//When none of the standard abilities are clicked
					SetListenerForAbility(ability, menuButton, unit.invokeAbility);
					break;
				
			}

			//Add this newly created game object into the buttons list
			this.buttons.Add(menuButtonObject);
		}
		//Select the first button to get it highlighted, so we can use the keyboard for navigating the menu as well!
		this.buttons[0].GetComponent<Button>().Select();
	}

    //Show all buildings as options to place
    public void showBuildMenu(List<GameBuilding> Buildables, GameCharacter gc) {

        //Clear buttons list
        this.buttons.Clear();
        this.removeButtonsFromMenu();

        //Set size of button container
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 0);

        //Loop through all buildables
        foreach(GameBuilding Building in Buildables) {

            //Create a new Game Object that instanciates with the prefab
            GameObject menuButtonObject = (GameObject)Instantiate(actionsMenuPrefab);
            //Set this monobehaviour as the parent
            menuButtonObject.transform.SetParent(this.transform);
            //Set the name of the game object
            menuButtonObject.name = "Buildable: " + Building.buildingName;
            //Get the text component and set the text to the ability name
            menuButtonObject.transform.GetComponentInChildren<Text>().text = Building.buildingName;

            //Get the RectTransform component from the Game object
            RectTransform rt = menuButtonObject.GetComponent<RectTransform>();
            //Set the scale to 1
            rt.localScale = new Vector3(1f, 1f, 1f);
            //Set the sizeDelta to the preferred width & height
            rt.sizeDelta = new Vector2(120, 20);

            //Get the button component from the game object
            Button menuButton = menuButtonObject.GetComponent<Button>();

            //Set listener on button
            SetListenerForBuild(Building.buildingType, menuButton, gc.executeBuild);

            //Add this newly created game object into the buttons list
            this.buttons.Add(menuButtonObject);

            //Select the first button to get it highlighted, so we can use the keyboard for navigating the menu as well!
            this.buttons[0].GetComponent<Button>().Select();

        }
    }


    //Function that updates the action menu with all the valid abilities on that tile
    // - Param 1: Dictionary of all possible selectable entities
    public void showSelectMenu(Dictionary<int, string> selectables) {
        //Clear all buttons
        this.buttons.Clear();
        this.removeButtonsFromMenu();

        //Set size of button container
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 0);

        //Loop through all selectables
        foreach(KeyValuePair<int, string> selectable in selectables) {
            //Create a new Game Object that instanciates with the prefab
            GameObject menuButtonObject = (GameObject)Instantiate(actionsMenuPrefab);

            //Set this monobehaviour as the parent
            menuButtonObject.transform.SetParent(this.transform);
            //Set the name of the game object
            menuButtonObject.name = "Selectable: " + selectable.Key;
            //Get the text component and set the text to the ability name
            menuButtonObject.transform.GetComponentInChildren<Text>().text = selectable.Value;

            //Get the RectTransform component from the Game object
            RectTransform rt = menuButtonObject.GetComponent<RectTransform>();
            //Set the scale to 1
            rt.localScale = new Vector3(1f, 1f, 1f);
            //Set the sizeDelta to the preferred width & height
            rt.sizeDelta = new Vector2(120, 20);

            //Get the button component from the game object
            Button menuButton = menuButtonObject.GetComponent<Button>();

            //Set listener on button
            SetListenerForSelectable(selectable.Key, menuButton, KeyBoardControls._instance.selectUnitFromMenu);

            //Add this newly created game object into the buttons list
            this.buttons.Add(menuButtonObject);

            //Select the first button to get it highlighted, so we can use the keyboard for navigating the menu as well!
            this.buttons[0].GetComponent<Button>().Select();
        }
    }


    //Sets an action listener to the button
    // - Param 1: The ability name
    // - Param 2: The button component to put the listener on
    // - Param 3: Action contains the name of the function which we invoke as soon as the listener is called
    private void SetListener(string ability, Button menuButton, Action action){
		menuButton.onClick.AddListener( delegate {
			action();
       	});
	}

    //Sets an action listener to the button for an ability, which requires more parameters
    // - Parem 1: The ability value (Used to specify which ability is going to be used)
    // - Param 2: The button component to put the listener on
    // - Param 3: Action with the list of strings, that the function that is getting invoked requires
	private void SetListenerForAbility(string ability, Button menuButton, Action<string, string> action) {
		menuButton.onClick.AddListener( delegate {
			action(ability, "");
		});
	}

    //Sets an action listener to the button for building
    // - Param 1: The building type that needs to be built
    // - Param 2: The button component to put the listener on
    // - Param 3: Action with the list of strings, that the function that is getting invoked requires
    private void SetListenerForBuild(string buildingType, Button menuButton, Action<string> action) {
        menuButton.onClick.AddListener( delegate {
            action(buildingType);
        });
    }

    //Sets an action listener to the button for selecting
    // - Param 1: The building type that needs to be built
    // - Param 2: The button component to put the listener on
    // - Param 3: Action with the list of strings, that the function that is getting invoked requires
    private void SetListenerForSelectable(int selectableInstanceID, Button menuButton, Action<int> action) {
        menuButton.onClick.AddListener( delegate {
            action(selectableInstanceID);
        });
    }

    //Function to destroy the button objects within this monobehaviour
    public void removeButtonsFromMenu(){
		//Loop through all child objects within this object
		for(int i = 0; i < this.transform.childCount; i++) {
			//Destory the child game object
			Destroy(this.transform.GetChild(i).gameObject);
		}
	}
}
