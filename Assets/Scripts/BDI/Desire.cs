using UnityEngine;
using System.Collections;

public class Desire 
{
	private DesireType desireType;
	private Transform desireDest;

	private float danger;
	private float confidence;
	private float desireValue;

	// Constructor
	public Desire(DesireType desireType, Transform desireDest, float danger, float confidence)
	{
		this.desireType = desireType;
		this.desireDest = desireDest;
		this.danger = danger;
		this.confidence = confidence;
		this.desireValue = confidence - danger;
	}

	// Getters
	public DesireType DesireType 
	{
		get { return desireType; }
	}

	public Transform DesireDest 
	{
		get { return desireDest; }
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