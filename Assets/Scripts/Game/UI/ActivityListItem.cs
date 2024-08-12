using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityListItem : MonoBehaviour
{
    #region Fields

		public TMP_Text titleText;
		public TMP_Text descriptionText;

		public Image typeIcon;

		// to do - better visual representation of health
		public GameObject healthBar;
		public TMP_Text healthRemainingText;

		#endregion

		#region Properties

		// public string Property { get; set; }

		#endregion

		#region Methods

    public void SetData(MapActivity activity)
    {
      titleText.text = activity.activityData.name;
      descriptionText.text = activity.activityData.description;

			healthBar.SetActive(activity.activityData.hasLifetime);
			healthRemainingText.text = activity.currentHealthRemaining.ToString();

			ActivityTypeData defaultData = DataManager.Instance.GetActivityTypeData(activity.activityData.type);
			typeIcon.sprite = defaultData.icon;
    }

    void Update()
    {
        
    }

		#endregion
}
