using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

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

	private Transform boardHolder;
	private List<Vector3> gridPositions =  new List<Vector3>();

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

	void BoardSetup() 
	{
		boardHolder = new GameObject ("Board").transform;

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


	void LayoutObjectAtRandom (GameObject[] tileArray, Count range)
	{
		int objectCount = Random.Range (range.minimum, range.maximum + 1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity);
			instance.transform.SetParent(boardHolder);
		}
	}

	public void SetupScene() 
	{
		ClearBoard ();
		BoardSetup ();
		InitializeList ();
		LayoutObjectAtRandom (destructibleTiles, new Count(destructibleCount/2, destructibleCount));
	}

	void ClearBoard()
	{
		if (boardHolder) {
			Destroy (boardHolder.gameObject);
		}
	}

	public void AddRandomHeart() {
		Vector3 randomPosition = RandomPosition();
		GameObject instance = Instantiate (heartObject, randomPosition, Quaternion.identity);
		instance.transform.SetParent (boardHolder);
	}

}
