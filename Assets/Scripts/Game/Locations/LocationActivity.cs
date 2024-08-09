using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ActivityType {
	Rest,
	Quest,
	Trade,
	PassTime
}

[System.Serializable]
public struct ActivityTypeData {
	public ActivityType type;
	public Sprite icon;
}

[System.Serializable]
public class LocationActivity
{
    #region Fields

		private System.Guid _id = System.Guid.NewGuid();
		public string name = "Some Activity";
		public string description;
		public ActivityType type = ActivityType.PassTime;

		[Header("Some relative amount of change for adventurer stats")]
		public int power = 1;

		public int costToBuild = 10;

		#endregion

		#region Properties

		public System.Guid Id { get { return _id; } }

		#endregion
}
