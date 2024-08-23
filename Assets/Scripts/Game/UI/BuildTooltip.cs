using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// On-hover tooltip displays location or location type details during build loop
/// </summary>
public class BuildTooltip : MonoBehaviour
{
    #region Fields

		private Map _map;

		private Location _location;
		private LocationType _type;
		private LocationTypeData _typeData;

		public TMP_Text nameText;
		public TMP_Text descriptionText;
		public TMP_Text sizeText;

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
			
			LocationTypeData defaultData = DataManager.Instance.GetLocationTypeData(location.type);
			icon.sprite = defaultData.icon;
    }

		public void SetData(LocationType locationType)
    {
			_type = locationType;
			_typeData = DataManager.Instance.GetLocationTypeData(_type);

			// UI
			nameText.text = _typeData.ToString();
			sizeText.text = _typeData.baseActivitySlots.ToString();
      // descriptionText.text = location.description;
			
			icon.sprite = _typeData.icon;
    }

		#endregion
}
