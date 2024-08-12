using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// An available activity
[System.Serializable]
public class Activity : UniqueObject
{
	#region Fields

	public string name = "Some Activity";
	public string description;
	public string failureMessage = "This adventurer has died during the quest attempt";
	public ActivityType type = ActivityType.PassTime;
	public LocationType[] appearsInLocationTypes;

	[Header("Control if this activity will be removed when `lifetime` is 0")]
	public bool hasLifetime = false;
	public int lifetime = 0;

	// Effect to be added to adventurers that use this activity
	// eg: +1 success on next quest, must do X quest next, more happiness from X activity type, more
	// public AdventurerEffect adventurerEffect;

	[Header("Some relative amount of change for adventurer stats upon use")]
	public int power = 1;

	[Header("Cost for the player to add to a location")]
	public int costToPlace = 50;

	[Header("Cost for the adventurers to perform this activity")]
	public int costToUse = 10;

	#endregion

	#region Properties

	#endregion
}

// An activity that is in a location on the map RIGHT NOW
[SerializeField]
public class MapActivity
{
	public Activity activityData;
	public MapLocation locationParent;
	public int currentHealthRemaining = 0;
	public List<Adventurer> adventurersPresent;

	public MapActivity(Activity activity, MapLocation mapLocation) 
	{
		this.activityData = activity;
		this.locationParent = mapLocation;
		this.adventurersPresent = new List<Adventurer>();

		if (activity.hasLifetime) {
			this.currentHealthRemaining = activity.lifetime;
		}
	}
}
