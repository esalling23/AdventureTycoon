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
		public TMP_Text adventurerCountText;

		public Image typeIcon;

		// VIP display
		public Image vipIcon;
		public GameObject healthBar;
		public TMP_Text healthRemainingText;

		#endregion

		#region Properties

		// public string Property { get; set; }

		#endregion

		#region Methods

    public void SetData(MapActivity activity)
    {
      titleText.text = activity.data.Name;
      descriptionText.text = activity.data.Description;

			healthBar.SetActive(false);
			// healthBar.SetActive(activity.data.hasLifetime);
			// healthRemainingText.text = activity.currentHealthRemaining.ToString();

			adventurerCountText.text = activity.adventurersPresent.Count.ToString();

			ActivityTypeData defaultData = DataManager.Instance.GetActivityTypeData(activity.data.Type);
			typeIcon.sprite = defaultData.icon;
    }

    void Update()
    {
        
    }

		#endregion
}
