using UnityEngine;
using System.Collections;

public class Desire 
{
	private Belief belief;

	private float danger;
	private float confidence;
	private float desireValue;

	// Constructor
	public Desire(Belief belief, float danger, float confidence)
	{
		this.belief = belief;
		this.danger = danger;
		this.confidence = confidence;
		this.desireValue = confidence - danger;
	}

	// Getters
	public Belief Belief 
	{
		get { return belief; }
	}

	public float Danger 
	{
		get { return danger; }
	}

	public float Confidence 
	{
		get { return confidence; }
	}

	public float DesireValue 
	{
		get { return desireValue; }
	}
}