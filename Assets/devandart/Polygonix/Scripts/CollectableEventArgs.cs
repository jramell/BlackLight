using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Event arguments the player receives when a enemy is killed
/// </summary>
public class CollectableEventArgs : EventArgs {

	private Vector3 position;

	/// <summary>
	/// Position of the enemy when killed
	/// </summary>
	/// <value>The position.</value>
	public Vector3 Position {
		get {
			return position;
		}
		set {
			position = value;
		}
	}

    private int points;

	/// <summary>
	/// Points the player gets from this enemy
	/// </summary>
	/// <value>The points.</value>
    public int Points
    {
        get { return points; }
        set { points = value; }
    }

	private float collectedTime;

	/// <summary>
	/// The time when the enemy got killed
	/// </summary>
	/// <value>The killed time.</value>
	public float CollectedTime
	{
		get { return collectedTime; }
		set { collectedTime = value; }
	}
}
