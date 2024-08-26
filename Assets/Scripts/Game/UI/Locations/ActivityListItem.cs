using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityListItem : MonoBehaviour
{
    #region Fields

		private MapActivity _mapActivity;
		private bool _isConfirmed = false;
		private int _rollCountRemaining = 1;

		public TMP_Text titleText;
		public TMP_Text descriptionText;
		public TMP_Text adventurerCountText;
		public Image typeIcon;
		
		[Header("Reroll & Confirm buttons used upon creation")]
		public GameObject creationButtons;


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
			_mapActivity = activity;

      titleText.text = activity.data.Name;
      // descriptionText.text = activity.data.Description;

			// healthBar.SetActive(false);
			// healthBar.SetActive(activity.data.hasLifetime);
			// healthRemainingText.text = activity.currentHealthRemaining.ToString();

			// Debug.Log($"Displaying a count of {activity.adventurersPresent.Count} adventurers");
			adventurerCountText.text = activity.adventurersPresent.Count.ToString();

			ActivityTypeData defaultData = DataManager.Instance.GetActivityTypeData(activity.data.Type);
			typeIcon.sprite = defaultData.icon;
    }

		private void 

    public void HandleClickReroll()
    {
			if (_isConfirmed || _rollCountRemaining <= 0) return;

			_rollCountRemaining--;
      _mapActivity.TryReroll();
    }
    public void HandleClickConfirm()
    {
			_isConfirmed = true;
      _mapActivity.TryReroll();
    }

		#endregion
}
