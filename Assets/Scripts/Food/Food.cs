using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {

	public float energy = 30.0f;
	private bool transport = false;
	
	// Getters and setters
	public float Energy 
	{
		get { return energy; }
	}

	public bool Transport 
	{
		get { return transport; }
		set { transport = value; }
	}

	// Consume food energy
	public float ConsumeEnergy(float percentage) 
	{
		float consumed = energy * percentage;
		energy -= consumed;
		
		return consumed;
	}
}