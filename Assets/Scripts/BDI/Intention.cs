using UnityEngine;
using System.Collections;

public class Intention 
{
	private Desire desire;
	private BeliefType type;

	private float intentionValue;
	private GameObject intentionObject;

	// Constructor
	public Intention(Desire desire)
	{
		this.desire = desire;
		this.type = desire.Belief.Type;

		this.intentionValue = desire.DesireValue;
		this.intentionObject = desire.Belief.BeliefObject;
	}

	// Getters
	public Desire Desire 
	{
		get { return desire; }
	}

	public BeliefType Type 
	{
		get { return type; }
	}

	public float IntentionValue 
	{
		get { return intentionValue; }
	}

	public GameObject IntentionObject
	{
		get{ return intentionObject; }
	}
}