using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GridCell 
{
    #region Fields

		private bool _isSelected;

		private Vector2Int _coordinates;

		// private List<MapObject> _objects;
		private MapLocation _location;
		private TerrainType _terrainType = TerrainType.Jungle;

		#endregion

		#region Properties

		public bool IsSelected { get { return _isSelected; } }
		public Vector2Int Coordinates { get { return _coordinates; } }
		public MapLocation Location { get { return _location; } }

		public TerrainType TerrainType { get { return _terrainType; } }

		#endregion

		#region Methods

    public GridCell(Vector2Int coordinates, TerrainTileSprite[] availableTerrains) {
			_isSelected = false;
			_coordinates = coordinates;

			// To do - sensical terrain generation
			TerrainTileSprite sprite = Utils.GetRandomFromList(availableTerrains.ToList());
			_terrainType = sprite.type;

			EventManager.StartListening(EventName.OnGridValueChanged, HandleGridValueChanged);
		}

		public void HandleGridValueChanged(Dictionary<string, object> data) {
			if (data.TryGetValue("coords", out object coords)) {
				Vector2Int vectCoords = (Vector2Int) coords;
				_isSelected = vectCoords.x == Coordinates.x && vectCoords.y == Coordinates.y;
			}
		}

		public void SetValue(bool isSelected) {
			_isSelected = isSelected;
		}

		public void PlaceLocation(MapLocation location) {
			_location = location;
			location.coordinates = Coordinates;
			EventManager.TriggerEvent(EventName.OnGridValueChanged, new Dictionary<string, object> {
				{ "coords", Coordinates },
			});
		}

		public void RemoveLocation() {
			EventManager.TriggerEvent(EventName.OnGridValueChanged, new Dictionary<string, object> {
				{ "coords", Coordinates },
				{ "locationRemoved", _location.Id }
			});
			_location = null;
		}

		public override string ToString() {
			if (_location != null) {
				return _location.name;
			}
			return Coordinates.x + ", " + Coordinates.y;
		}

		#endregion
}
