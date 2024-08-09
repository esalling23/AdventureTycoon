using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationDetailsPanel : MonoBehaviour
{
    #region Fields

		[SerializeField] private Map _map;
		private MapLocation _activeLocation;

		public TMP_Text nameText;
		public TMP_Text descriptionText;

		public ActivityListItem activityListItemPrefab;
		public GameObject activitiesContainer;

		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			gameObject.SetActive(false);

			EventManager.StartListening(EventName.OnActivityChanged, HandleActivityChanged);
    }

		public void HandleActivityChanged(Dictionary<string, object> _data = null) {
			DisplayActivities();
		}

		public void SetLocationData(MapLocation location) {
			_activeLocation = location;

			nameText.text = location.name;
			descriptionText.text = location.description;

			DisplayActivities();
		}

		private void DisplayActivities() {
			// to do - don't delete all, just replace data & remove extras
			foreach(Transform child in activitiesContainer.transform)
			{
				Destroy(child.gameObject);
			}

			foreach(LocationActivity activity in _activeLocation.activities) {
				ActivityListItem item = Instantiate(
					activityListItemPrefab,
					Vector3.zero,
					Quaternion.identity,
					activitiesContainer.transform
				);

				item.SetData(activity);
			}
		}

		public void HandleClickAddActivity() {
			// to do - show a generated set of activities & allow user to pick one
			_activeLocation.AddRandomActivity();
		}

		#endregion
}
