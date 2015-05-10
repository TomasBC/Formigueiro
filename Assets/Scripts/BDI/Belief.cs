using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Belief 
{
	private GameObject beliefObject;
	private BeliefType type;
	
	// Constructor
	public Belief(GameObject beliefObject, BeliefType type)
	{
		this.beliefObject = beliefObject;
		this.type = type;
	}
	
	// Getters
	public GameObject BeliefObject 
	{
		get { return beliefObject; }
	}

	public BeliefType Type 
	{
		get { return type; }
	}
}