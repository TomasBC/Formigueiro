using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Belief {

	protected BeliefsTypes type;

	public BeliefsTypes Type {
		get {
			return type;
		}
	}

	protected GameObject beliefObject;

	public GameObject BeliefObject {
		get {
			return beliefObject;
		}
	}

	protected int enemiesCount;
	public int EnemiesCount{
		get{return enemiesCount;}
	}
	public int FriendsCount{
		get{return friendsCount;}
	}
	protected int friendsCount;

	public Belief(GameObject bo, BeliefsTypes type, int enemies, int friends){
		beliefObject = bo;
		this.type = type;
		this.enemiesCount = enemies;
		this.friendsCount = friends;
	}
}
