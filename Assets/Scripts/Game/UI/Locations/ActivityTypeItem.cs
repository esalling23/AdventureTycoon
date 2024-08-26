using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityTypeItem : MonoBehaviour
{
    #region Fields

		private ActivityType _type;
		private ActivityTypeData _typeData;

		public Image icon;
		public TMP_Text nameText;
		public TMP_Text costText;

		public Button buyButton;

		#endregion

		#region Methods

		private void Start() 
		{
			EventManager.StartListening(EventName.OnPlayerGoldChanged, HandleOnPlayerGoldChanged);
		}
		private void OnDestroy() 
		{
			EventManager.StopListening(EventName.OnPlayerGoldChanged, HandleOnPlayerGoldChanged);
		}

    public void SetData(ActivityTypeData data)
    {
			_type = data.type;
			_typeData = data;

			// UI
			// nameText.text = _typeData.ToString();
			costText.text = _typeData.costToPlace.ToString();
			
			icon.sprite = _typeData.icon;

			CheckIsEnabled();
    }

		public void CheckIsEnabled()
		{
			bool isEnabled = true;

			if (GameManager.Instance.Gold < _typeData.costToPlace)
			{
				isEnabled = false;
			}

			buyButton.interactable = isEnabled;
		}

		public void HandleClickForBuild() {
			if (GameManager.Instance.Gold < _typeData.costToPlace)
			{
				Debug.LogError("Player doesn't have enough gold to build this activity type");
				return;
			}
			GameManager.Instance.UpdatePlayerGold(-_typeData.costToPlace);

			Debug.Log($"Activity type transaction completed. Player gold {GameManager.Instance.Gold}");

			EventManager.TriggerEvent(EventName.OnActivityTypeSelected, new Dictionary<string, object> {
				{ "type", _type }
			});
		}

		private void HandleOnPlayerGoldChanged(Dictionary<string, object> data = null)
		{
			CheckIsEnabled();
		}

		#endregion
}
