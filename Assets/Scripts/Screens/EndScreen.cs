using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    #region Fields

		public TMP_Text _subTitle;
		public TMP_Text _adventurerStat;
		public TMP_Text _daysStat;
		// public TMP_Text _locationStat;
		// public TMP_Text _activityStat;
		// public TMP_Text _questStat;

		#endregion

		#region Properties

		#endregion

		#region Methods

		void Start()
		{
			GameManager gm = GameManager.Instance;
			if (gm.IsForceQuit)
			{
				// show a different for force quit?
				_subTitle.gameObject.SetActive(false);
			}
			SetStats(
				gm.HighestPopulation,
				gm.CurrentDay
				// gm.TotalLocations,
				// gm.TotalActivities,
				// gm.TotalQuests
			);
		}

    void SetStats(int adventurerCount, int dayCount)
    {
      _adventurerStat.text = $"Highest Population: {adventurerCount}";
      _daysStat.text = $"Total Days: {dayCount}";
      // _locationStat.text = $"Total Locations: {locationCount}";
      // _activityStat.text = $"Total Activities: {activityCount}";
      // _questStat.text = $"Total Quests: {questCount}";
    }

    public void Continue()
    {
			SceneManager.LoadScene("StartScreen");
    }

		#endregion
}
