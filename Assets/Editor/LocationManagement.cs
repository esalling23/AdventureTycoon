using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapLocation))]
public class LocationManagement : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		MapLocation location = (MapLocation) target;
		if (GUILayout.Button("Add Activity Reroll"))
		{
			location.AddActivityReroll();
		}

		if (GUILayout.Button("Add Activity Remove Available"))
		{
			location.AddActivityRemove();
		}

		if (GUILayout.Button("Add Remove Available"))
		{
			location.AddActivitySlot();
		}

		if (GUILayout.Button("Kick Out All Adventurers"))
		{
			foreach (MapActivity activity in location.activities)
			{
				activity.KickOutAll();
			}
		}
	}
}