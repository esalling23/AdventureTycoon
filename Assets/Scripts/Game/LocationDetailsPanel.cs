using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationDetailsPanel : MonoBehaviour
{
    #region Fields

		[SerializeField] private Map _map;
		public TMP_Text nameText;
		public TMP_Text descriptionText;

		public GameObject activitiesContainer;

		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			gameObject.SetActive(false);

			// EventManager.StartListening(EventName.OnGridValueChanged, HandleGridValueChanged);
    }

		// public void HandleGridValueChanged(Dictionary<string, object> data) {
		// 	if (data.TryGetValue("coords", out object coords)) {
		// 		Vector2Int vectCoords = (Vector2Int) coords;
		// 		MapLocation location = 
		// 	}
		// }

		public void SetLocationData(MapLocation location) {
			nameText.text = location.name;
			descriptionText.text = location.description;


		}

		#endregion
}
