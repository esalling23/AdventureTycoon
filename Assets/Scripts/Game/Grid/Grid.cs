// using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid<TGridObject>
{
    #region Fields

		private int width;
		private int height;
		private float cellSize;
		private Vector3 originPosition;
	
		private TGridObject[,] gridArray;
		private TextMesh[,] debugTextArray;

		#endregion

		#region Properties

		// public string Property { get; set; }

		#endregion

		#region Methods

    public Grid (
			int width, int height, 
			float cellSize, 
			Vector3 originPosition, 
			System.Func<Grid<TGridObject>, int, int, TGridObject> createGridObject
		) {
			this.width = width;
			this.height = height;
			this.cellSize = cellSize;
			this.originPosition = originPosition;

			gridArray = new TGridObject[width, height];
			debugTextArray = new TextMesh[width, height];

			for (int x = 0; x < gridArray.GetLength(0); x++) {
				for (int y = 0; y < gridArray.GetLength(1); y++) {
					gridArray[x, y] = createGridObject(this, x, y);
					debugTextArray[x, y] = UtilsClass.CreateWorldText(
						gridArray[x, y]?.ToString(), 
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
				{ "coords", (object) coords }
			});
		}

		public Vector3 GetCenteredCellPosition(int x, int y) {
			return GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
		}

		public Vector3 GetWorldPosition(int x, int y) {
			return new Vector3(x, y) * cellSize + originPosition;
		}

		private void GetXY(Vector3 worldPosition, out int x, out int y) {
			x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
			y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
		}

		public Vector2Int GetRandomCoords() {
			int x = Random.Range(0, width);
			int y = Random.Range(0, height);

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
			if (x >= 0 && y >= 0 && x < width && y < height) {
				gridArray[x, y] = value;
				debugTextArray[x, y].text = gridArray[x, y].ToString();
			}
		}

		public void SetGridObject(Vector3 worldPosition, TGridObject value) {
			int x, y;
			GetXY(worldPosition, out x, out y);
			SetGridObject(x, y, value);
		}

		public TGridObject GetGridObject(int x, int y) {
			if (x >= 0 && y >= 0 && x < width && y < height) {
				return gridArray[x, y];
			} else {
				return default(TGridObject);
			}
		}

		public TGridObject GetGridObject(Vector3 worldPosition) {
			int x, y;
			GetXY(worldPosition, out x, out y);
			return GetGridObject(x, y);
		}

		#endregion
}
