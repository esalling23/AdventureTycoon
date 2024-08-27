using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VIPListItem : MonoBehaviour
{
    #region Fields

		private MapVIP _mapVIP;
		[SerializeField] private ActivityListItem _questItemPrefab;
		[SerializeField] private Transform _questItemsContainer;
		private List<ActivityListItem> _questItems;

		// VIP display
		public Image icon;
		public TMP_Text nameText;
		public TMP_Text lifetimeText;
		public TMP_Text descriptionText;
		public GameObject healthBar;
		public TMP_Text healthRemainingText;

		#endregion

		#region Properties

		// public string Property { get; set; }

		#endregion

		#region Methods

    public void SetData(MapVIP mapVip)
    {
			_mapVIP = mapVip;

      nameText.text = mapVip.Data.name;
			lifetimeText.text = $"leaving in {mapVip.Data.lifetime - mapVip.Age} days";

			foreach (Transform child in _questItemsContainer.transform)
			{
				Destroy(child.gameObject);
			}

			_questItems = new();

			foreach (MapActivity quest in mapVip.MapQuests)
			{
				ActivityListItem item = Instantiate(
					_questItemPrefab,
					Vector3.zero,
					Quaternion.identity,
					_questItemsContainer
				);

				item.SetData(quest);

				_questItems.Add(item);
			}
    
		}

		#endregion
}
