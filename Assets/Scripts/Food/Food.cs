using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {

	public float energy = 30.0f;
	private bool inTransport = false;

	
	// Getters and setters
	public float GetEnergy()
	{
		return energy;
	}

	public bool GetTransport() 
	{
		return inTransport;
	}

	public float ConsumeEnergy(float percentage) 
	{
		float consumed = energy * percentage;
		energy -= consumed;
		
		return consumed;
	}

	public void SetTransport(bool value) 
	{
		inTransport = value;
	}
}