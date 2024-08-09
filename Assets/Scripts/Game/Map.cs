// using System.Numerics;
// using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour
{
    #region Fields

		[SerializeField] private GameManager _manager;

		// Grid
		public Vector2Int size;
		public float cellSize = 10f;
		private Grid<GridCell> _mapGrid;

		// Locations
		[SerializeField] private MapLocation[] _locationPrefabs;

		// UI
		[SerializeField] private GameObject _selectedIndicator;
		[SerializeField] private LocationDetailsPanel _locationDetailsPanel;

		#endregion

		#region Properties


		#endregion

		#region Methods

    void Start()
    {
      _mapGrid = new Grid<GridCell>(
				(int) size.x, 
				(int) size.y, 
				cellSize, 
				new Vector2(-size.x * 5, -size.y * 5),
				(Grid<GridCell> g, int x, int y) => new GridCell(g, new Vector2Int(x, y))
			);

			InitMapLocations(_locationPrefabs, 3);
    }

    void Update()
    {
			if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) {
				GridCell cell = _mapGrid.GetGridObject(UtilsClass.GetMouseWorldPosition());
				Debug.Log($"Clicked on cell coordinates ({cell.Coordinates.x}, {cell.Coordinates.y})");
				switch(_manager.Mode) {
					case GameMode.Build:
						PlaceLocation(cell, GetRandomLocationPrefab(_locationPrefabs));
						break;
					default: 
						break;
				}
				SelectCell(cell);
			}

			if (Input.GetMouseButtonDown(1)) {
				Deselect();
			}
    }

		private void Deselect() {
			_selectedIndicator.SetActive(false);
			_locationDetailsPanel.gameObject.SetActive(false);
		}

		private void SelectCell(GridCell cell) {
			_selectedIndicator.transform.position = _mapGrid.GetCenteredCellPosition(cell.Coordinates.x, cell.Coordinates.y);
			_selectedIndicator.SetActive(true);

			if (cell.Location) {
				_locationDetailsPanel.SetLocationData(cell.Location);
				_locationDetailsPanel.gameObject.SetActive(true);
			} else {
				_locationDetailsPanel.gameObject.SetActive(false);
			}
		}

		public void PlaceLocation(GridCell cell, MapLocation locationPrefab) {
			// only place if it's a null position
			if (!cell?.Location) {
				// Create the actual location object to display (visual layer)
				MapLocation newPlacement = CreateLocationObject(locationPrefab, cell);
				newPlacement.SetSpriteSize((float) cellSize / 2, (float) cellSize / 2);
				// Keep track of the placed object in the grid (data later)
				cell.PlaceLocation(newPlacement);
				// Todo: Trigger grid update event
				_mapGrid.TriggerOnChangeEvent(cell.Coordinates);
			}
		}

		public void Regenerate() {
			// InitMapLocations();
		}

		private void GenerateMapTerrain() {
			// generate map background/terrain
		}

		private MapLocation GetRandomLocationPrefab(MapLocation[] available) {
			int rand = Random.Range(0, available.Length);
			return available[rand];
		}

		private void InitMapLocations(MapLocation[] available, int count) {
			for (int i = 0; i < count; i++) {	
				// find empty cell			
				GridCell cell;
				do {
					cell = _mapGrid.GetRandomGridObject();
				} while (cell?.Location != null);

				// init locations are random
				MapLocation randLocation = GetRandomLocationPrefab(available);
				PlaceLocation(cell, randLocation);
			}
		}

		private MapLocation CreateLocationObject(MapLocation locationPrefab, GridCell cell) {
			return Instantiate(
				locationPrefab,
				_mapGrid.GetCenteredCellPosition(cell.Coordinates.x, cell.Coordinates.y),
				Quaternion.identity, 
				this.transform
			);
		}

		#endregion
}
