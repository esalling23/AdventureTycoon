using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// An available activity
[System.Serializable]
public class ActivityBase : UniqueObject, IActivity
{
	#region Fields

	public string name = "Some Activity";
	public string description;
	public ActivityType type = ActivityType.PassTime;

	/// <summary>
	/// Maximum adventurers allowed to use this activity at a time
	/// </summary>
	public int capacity = 5;
	/// <summary>
	/// Cost for the adventurers to perform this activity
	/// </summary>
	public int costToUse = 0;
	/// <summary>
	/// Happiness gained from this succeeding this activity
	/// </summary>
	public int happinessEffect = 1;
	/// <summary>
	/// Health gained from this activity - mostly used by Rest activities
	/// </summary>
	public int healthEffect = 0;
	/// <summary>
	/// Min time an adventurer will take using this activity
	/// </summary>
	public float minTimeToUse = 0.5f;
	/// <summary>
	/// Max time an adventurer will take using this activity
	/// </summary>
	public float maxTimeToUse = 0.8f;

	#endregion

	#region Properties

	public string Name { get { return name; } }
	public string Description { get { return description; } }
	public int Capacity { get { return capacity; } }
	public ActivityType Type { get { return type; } }
	public int CostToUse { get { return costToUse; } }
	public float MinTimeToUse { get { return minTimeToUse * GameManager.Instance.MinutesInDay; } }
	public float MaxTimeToUse { get { return maxTimeToUse * GameManager.Instance.MinutesInDay; } }
	public float HappinessEffect { get { return happinessEffect; } }

	#endregion

	#region Methods

	public virtual bool Attempt(Adventurer adventurer)
	{
		return true;
	}

	#endregion
}

