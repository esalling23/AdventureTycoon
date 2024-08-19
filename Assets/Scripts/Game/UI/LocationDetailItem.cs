using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles display of rolled locations that can be built
/// Used in the LocationBuildModal
/// </summary>
public class LocationDetailItem : MonoBehaviour
{
    #region Fields

		private Location _location;
		public TMP_Text nameText;
		public TMP_Text descriptionText;

		public Image icon;

		#endregion

		#region Methods

    public void SetData(Location location)
    {
			_location = location;

			// UI
			nameText.text = location.name;
      descriptionText.text = location.description;
			
			LocationTypeData defaultData = DataManager.Instance.GetLocationTypeData(location.type);
			icon.sprite = defaultData.icon;
    }

		public void HandleClickLocationForBuild() {
			EventManager.TriggerEvent(EventName.OnBuildLocationSelected, new Dictionary<string, object>() {
				{ "location", _location }
			});
		}

		#endregion
}
