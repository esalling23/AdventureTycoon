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
	private CameraManager _camera;
	public float cameraMoveSpeed = 50;

	// Grid
	public Vector2Int gridSize;
	public float cellSize = 10f;
	private Grid<GridCell> _mapGrid;

	// Locations
	[SerializeField] private MapLocation _locationPrefab;
	private Location _activeLocationToBuild;

	// Adventurers
	[SerializeField] private Adventurer _adventurerPrefab;
	[SerializeField] private GameObject _adventurerContainer;

	// UI
	[SerializeField] private GameObject _selectedIndicator;
	[SerializeField] private LocationDetailsPanel _locationDetailsPanel;

	#endregion

	#region Properties


	#endregion

	#region Methods

	void Start()
	{
		_camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraManager>();

		_mapGrid = new Grid<GridCell>(
			(int) gridSize.x, 
			(int) gridSize.y, 
			cellSize, 
			new Vector2(-gridSize.x * 5, -gridSize.y * 5),
			(Grid<GridCell> g, int x, int y) => new GridCell(g, new Vector2Int(x, y))
		);

		_activeLocationToBuild = null;

		InitMapLocations(DataManager.Instance.WorldLocations, 3);
		InitMapAdventurers(2);
	}

	void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) {
			GridCell cell = _mapGrid.GetGridObject(UtilsClass.GetMouseWorldPosition());
			Debug.Log($"Clicked on cell coordinates ({cell.Coordinates.x}, {cell.Coordinates.y})");
			switch(_manager.Mode) {
				case GameMode.Build:
					if (_activeLocationToBuild != null) {
						PlaceLocation(cell, _activeLocationToBuild);
					}
					else 
					{
						Debug.Log("No Location Selected For Building");
					}
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

	public MapLocation FindLocationWithActivityType(ActivityType type) 
	{
		return new MapLocation();
		// return WorldLocations.First();
	}

	private void Deselect() {
		_selectedIndicator.SetActive(false);
		_locationDetailsPanel.gameObject.SetActive(false);
	}

	public void SetLocationToBuild(Location location) 
	{
		_manager.SetMode(GameMode.Build);
		_activeLocationToBuild = location;
	}
	
	private void SelectCell(GridCell cell) {
		Vector3 centeredCellPos = _mapGrid.GetCenteredCellPosition(cell.Coordinates.x, cell.Coordinates.y);
		
		_selectedIndicator.transform.position = centeredCellPos;
		_selectedIndicator.SetActive(true);

		//todo - move to it's own camera movement script!
		_camera.AnimateTo(centeredCellPos, 0.5f);
		
		if (cell.Location) {
			_locationDetailsPanel.SetLocationData(cell.Location);
			_locationDetailsPanel.gameObject.SetActive(true);
		} else {
			_locationDetailsPanel.gameObject.SetActive(false);
		}
	}

	public void PlaceLocation(GridCell cell, Location locationData) {
		// only place if it's a null position
		if (!cell?.Location) {
			// Create the actual location object to display (visual layer)
			MapLocation newPlacement = CreateLocationObject(locationData, cell);
			newPlacement.SetSpriteSize((float) cellSize / 2, (float) cellSize / 2);
			// Keep track of the placed object in the grid (data later)
			cell.PlaceLocation(newPlacement);
			// Todo: Trigger grid update event
			_mapGrid.TriggerOnChangeEvent(cell.Coordinates);

			_activeLocationToBuild = null;
			_manager.SetMode(GameMode.Run);
		}
	}

	public void Regenerate() {
		// InitMapLocations();
	}

	private void GenerateMapTerrain() {
		// generate map background/terrain
	}

	public Location GetRandomLocationData(Location[] available) {
		int rand = Random.Range(0, available.Length);
		return available[rand];
	}

	private void InitMapAdventurers(int count) {
		for (int i = 0; i < count; i++) {
			Adventurer newAdventurer = Instantiate(
				_adventurerPrefab,
				Vector3.zero,
				Quaternion.identity,
				_adventurerContainer.transform
			);
			// newAdventurer.
		}
	}

	private void InitMapLocations(Location[] available, int count) {
		int locCount = count;
		if (count > _mapGrid.TotalCellCount) {
			locCount = _mapGrid.TotalCellCount;
		}
		for (int i = 0; i < locCount; i++) {	
			// find empty cell			
			GridCell cell;
			do {
				cell = _mapGrid.GetRandomGridObject();
			} while (cell?.Location != null);

			// init locations are random
			Location randLocation = GetRandomLocationData(available);
			PlaceLocation(cell, randLocation);
		}
	}

	private MapLocation CreateLocationObject(Location locationData, GridCell cell) {
		MapLocation location = Instantiate(
			_locationPrefab,
			_mapGrid.GetCenteredCellPosition(cell.Coordinates.x, cell.Coordinates.y),
			Quaternion.identity, 
			this.transform
		);

		location.SetData(locationData);
		
		return location; 
	}

	#endregion
}
