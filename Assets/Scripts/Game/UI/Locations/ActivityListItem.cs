using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityListItem : MonoBehaviour
{
    #region Fields

		private MapActivity _mapActivity;

		public Image backgroundImage;
		public GameObject detailsContainer;

		public TMP_Text titleText;
		public TMP_Text descriptionText;
		public TMP_Text capacityStat;
		public Stat healthStat;
		public Stat happinessStat;
		public Stat incomeStat;
		public Image typeIcon;

		public GameObject rerollButton;
		public GameObject removeButton;


		// VIP display
		public Image vipIcon;
		public GameObject healthBar;
		public TMP_Text healthRemainingText;

		#endregion

		#region Properties

		// public string Property { get; set; }

		#endregion

		#region Methods

		void Start()
		{
			EventManager.StartListening(EventName.OnActivityChanged, HandleOnActivityChanged);
		}

		void OnDestroy()
		{
			EventManager.StopListening(EventName.OnActivityChanged, HandleOnActivityChanged);
		}

    public void SetData(MapActivity activity)
    {
			_mapActivity = activity;

      titleText.text = activity.data.Name;
      // descriptionText.text = activity.data.Description;

			// healthBar.SetActive(false);
			// healthBar.SetActive(activity.data.hasLifetime);
			// healthRemainingText.text = activity.currentHealthRemaining.ToString();
			ActivityBase activityBase = (ActivityBase) activity.data;
			capacityStat.text = $"{activity.adventurersPresent.Count} / {activityBase.Capacity}";

			healthStat.SetStat(
				activityBase.HealthEffect != 0, 
				$"+{activityBase.healthEffect}"
			);
			happinessStat.SetStat(
				activityBase.HappinessEffect > 0, 
				$"+{activityBase.happinessEffect}"
			);
			incomeStat.SetStat(
				activityBase.CostToUse > 0, 
				$"+{activityBase.CostToUse}g"
			);

			ActivityTypeData defaultData = DataManager.Instance.GetActivityTypeData(activityBase.Type);
			typeIcon.sprite = defaultData.icon;

			DisplayFilled();

			CheckForActions(activity.locationParent);
    }

		public void DisplayFilled(bool isFilled = true)
		{
			detailsContainer.SetActive(isFilled);

			Color bgColor = isFilled ? Color.white : Color.grey;
			backgroundImage.color = bgColor;
		}

		public void CheckForActions(MapLocation location)
		{
			rerollButton.SetActive(location.ActivityRollsAvailable > 0);
			removeButton.SetActive(location.ActivityRemovesAvailable > 0);
		}

    public void HandleClickReroll()
    {
			_mapActivity.Reroll();
    }
    public void HandleClickRemove()
    {
			_mapActivity.RemoveSelf();
    }

		private void HandleOnActivityChanged(Dictionary<string, object> data) {
			if (_mapActivity == null) return;

			if (data.TryGetValue("event", out object activityEvent) && data.TryGetValue("id", out object mapActivityId))
			{
				System.Enum.TryParse(activityEvent.ToString(), out ActivityChangeEvent evt);
				
				if (evt != ActivityChangeEvent.Update) return;
				if (_mapActivity.Id.CompareTo(mapActivityId) == 0) return;
				
				SetData(_mapActivity);
			}
		}

		#endregion
}
