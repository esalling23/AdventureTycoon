using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles display of map locations that can be selected for various actions
/// </summary>
public class MapLocationDetailItem : MonoBehaviour
{
    #region Fields

		private MapLocation _mapLocation;
		public TMP_Text nameText;
		public TMP_Text descriptionText;
		public Image icon;

		#endregion

		#region Methods

    public void SetData(MapLocation location)
    {
			_mapLocation = location;

			Location data = location.LocationData;

			// UI
			nameText.text = data.name;
      descriptionText.text = data.description;
			
			LocationTypeData defaultData = DataManager.Instance.GetLocationTypeData(data.type);
			icon.sprite = defaultData.icon;
    }
		
		public void HandleSelectLocationForVIP() {
			VIPManager.Instance.PlaceCurrentVIP(_mapLocation);
		}

		#endregion
}
