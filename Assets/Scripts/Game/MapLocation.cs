using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Any location that could be placed on the map (town, city, quest spot, etc.)
public class MapLocation : MapObject
{
    #region Fields

		public LocationType type = LocationType.Building;

		public List<LocationActivity> activities;

		public string name = "Map Location";
		public string description;

		public Sprite icon;
		private SpriteRenderer _spriteRenderer;

		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			_spriteRenderer = GetComponent<SpriteRenderer>();
      _spriteRenderer.sprite = icon;
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

		#endregion
}
