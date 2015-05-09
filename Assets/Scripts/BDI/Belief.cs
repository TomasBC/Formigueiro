using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Belief {
	protected BeliefsTypes type;
	protected GameObject beliefObject;

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
