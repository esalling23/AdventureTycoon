using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationDetailItem : MonoBehaviour
{
    #region Fields

		private Map _map;

		private Location _location;
		public TMP_Text nameText;
		public TMP_Text descriptionText;

		public Image icon;

		#endregion

		#region Methods

		private void Start() 
		{
			_map = GameObject.FindWithTag("Map").GetComponent<Map>();
		}

    public void SetData(Location location)
    {
			_location = location;

			// UI
			nameText.text = location.name;
      descriptionText.text = location.description;
			
			GameManager manager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
			LocationTypeData defaultData = manager.GetLocationTypeData(location.type);
			icon.sprite = defaultData.icon;
    }

		public void HandleClickLocationForBuild() {
			_map.SetLocationToBuild(_location);

			EventManager.TriggerEvent(EventName.OnBuildLocationSelected, null);
		}

		#endregion
}
