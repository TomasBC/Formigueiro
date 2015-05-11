using UnityEngine;
using System.Collections;

public class Intention 
{
	private Desire desire;
	private DesireType type;

	private float intentionValue;
	private GameObject intentionObject;

	// Constructor
	public Intention(Desire desire)
	{
		this.desire = desire;
		this.type = desire.DesireType;

		this.intentionValue = desire.DesireValue;
		this.intentionObject = desire.DesireObject;
	}

	// Getters
	public Desire Desire 
	{
		get { return desire; }
	}

	public DesireType Type 
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