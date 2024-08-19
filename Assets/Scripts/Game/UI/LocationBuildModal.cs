using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Modal that allows players to roll for locations of the bought type
/// for placing on the map
/// </summary>
public class LocationBuildModal : MonoBehaviour
{
    #region Fields

		[SerializeField] private int _locationGenerationCount = 3;

		// Displays location types to build
		[SerializeField] private GameObject _modalContainer;
		[SerializeField] private GameObject _buildShelf;
		[SerializeField] private RollBuildItem _rollBuildItemPrefab;
		private List<RollBuildItem> _rollBuildItems = new List<RollBuildItem>();

		// First roll is free, subsequent rolls will increase by _costToRollIncrementAmount
		private int _currentCostToRoll = 0;
		private int _costToRollIncrementAmount = 100;


		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			// initial hide
			// ClearBuildShelf();
			_modalContainer.SetActive(false);

			EventManager.StartListening(EventName.OnBuildOptionRolled, HandleOnBuildOptionRolled);
			EventManager.StartListening(EventName.OnBuildTypeSelected, HandleBuildTypeSelected);
			EventManager.StartListening(EventName.OnBuildLocationSelected, HandleBuildLocationSelected);
			EventManager.StartListening(EventName.OnPlayerGoldChanged, HandlePlayerGoldChanged);
    }

		public void InitTypeLocations(LocationType type)
		{
			ClearBuildShelf();
			_currentCostToRoll = 0;
			_rollBuildItems = new List<RollBuildItem>();

			for (int i = 0; i < _locationGenerationCount; i++)
			{
				// Show roll build items
				RollBuildItem item = Instantiate(
					_rollBuildItemPrefab,
					Vector3.zero,
					Quaternion.identity,
					_buildShelf.transform
				);
				item.CheckCanBuy(_currentCostToRoll);
				_rollBuildItems.Add(item);
			}
			_buildShelf.SetActive(true);
		}

		void ClearBuildShelf() {
			foreach (Transform child in _buildShelf.transform) {
				Destroy(child.gameObject);
			}
			_rollBuildItems.Clear();
			Debug.Log("Cleared location rollables");
			Debug.Log(_rollBuildItems.Count);
		}

		public void OnClickAbandonButton()
		{
			// confirmation?

			ClearBuildShelf();
			_modalContainer.SetActive(false);
		}

		#region Event Handlers

		void HandleBuildLocationSelected(Dictionary<string, object> data) 
		{
			_modalContainer.SetActive(false);
		}
		void HandleBuildTypeSelected(Dictionary<string, object> data) 
		{
			if (data.TryGetValue("type", out object type))
			{
				Debug.Log($"Build type {type} selected");
				if (System.Enum.TryParse(type.ToString(), out LocationType locationType))
				{
					Debug.Log("BUild modal heard type selected");
					_modalContainer.SetActive(true);
					InitTypeLocations(locationType);
				}
			}
		}

		void HandleOnBuildOptionRolled(Dictionary<string, object> data)
		{
			GameManager.Instance.UpdatePlayerGold(-_currentCostToRoll);

			_currentCostToRoll += _costToRollIncrementAmount;

			EventManager.TriggerEvent(EventName.OnRollCostChanged, new Dictionary<string, object>() {
				{ "cost", _currentCostToRoll }
			});
		}

		private void HandlePlayerGoldChanged(Dictionary<string, object> data)
		{
			foreach (RollBuildItem item in _rollBuildItems)
			{
				item.CheckCanBuy(_currentCostToRoll);
			}
		}

		#endregion

		#endregion
}
