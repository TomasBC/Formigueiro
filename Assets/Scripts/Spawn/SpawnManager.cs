using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	public GameObject obj;              // The object prefab to be spawned.

	public int spawnLimit;				// Spawn limit.
	public float spawnTime = 3f;        // How long between each spawn.
	public Transform[] spawnPoints;     // An array of the spawn points this enemy can spawn from.

	void Start()
	{
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating("Spawn", spawnTime, spawnTime);
	}

	void Spawn()
	{		
		// Find a random index between zero and one less than the number of spawn points.
		int spawnPointIndex = Random.Range(0, spawnPoints.Length);
		
		if(Utils.FindGameObjectsByName(obj.name, obj.tag) < spawnLimit){
			// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
			Instantiate(obj, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
		}
	}
}
