using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Any location that could be placed on the map (town, city, quest spot, etc.)
public class MapLocation : MapObject
{
    #region Fields

		private System.Guid _id = System.Guid.NewGuid();

		[SerializeField] private GameManager _manager;

		public LocationType type = LocationType.Building;

		public List<LocationActivity> activities;

		// To do - load data from somewhere
		public string name = "Map Location";
		public string description;

		public int costToPlace = 0;
		public int amenitySlotCount;

		[Header("Optional custom icon. Will default to type icon.")]
		public Sprite icon;
		private SpriteRenderer _spriteRenderer;

		#endregion

		#region Properties

		public System.Guid Id { get { return _id; } }

		#endregion

		#region Methods

    void Start()
    {
			_manager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
			LocationTypeData defaultData = _manager.GetLocationTypeData(type);

			_spriteRenderer = GetComponent<SpriteRenderer>();
			if (icon) {
				_spriteRenderer.sprite = icon;
			} else {
				_spriteRenderer.sprite = defaultData.icon;
			}

			amenitySlotCount = defaultData.baseAmenitySlots;
    }

		public void SetSpriteSize(float width, float height) {
			if (!_spriteRenderer) {
				_spriteRenderer = GetComponent<SpriteRenderer>();
				_spriteRenderer.sprite = icon;
			}
			Debug.Log(_spriteRenderer.size);
			_spriteRenderer.size = new Vector3(width, height, 0f);
			Debug.Log(_spriteRenderer.size);
		}

		public LocationActivity AddRandomActivity() {
			Debug.Log("Adding Map Location Amenity");
			LocationActivity newActivity = new LocationActivity();

			activities.Add(newActivity);

			EventManager.TriggerEvent(EventName.OnActivityChanged, null);

			return newActivity;
		}

		public void RemoveActivity(LocationActivity activity) {
			Debug.Log("Removing Map Location Amenity");
			activities = activities.Where(act => act.name != activity.name).ToList();

			EventManager.TriggerEvent(EventName.OnActivityChanged, null);
		}

		#endregion
}
