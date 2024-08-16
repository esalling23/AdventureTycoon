

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Map))]
public class ShowGridDebug : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Map map = (Map) target;
		if (GUILayout.Button("Show Debug"))
		{
			map.Grid.ShowDebug();
		}
	}
}