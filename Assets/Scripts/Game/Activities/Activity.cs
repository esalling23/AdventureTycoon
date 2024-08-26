using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// An available activity
[System.Serializable]
public class Activity : ActivityBase
{
	#region Fields

	public LocationType[] appearsInLocationTypes;

	// Effect to be added to adventurers that use this activity
	// eg: +1 success on next quest, must do X quest next, more happiness from X activity type, more
	// public AdventurerEffect adventurerEffect;

	#endregion
	#region Properties

	public override bool Attempt(Adventurer adventurer)
	{
		if (Type == ActivityType.Rest)
		{
			adventurer.ChangeHealth(healthEffect);
		}
		adventurer.ChangeHappiness(happinessEffect);
		return base.Attempt(adventurer);
	}

	#endregion
}

