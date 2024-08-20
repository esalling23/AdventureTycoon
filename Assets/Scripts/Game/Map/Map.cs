// using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using UnityEngine.EventSystems;

[System.Serializable]
public struct TerrainTileSprite
{
	public TerrainType type;
	public Sprite sprite;
}

public class Map : MonoBehaviour
{
	#region Fields

	[SerializeField] private GameManager _manager;
	private CameraManager _camera;

	// Initialization
	[SerializeField] private int initLocationCount = 1;
	[SerializeField] private int initAdventurerCount = 3;
	[SerializeField] private int initActivityCount = 2;
	[SerializeField] private int initQuestCount = 1;

	// Grid
	public Vector2Int gridSize = new Vector2Int(20, 10);
	public float cellSize = 10f;
	private Grid<GridCell> _mapGrid;
	[SerializeField] private MapTile _tilePrefab;
	[SerializeField] private TerrainTileSprite[] _terrainTileSprites;
	private Dictionary<TerrainType, Sprite> _terrainTileSpritesMap = new Dictionary<TerrainType, Sprite>();

	// Locations
	private List<MapLocation> _locationsOnMap = new List<MapLocation>();
	[SerializeField] private MapLocation _locationPrefab;
	private Location _activeLocationToBuild;
	private TempBuildLocation _activeTempLocation;
	[SerializeField] private TempBuildLocation _tempLocationPrefab;

	// UI
	[SerializeField] private GameObject _selectedIndicator;
	[SerializeField] private LocationDetailsPanel _locationDetailsPanel;

	#endregion

	#region Properties

	public List<MapLocation> LocationsOnMap { get { return _locationsOnMap; } }
	public Grid<GridCell> Grid { get { return _mapGrid; } }

	#endregion

	#region Methods

	void Start()
	{
		MapTerrainTileSprites();

		_camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraManager>();

		Init();

		EventManager.StartListening(EventName.OnBuildLocationSelected, HandleBuildLocationSelected);
		EventManager.StartListening(EventName.OnGridValueChanged, HandleGridValueChanged);
	}

	void OnDestroy()
	{
		EventManager.StopListening(EventName.OnBuildLocationSelected, HandleBuildLocationSelected);
		EventManager.StopListening(EventName.OnGridValueChanged, HandleGridValueChanged);
	}

