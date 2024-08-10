using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(GameManager))]
public class GameMangaerDataHandler : Editor
{

	private string Path { get { return "Data/save_data"; }}
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GameManager manager = (GameManager) target;
		if (GUILayout.Button("Save Data"))
		{
			Debug.Log(manager.WorldLocations.Length);
			Data data = new Data(
				manager.WorldLocations,
				manager.WorldActivities
			);
			string jsonData = JsonUtility.ToJson(data);
			System.IO.File.WriteAllText(Application.dataPath + "/Resources/" + Path + ".json", jsonData);
		}
		else if (GUILayout.Button("Load Data"))
		{
			string json = Resources.Load<TextAsset>(Path).text;
			Data dataToLoad = JsonUtility.FromJson<Data>(json);

			manager.LoadData(dataToLoad);
						
			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			#endif
		}
	}
}