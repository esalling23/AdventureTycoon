using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Adventurer))]
public class AdventurerManagement : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Adventurer adventurer = (Adventurer) target;
		if (GUILayout.Button("Kick Out of Activity"))
		{
			adventurer.KickOut();
		}

		if (GUILayout.Button("Kill"))
		{
			adventurer.Kill();
		}
	}
}