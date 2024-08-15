using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class AdventurerManager : MonoBehaviour
{
    #region Fields

		private static AdventurerManager _instance;

		public int initHealthRoll = 4;
		public int minGroupSize = 5;
		public int maxGroupSize = 10;
		public int maxHappiness = 100;
		private float _xpIncreaseRate = 0.3f;

		private List<Adventurer> _adventurersOnMap = new List<Adventurer>();

		[SerializeField] private Adventurer _adventurerPrefab;

		#endregion

		#region Properties

		public static AdventurerManager Instance { get { return _instance; }}

		#endregion

		#region Methods

		/// <summary>
    /// Manages singleton wakeup/destruction
    /// </summary>
    private void Awake()
    {
        // Singleton management
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

		public void GenerateGroup() {
			int count = Random.Range(minGroupSize, maxGroupSize);

			for (int i = 0; i < count; i++) {
				CreateAdventurer();
			}
		}

		public void CreateAdventurer() 
		{
			Adventurer newAdventurer = Instantiate(
				_adventurerPrefab,
				Vector3.zero,
				Quaternion.identity,
				gameObject.transform
			).GetComponent<Adventurer>();
			newAdventurer.Init();
			newAdventurer.Loop();
		}

		public int GetAdventurerLevelXp(int level = 1)
		{
			return Mathf.FloorToInt(100 * Mathf.Pow(1 + _xpIncreaseRate, level));
		}

		#endregion
}
