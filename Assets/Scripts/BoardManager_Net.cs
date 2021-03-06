﻿using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

public class BoardManager_Net : NetworkBehaviour {

	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count(int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int rows = 15;
	public int columns = 15;

	public GameObject[] floorTiles;
	public GameObject[] borderWallTiles;

	public int destructibleCount = 100;
	public GameObject[] destructibleTiles;

	public GameObject heartObject;
	public GameObject nuclearPickup;

	public int grassFieldSize = 9;
	public GameObject grassObject;

	public GameObject objectsHolderPrefab;

	private Transform boardHolder;
	private GameObject objectsHolder;

	private List<Vector3> gridPositions =  new List<Vector3>();

	[Server]
	void InitializeList()
	{
		gridPositions.Clear ();

		for (int x = 0; x < columns; x++) 
		{
			for (int y = 1; y < rows - 1; y++) 
			{
				if (!((x > columns / 2 - 1 && x < columns / 2 + 2) && (y == 2 || y == rows - 2))) {
					gridPositions.Add (new Vector3 (x,y,0f));
				}
			}
		}
	}

	[Server]
	void InitObjectsHolder ()
	{
		objectsHolder = Instantiate(objectsHolderPrefab);
		NetworkServer.Spawn (objectsHolder);
	}
		
	public void BoardSetup() 
	{
		if (isClient) {
			ClientScene.RegisterPrefab (grassObject);
		}

		GameObject board = new GameObject ("Board");
		boardHolder = board.transform;


		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				if (x == -1 || x == columns || y == -1 || y == rows) {
					toInstantiate = borderWallTiles[Random.Range(0, borderWallTiles.Length)];
				}

				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent(boardHolder);
			}
		}
	}

	Vector3 RandomPosition() 
	{
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	[Server]
	void LayoutObjectAtRandom (GameObject[] tileArray, Count range)
	{
		int objectCount = Random.Range (range.minimum, range.maximum + 1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity);
			instance.transform.SetParent(objectsHolder.transform);
			NetworkServer.Spawn (instance);
		}
	}

	[Server]
	void AddRandomGrassField () {

		int randomX = Random.Range (1, columns - grassFieldSize - 1);
		int randomY = Random.Range (1, rows - grassFieldSize - 1);


		for (int x = randomX; x < (randomX + grassFieldSize); x++) {
			for (int y = randomY; y < (randomY + grassFieldSize); y++) {
				int randomSeed = Random.Range (0, 3);
				if (randomSeed < 2) {
					GameObject grass = Instantiate (grassObject, new Vector3(x,y,0.0f), Quaternion.identity);
					grass.transform.SetParent (objectsHolder.transform);
					NetworkServer.Spawn (grass);

					Predicate<Vector3> vectorPred = (Vector3 v) => {
						return (v.x == (float)x && v.y == (float)y);
					};
					int index = gridPositions.FindIndex (vectorPred);
					if (index >= 0 && index < gridPositions.Count) {
						gridPositions.RemoveAt (index);
					}
				}
			}
		}
	}

	[Server]
	public void CmdSetupScene() 
	{
		//RpcClearBoard ();
		BoardSetup ();
		InitObjectsHolder ();
		InitializeList ();
		AddRandomGrassField ();
		LayoutObjectAtRandom (destructibleTiles, new Count(destructibleCount/2, destructibleCount));
	}

	[Server]
	public void ClearBoard()
	{
		if (boardHolder) {
			Destroy (boardHolder.gameObject);
		}
	}

	[Server]
	public void ClearObjects ()
	{
		if (objectsHolder) {
			NetworkServer.Destroy (objectsHolder);
		}
	}

	[Server]
	public void AddRandomHeart() {
		SpawnObjectAtRandomPosition (heartObject);
	}

	[Server]
	public void AddRandomNuclearPickup() {
		SpawnObjectAtRandomPosition (nuclearPickup);
	}

	[Server]
	public void SpawnObjectAtRandomPosition(GameObject obj) {
		Vector3 randomPosition = RandomPosition();
		GameObject instance = Instantiate (obj, randomPosition, Quaternion.identity);
		instance.transform.SetParent (boardHolder);
		NetworkServer.Spawn (instance);
	}
}
