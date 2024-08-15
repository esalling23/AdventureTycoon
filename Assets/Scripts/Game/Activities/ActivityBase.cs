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

	[Header("Maximum adventurers allowed to use this activity at a time")]
	public int capacity = 5;

	[Header("Cost for the adventurers to perform this activity")]
	public int costToUse = 0;

	#endregion
	#region Properties

	public string Name { get { return name; } }
	public string Description { get { return description; } }
	public int Capacity { get { return capacity; } }
	public ActivityType Type { get { return type; } }
	public int CostToUse { get { return costToUse; } }

	#endregion
}

