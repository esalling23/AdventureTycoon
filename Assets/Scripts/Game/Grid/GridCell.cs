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
		private TerrainType _terrainType = TerrainType.Jungle;

		#endregion

		#region Properties

		public bool IsSelected { get { return _isSelected; } }
		public Vector2Int Coordinates { get { return _coordinates; } }
		public MapLocation Location { get { return _location; } }

		public TerrainType TerrainType { get { return _terrainType; } }

		#endregion

		#region Methods

    public GridCell(Grid<GridCell> grid, Vector2Int coordinates) {
			this._grid = grid;
			this._isSelected = false;
			this._coordinates = coordinates;

			// To do - sensical terrain generation
			System.Array terrainValues = System.Enum.GetValues(typeof(TerrainType));
			int random = Mathf.FloorToInt(Random.Range(0, terrainValues.Length));
			TerrainType randomType = (TerrainType) terrainValues.GetValue(random);
			this._terrainType = randomType;

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

		public override string ToString() {
			if (_location) {
				return _location.name;
			}
			return _coordinates.x + ", " + _coordinates.y;
		}

		#endregion
}
