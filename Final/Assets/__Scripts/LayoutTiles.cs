﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class TileTex {
	// This class enables us to define various textures for tiles
	public string str;
	public Texture2D tex;
}

public class LayoutTiles : MonoBehaviour {
	static public LayoutTiles S;
	public TextAsset levelText; // The Levels.xml file
	public string levelNumber = "0"; // Current level # as a string
	public GameObject tilePrefab; // Prefab for all Tiles
	public GameObject goalPrefab; // Prefab for all Tiles
	public GameObject playerPrefab; // Prefab for player
	public bool ________________;
	public PT_XMLReader levelsXMLR;
	public PT_XMLHashList levelsXML;
	public Tile[,,] tiles;
	public Transform tileAnchor;
	void Start(){
		S = this; // Set the Singleton for LayoutTiles
		// Make a new GameObject to be the TileAnchor (the parent transform of
		// all Tiles). This keeps Tiles tidy in the Hierarchy pane.
		GameObject tAnc = new GameObject("TileAnchor");
		tileAnchor = tAnc.transform;
		Utils.tr("..........",tileAnchor);
		// Read the XML
		levelsXMLR = new PT_XMLReader(); // Create a PT_XMLReader
		levelsXMLR.Parse(levelText.text); // Parse the Rooms.xml file
		levelsXML = levelsXMLR.xml["xml"][0]["level"]; // Pull all the <room>s
		// Build the 0th Room
		BuildLevel(levelNumber);
	}
	public void BuildLevel(PT_XMLHashtable level) {
		// Destroy any old Tiles
		foreach (Transform t in tileAnchor) { // Clear out old tiles
			// ^ You can iterate over a Transform to get its children
			Destroy(t.gameObject);
		}
		string lNumStr = level.att("num");
		// TODO: Get Start point  ( x_start, y_start )
		// TODO: Get Goal point   ( x_goal,  y_goal  )
		int x_start = int.Parse (level.att ("x_start"));
		int y_start = int.Parse (level.att ("y_start"));
		int x_goal = int.Parse (level.att ("x_goal"));
		int y_goal = int.Parse (level.att ("y_goal"));
		string[] levelRows = level.text.Split('\n');
		for (int i=0; i<levelRows.Length; i++) {
			levelRows[i] = levelRows[i].Trim('\t');
		}
		// Clear the tiles Array
		tiles = new Tile[ 100, 100 , 10 ]; // Arbitrary max room size is 100x100x10
		// Declare a number of local fields that we'll use later
		Tile ti;
		string type;
		GameObject go;
		int height;
		float maxY = levelRows.Length-1;
		// These loops scan through each tile of each row of the room
		for (int y=0; y<levelRows.Length; y++) {
			for (int x=0; x<levelRows[y].Length; x++) {
				// Set defaults
				height = 0;
				// Get the character representing the tile
				type = levelRows [y] [x].ToString ();
				switch (type) {
				case ".":
				case " ":
				case "_":
					height = 0;
					break;
				default:
					height = int.Parse(type);
					for (int h = 0; h < height; h++) {
						if (y == y_goal && x == x_goal && h == height-1){
							// Instantiate a new TilePrefab
							go = Instantiate (goalPrefab) as GameObject;
						}else{
							// Instantiate a new TilePrefab
							go = Instantiate (tilePrefab) as GameObject;
						}
						ti = go.GetComponent<Tile> ();
						// Set the parent Transform to tileAnchor
						ti.transform.parent = tileAnchor;
						// Set the position of the tile
						ti.pos = new Vector3 (x,h*0.5f,y);
						tiles [x, h, y] = ti; // Add ti to the tiles 2D Array
						// Set the type, height, and texture of the Tile
						ti.type = "baldosa";
					}
					if (y == y_start && x == x_start){
						go = Instantiate (playerPrefab) as GameObject;
						ti = go.GetComponent<Tile> ();
						// Set the parent Transform to tileAnchor
						ti.transform.parent = tileAnchor;
						// Set the position of the tile
						ti.pos = new Vector3 (x,height*0.5f,y);
						tiles [x, height, y] = ti; // Add ti to the tiles 2D Array
						// Set the type, height, and texture of the Tile
						ti.type = "player";
					}

					break;
				}
			}
		}

	}
	// Build a level based on level number. This is an alternative version of
	// BuildLevel that grabs levelXML based on num.
	public void BuildLevel(string lNumStr) {
		PT_XMLHashtable levelHT = null;
		for (int i=0; i<levelsXML.Count; i++) {
			PT_XMLHashtable ht = levelsXML[i];
			if (ht.att("num") == lNumStr) {
				levelHT = ht;
				break;
			}
		}
		if (levelHT == null) {
			Utils.tr("ERROR","LayoutTiles.BuildLevel()",
			         "Level not found: "+lNumStr);
			return;
		}
		BuildLevel(levelHT);
	}
}