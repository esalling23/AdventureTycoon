using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActivityType {
	Rest,
	Quest,
	Trade,
	PassTime
}

[System.Serializable]
public class LocationActivity
{
    #region Fields

		public string name = "Some Activity";
		public string description;
		public ActivityType type = ActivityType.PassTime;

		[Header("Some relative amount of change for adventurer stats")]
		public int power = 1;

		#endregion
}
