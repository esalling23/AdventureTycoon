using UnityEngine;

[System.Serializable]
// Categories of locations
// All categories might contain available quests or amenities
public enum LocationType 
{
  City,
	Town,
	Inn,
	Camp,
	Dungeon,
	Building,
	Castle,
}

// Shared default data for different location types
[System.Serializable]
public struct LocationTypeData {
	public LocationType type;

	public Sprite icon;
	public int baseAmenitySlots;
}
