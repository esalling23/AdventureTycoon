using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Handles display of a rollable item (location, activity)
/// </summary>
public class RollBuildItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Fields

		private Location _location;

		[SerializeField] private Button _selectButton;
		[SerializeField] private GameObject _errorMessage;
		[SerializeField] private GameObject _costToRollContainer;
		[SerializeField] private TMP_Text _costToRollText;

		[SerializeField] private GameObject[] _hoverStateObjs;
		[SerializeField] private GameObject _preRollDisplay;
		[SerializeField] private LocationDetailItem _postRollDetails;

		#endregion

		#region Properties

		public Location Location { get { return _location; } }

		#endregion

		#region Methods

		private void Start() 
		{
			_postRollDetails.gameObject.SetActive(false);
			_preRollDisplay.gameObject.SetActive(true);
			_costToRollContainer.SetActive(false);

			EventManager.StartListening(EventName.OnRollCostChanged, HandleOnRollCostChanged);
		}

		private void OnDestroy()
		{
			EventManager.StopListening(EventName.OnRollCostChanged, HandleOnRollCostChanged);
		}

		public void CheckCanBuy(int cost)
		{
			bool canBuy = GameManager.Instance.Gold > cost;
			_selectButton.interactable = canBuy;

			_errorMessage.SetActive(!canBuy);
		}

		public void HandleClickRoll() {
			Debug.Log("Clicked roll build item");

			_postRollDetails.SetData(_location);

			_postRollDetails.gameObject.SetActive(true);
			_preRollDisplay.gameObject.SetActive(false);

			EventManager.TriggerEvent(EventName.OnBuildOptionRolled, null);
			EventManager.StopListening(EventName.OnRollCostChanged, HandleOnRollCostChanged);
		}

		public void SetFutureLocation(List<Location> available)
		{
			_location = Utils.GetRandomFromList(available);
		}

		public void OnPointerEnter(PointerEventData eventData)
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        foreach (GameObject obj in _hoverStateObjs)
				{
					obj.SetActive(true);
				}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        foreach (GameObject obj in _hoverStateObjs)
				{
					obj.SetActive(false);
				}
    }

		/// <summary>
		/// Handles OnRollCostChanged event
		/// Display cost to roll
		/// </summary>
		/// <param name="data"></param>
		private void HandleOnRollCostChanged(Dictionary<string, object> data)
		{
			if (data.TryGetValue("cost", out object costObj))
			{
				_costToRollText.text = costObj.ToString();
				_costToRollContainer.SetActive((int) costObj > 0);
				
				CheckCanBuy((int) costObj);
			}
		}

		#endregion
}