	void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject()) {
			GridCell cell = _mapGrid.GetGridObject(UtilsClass.GetMouseWorldPosition());
			
			if (cell != null)
			{
				if (Input.GetMouseButtonDown(0))
				{
					// Debug.Log($"Clicked on cell coordinates ({cell.Coordinates.x}, {cell.Coordinates.y})");
					switch(_manager.Mode) {
						case GameMode.Build:
							if (_activeLocationToBuild != null && !cell.Location) {
								PlaceLocation(_activeLocationToBuild, cell);
							}
							break;
						default: 
							break;
					}
					SelectCell(cell);
				} 
				else if (_activeLocationToBuild != null)
				{
					if (_activeTempLocation == null)
					{
						_activeTempLocation = CreateTempLocation(_activeLocationToBuild, cell);
					}
					_activeTempLocation.SetPosition(
						_mapGrid.GetCenteredCellPosition(cell.Coordinates.x, cell.Coordinates.y) - new Vector3(0f, cellSize / 2, 0f)
					);
					_activeTempLocation.SetSortingOrder(_mapGrid.GridArray.GetLength(1) - cell.Coordinates.y);
				}	
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			Deselect();
		}
	}

	public void Init() {
		// Debug.Log("Initializing map");

		_mapGrid = new Grid<GridCell>(
			gridSize.x,
			gridSize.y, 
			cellSize, 
			new Vector2(-gridSize.x * 5, -gridSize.y * 5),
			(Grid<GridCell> g, int x, int y) => new GridCell(new Vector2Int(x, y), _terrainTileSprites)
		);

		ShowTerrainSprites();

		InitMapLocations(DataManager.Instance.WorldLocations, initLocationCount);
		InitMapAdventurers(initAdventurerCount);

		_activeLocationToBuild = null;
	}

	// Builds map for terrain type -> tile sprite
	private void MapTerrainTileSprites()
	{
		foreach (TerrainTileSprite terrain in _terrainTileSprites)
		{
			if (_terrainTileSpritesMap.TryGetValue(terrain.type, out Sprite sprite)) {
				_terrainTileSpritesMap[terrain.type] = terrain.sprite;
			}
			else
			{
				_terrainTileSpritesMap.Add(terrain.type, terrain.sprite);
			}
		}
	}

	private void ShowTerrainSprites()
	{

		for (int x = 0; x < _mapGrid.GridArray.GetLength(0); x++) 
		{
			for (int y = 0; y < _mapGrid.GridArray.GetLength(1); y++)
			{
				MapTile tile = Instantiate(
					_tilePrefab,
					Vector3.zero,
					Quaternion.identity,
					transform
				);
				tile.cell = _mapGrid.GetGridObject(x, y);
				// Debug.Log($"Cell for ({x}, {y}) is {tile.cell}");
				if (_terrainTileSpritesMap.TryGetValue(tile.cell.TerrainType, out Sprite tileSprite)) {
					tile.SetSprite(tileSprite);
				}
				else 
				{
					Debug.Log($"Could not find sprite for {tile.cell.TerrainType}");
				}
				tile.transform.position = _mapGrid.GetCenteredCellPosition(x, y) - new Vector3(0f, cellSize / 2, 0f);
				tile.SetSpriteSizeSquare(_mapGrid.CellSize, tile.Renderer.bounds.size.x);
				tile.Renderer.sortingOrder = _mapGrid.GridArray.GetLength(1) - y;
			}
		}
	}

	private void SelectCell(GridCell cell) {
		Vector3 centeredCellPos = _mapGrid.GetCenteredCellPosition(cell.Coordinates.x, cell.Coordinates.y);
		
		_selectedIndicator.transform.position = centeredCellPos;
		_selectedIndicator.SetActive(true);

		_camera.AnimateTo(centeredCellPos, 0.5f);
		
		if (cell.Location) {
			_locationDetailsPanel.SetLocationData(cell.Location);
			_locationDetailsPanel.ToggleOpen(true);
		} else {
			_locationDetailsPanel.ToggleOpen(false);
		}
	}

	private void Deselect() {
		_selectedIndicator.SetActive(false);
		_locationDetailsPanel.ToggleOpen(false);
	}

	public void SetLocationToBuild(Location location) 
	{
		_manager.SetMode(GameMode.Build);
		_activeLocationToBuild = location;
	}
	
	public void PlaceLocation(
		Location locationData, 
		GridCell cell, 
		System.Action<MapLocation> runSideEffects = null
	) {
		// only place if it's a null position
		if (!cell?.Location) {
			if (_activeTempLocation != null)
			{
				Destroy(_activeTempLocation.gameObject);
				_activeTempLocation = null;
			}
			// Create the actual location map object to display (visual layer)
			MapLocation newPlacement = CreateLocationObject(locationData, cell);

			runSideEffects?.Invoke(newPlacement);

			// Keep track of the placed object in the grid (data layer)
			cell.PlaceLocation(newPlacement);

			_locationsOnMap.Add(newPlacement);
		}
		_activeLocationToBuild = null;
		_manager.SetMode(GameMode.Run);
	}

	private TempBuildLocation CreateTempLocation(Location locationData, GridCell cell)
	{
		TempBuildLocation location = Instantiate(
			_tempLocationPrefab,
			GetCellObjectPosition(cell),
			Quaternion.identity, 
			transform
		);

		location.SetData(locationData);
		location.SetSpriteSize(cellSize, cellSize);

		return location;
	}

	public Location GetRandomLocationData(Location[] available) {
		return Utils.GetRandomFromList(available.ToList());
	}

	private Vector3 GetCellObjectPosition(GridCell cell)
	{
		return _mapGrid.GetCenteredCellPosition(cell.Coordinates.x, cell.Coordinates.y) - new Vector3(0f, cellSize / 2, 0f);
	}

	private void InitMapAdventurers(int count) {
		for (int i = 0; i < count; i++) {
			AdventurerManager.Instance.CreateAdventurer();
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
			Location randLocation = Utils.GetRandomFromList(available.ToList());
			PlaceLocation(randLocation, cell, (MapLocation newLocation) => {
				for (int i = 0; i < initActivityCount; i++)
				{
					// Make sure there's at least 1 activity matching 
					// the base activity type for this location type
					newLocation.AddRandomActivity(i == 0);
				}
				for (int i = 0; i < initQuestCount; i++)
				{
					newLocation.AddRandomQuest();
				}
			});
		}
	}

	private MapLocation CreateLocationObject(Location locationData, GridCell cell) {
		MapLocation location = Instantiate(
			_locationPrefab,
			GetCellObjectPosition(cell),
			Quaternion.identity, 
			transform
		);

		location.SetData(locationData);
		
		return location; 
	}

	#region EventHandlers

	private void HandleBuildLocationSelected(Dictionary<string, object> msg)
	{
		if (msg.TryGetValue("location", out object locationToBuild))
		{
			SetLocationToBuild((Location) locationToBuild);
			Deselect();
		}
	}

	private void HandleGridValueChanged(Dictionary<string, object> data) {
		if (data.TryGetValue("locationRemoved", out object locationIdRemoved))
		{
			if (data.TryGetValue("coords", out object coords)) 
			{
				_locationsOnMap.RemoveAll((MapLocation location) => {
					return location.Id == (System.Guid) locationIdRemoved;
				});
			}
		}
	}

	#endregion

	#endregion
}
