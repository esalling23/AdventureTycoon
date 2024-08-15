using UnityEngine;

// A single, lore-filled location
[System.Serializable]
public class Location : UniqueObject
{

	public string name = "Map Location";
	public string description;

	public int activitySlotCount = 2;

	public LocationType type = LocationType.Inn;

	public int costToPlace = 500;

	// to do - store icon cdn address to load in game
	[Header("Optional custom icon. Will default to type icon.")]
	public Sprite icon;
}