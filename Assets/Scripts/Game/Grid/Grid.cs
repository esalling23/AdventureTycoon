using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;


public class Grid<TGridObject>
{
    #region Fields

		private int _width;
		private int _height;
		private float _cellSize;
		private Vector3 _originPosition;
	
		private TGridObject[,] _gridArray;
		private TextMesh[,] _debugTextArray;

		#endregion

		#region Properties

		public TGridObject[,] GridArray { get { return _gridArray; } }

		public int TotalCellCount { 
			get { 
				if (_gridArray == null) {
					return 0;
				}
				return _gridArray.Length; 
			} 
		}

		public float CellSize { get { return _cellSize; } }

		#endregion

		#region Methods

    public Grid (
			int width, int height, 
			float cellSize, 
			Vector3 originPosition, 
			System.Func<Grid<TGridObject>, int, int, TGridObject> createGridObject
		) {
			this._width = width;
			this._height = height;
			this._cellSize = cellSize;
			this._originPosition = originPosition;

			_gridArray = new TGridObject[width, height];
			_debugTextArray = new TextMesh[width, height];

			for (int x = 0; x < _gridArray.GetLength(0); x++) {
				for (int y = 0; y < _gridArray.GetLength(1); y++) {
					_gridArray[x, y] = createGridObject(this, x, y);
					_debugTextArray[x, y] = UtilsClass.CreateWorldText(
						_gridArray[x, y]?.ToString(), 
						null, 
						GetCenteredCellPosition(x, y),
						40, // font size
						Color.white,
						TextAnchor.MiddleCenter
					);
					// Each box gets a line beneath & a line to the left
					Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
					Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
				}
			}

			// Remaining lines fill the top & right (don't need these per box)
			Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
			Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
		
		}

		public void TriggerOnChangeEvent(Vector2Int coords) {
			EventManager.TriggerEvent(EventName.OnGridValueChanged, new Dictionary<string, object>() {
				{ "coords", (object) coords },
			});
		}

		public Vector3 GetCenteredCellPosition(int x, int y) {
			return GetWorldPosition(x, y) + new Vector3(_cellSize, _cellSize) * 0.5f;
		}

		public Vector3 GetWorldPosition(int x, int y) {
			return new Vector3(x, y) * _cellSize + _originPosition;
		}

		private void GetXY(Vector3 worldPosition, out int x, out int y) {
			x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
			y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
		}

		public Vector2Int GetRandomCoords() {
			int x = Random.Range(0, _width);
			int y = Random.Range(0, _height);

			return new Vector2Int(x, y);
		}

		public TGridObject GetRandomGridObject() {
			Vector2Int randCoords = GetRandomCoords();
			
			return GetGridObject(randCoords.x, randCoords.y);
		}

		// public void SetRandomGridObject(TGridObject value) {
		// 	Vector2Int randCoords = GetRandomCoords();
			
		// 	while (gridArray[randCoords.x, randCoords.y] != null) {
		// 		randCoords = GetRandomCoords();
		// 	}

		// 	SetGridObject(randCoords.x, randCoords.y, value);
		// }

		public void SetGridObject(int x, int y, TGridObject value) {
			if (x >= 0 && y >= 0 && x < _width && y < _height) {
				_gridArray[x, y] = value;
				_debugTextArray[x, y].text = _gridArray[x, y].ToString();
			}
		}

		public void SetGridObject(Vector3 worldPosition, TGridObject value) {
			int x, y;
			GetXY(worldPosition, out x, out y);
			SetGridObject(x, y, value);
		}

		public TGridObject GetGridObject(int x, int y) {
			if (x >= 0 && y >= 0 && x < _width && y < _height) {
				return _gridArray[x, y];
			} else {
				return default(TGridObject);
			}
		}

		public TGridObject GetGridObject(Vector3 worldPosition) {
			int x, y;
			GetXY(worldPosition, out x, out y);
			return GetGridObject(x, y);
		}

		public TGridObject FindFirstMatch(System.Func<TGridObject, bool> matchCondition) {
			TGridObject match = default(TGridObject);
			for (int x = 1; x < _gridArray.GetLength(0); x++) {
				for (int y = 1; y < _gridArray.GetLength(1); y++) {
					TGridObject obj = _gridArray[x, y];
					bool isMatch = matchCondition(obj);
					if (matchCondition(obj)) {
						match = obj;
						break;
					}
				}
			}

			return match;
		}

		#endregion
}
