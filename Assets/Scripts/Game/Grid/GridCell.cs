using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell 
{
    #region Fields

		private Grid<GridCell> _grid;

		private bool _isSelected;

		private Vector2Int _coordinates;

		// private List<MapObject> _objects;
		private MapLocation _location;

		#endregion

		#region Properties

		public bool IsSelected { get { return _isSelected; } }
		public Vector2Int Coordinates { get { return _coordinates; } }
		public MapLocation Location { get { return _location; } }

		#endregion

		#region Methods

    public GridCell(Grid<GridCell> grid, Vector2Int coordinates) {
			this._grid = grid;
			this._isSelected = false;
			this._coordinates = coordinates;
			// this._objects = new List<MapObject>;


			EventManager.StartListening(EventName.OnGridValueChanged, HandleGridValueChanged);

		}

		public void HandleGridValueChanged(Dictionary<string, object> data) {
			if (data.TryGetValue("coords", out object coords)) {
				Vector2Int vectCoords = (Vector2Int) coords;
				_isSelected = vectCoords.x == Coordinates.x && vectCoords.y == Coordinates.y;
			}
		}

		public void SetValue(bool isSelected) {
			this._isSelected = isSelected;
		}

		public void PlaceLocation(MapLocation location) {
			this._location = location;
		}

		private void HandleGridValueChanged() {
			EventManager.TriggerEvent(EventName.OnGridValueChanged, new Dictionary<string, object> {
				{ "coords", Coordinates },
			});
		}

		public override string ToString() {
			if (_location) {
				return _location.name;
			}
			return _coordinates.x + ", " + _coordinates.y;
		}

		#endregion
}
