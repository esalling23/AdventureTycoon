using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStatsHUD : MonoBehaviour
{
    #region Fields

		// [SerializeField] private Map _map;
		// [SerializeField] private GameManager _manager;

		[SerializeField] private TMP_Text totalAdventurersText;
		[SerializeField] private TMP_Text averageHappinessText;
		[SerializeField] private TMP_Text currentDayText;
		[SerializeField] private TMP_Text playerGoldText;

		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			HandleAdventurerStatChanged();
			HandlePlayerGoldChanged();
			HandleDayChanged();

			EventManager.StartListening(EventName.OnPlayerGoldChanged, HandlePlayerGoldChanged);
			EventManager.StartListening(EventName.OnAdventurerStatChanged, HandleAdventurerStatChanged);
			EventManager.StartListening(EventName.OnDayChanged, HandleDayChanged);
    }

		void OnDestroy()
    {

			EventManager.StopListening(EventName.OnPlayerGoldChanged, HandlePlayerGoldChanged);
			EventManager.StopListening(EventName.OnAdventurerStatChanged, HandleAdventurerStatChanged);
			EventManager.StopListening(EventName.OnDayChanged, HandleDayChanged);
    }

		void HandleAdventurerStatChanged(Dictionary<string, object> _data = null) 
		{
			// Debug.Log($"Setting HUD adventurer data: Happiness {AdventurerManager.Instance.AverageHappiness} and Total {AdventurerManager.Instance.TotalAdventurers}");
			totalAdventurersText.text = AdventurerManager.Instance.TotalAdventurers.ToString();
			averageHappinessText.text = AdventurerManager.Instance.AverageHappiness.ToString();
		}

		void HandlePlayerGoldChanged(Dictionary<string, object> _data = null) 
		{
			// Debug.Log($"Setting HUD gold: {GameManager.Instance.Gold}");
			playerGoldText.text = GameManager.Instance.Gold.ToString();
		}

		void HandleDayChanged(Dictionary<string, object> _data = null) 
		{
			// Debug.Log($"Setting HUD Day: {GameManager.Instance.CurrentDay}");
			currentDayText.text = GameManager.Instance.CurrentDay.ToString();
		}

		#endregion
}
