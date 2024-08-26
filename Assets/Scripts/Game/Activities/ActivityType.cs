using System.Collections;
using UnityEngine;

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
	public int costToPlace;
}