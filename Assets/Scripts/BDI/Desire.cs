using UnityEngine;
using System.Collections;

public class Desire 
{
	private DesireType desireType;
	private Transform desireDest;

	private int danger;
	private int confidence;
	private int desireValue;

	// Constructor
	public Desire(DesireType desireType, Transform desireDest, int danger, int confidence, int priority)
	{
		this.desireType = desireType;
		this.desireDest = desireDest;
		this.danger = danger;
		this.confidence = confidence;
		this.desireValue = (confidence - danger) + priority;
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

	public int Danger 
	{
		get { return danger; }
	}

	public int Confidence 
	{
		get { return confidence; }
	}

	public int DesireValue 
	{
		get { return desireValue; }
	}
}