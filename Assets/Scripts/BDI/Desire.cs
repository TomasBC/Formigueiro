using UnityEngine;
using System.Collections;

public class Desire 
{
	private DesireType desireType;
	private GameObject desireObject;

	private float danger;
	private float confidence;
	private float desireValue;

	// Constructor
	public Desire(DesireType desireType, GameObject desireObject, float danger, float confidence)
	{
		this.desireType = desireType;
		this.desireObject = desireObject;
		this.danger = danger;
		this.confidence = confidence;
		this.desireValue = confidence - danger;
	}

	// Getters
	public DesireType DesireType 
	{
		get { return desireType; }
	}

	public GameObject DesireObject 
	{
		get { return desireObject; }
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