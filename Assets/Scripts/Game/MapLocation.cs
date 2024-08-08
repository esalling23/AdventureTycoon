using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Any location that could be placed on the map (town, city, quest spot, etc.)
public class MapLocation : MonoBehaviour
{
    #region Fields

		public LocationType type = LocationType.Building;

		public List<LocationActivity> activities;

		public string name = "Map Location";

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

		#endregion
}
