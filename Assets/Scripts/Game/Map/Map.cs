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

	// UI
	[SerializeField] private GameObject _selectedIndicator;
	[SerializeField] private LocationDetailsPanel _locationDetailsPanel;

	#endregion

	#region Properties

	public List<MapLocation> LocationsOnMap { get { return _locationsOnMap; } }


	#endregion

	#region Methods

	void Start()
	{
		MapTerrainTileSprites();

		_camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraManager>();

		_mapGrid = new Grid<GridCell>(
			(int) gridSize.x, 
			(int) gridSize.y, 
			cellSize, 
			new Vector2(-gridSize.x * 5, -gridSize.y * 5),
			(Grid<GridCell> g, int x, int y) => new GridCell(g, new Vector2Int(x, y))
		);

		ShowTerrainSprites();

		_activeLocationToBuild = null;

		InitMapLocations(DataManager.Instance.WorldLocations, 1);
		InitMapAdventurers(2);

		EventManager.StartListening(EventName.OnGridValueChanged, HandleGridValueChanged);
	}

	void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) {
			GridCell cell = _mapGrid.GetGridObject(UtilsClass.GetMouseWorldPosition());
			if (cell != null)
			{
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
		}

		if (Input.GetMouseButtonDown(1)) {
			Deselect();
		}
	}

	// Builds map for terrain type -> tile sprite
	private void MapTerrainTileSprites()
	{
		foreach (TerrainTileSprite terrain in _terrainTileSprites)
		{
			if (_terrainTileSpritesMap.TryGetValue(terrain.type, out Sprite sprite)) {
				sprite = terrain.sprite;
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
					this.transform
				);
				tile.cell = _mapGrid.GetGridObject(x, y);
				Debug.Log($"Cell for ({x}, {y}) is {tile.cell}");
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

	private void HandleGridValueChanged(Dictionary<string, object> data) {
		if (data.TryGetValue("coords", out object coords)) 
		{
			Vector2Int vectCoords = (Vector2Int) coords;
			GridCell cell = _mapGrid.GetGridObject(vectCoords.x, vectCoords.y);
			if (cell.Location != null)
			{
				_locationsOnMap.Add(cell.Location);
			}
			else if (data.TryGetValue("locationRemoved", out object locationIdRemoved))
			{
				_locationsOnMap.RemoveAll((MapLocation location) => {
					return location.Id == (System.Guid) locationIdRemoved;
				});
			}
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

	public void PlaceLocation(
		GridCell cell, 
		Location locationData, 
		System.Action<MapLocation> runSideEffects = null
	) {
		// only place if it's a null position
		if (!cell?.Location) {
			// Create the actual location map object to display (visual layer)
			MapLocation newPlacement = CreateLocationObject(locationData, cell);
			newPlacement.SetSpriteSize((float) cellSize, (float) cellSize);

			if (runSideEffects != null) 
			{
				runSideEffects(newPlacement);
			}

			// Keep track of the placed object in the grid (data layer)
			cell.PlaceLocation(newPlacement);

			_locationsOnMap.Add(newPlacement);

			_mapGrid.TriggerOnChangeEvent(cell.Coordinates);
		}
		_activeLocationToBuild = null;
		_manager.SetMode(GameMode.Run);
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
			Location randLocation = GetRandomLocationData(available);
			PlaceLocation(cell, randLocation, (MapLocation newLocation) => {
				for (int i = 0; i < 1; i++)
				{
					// Make sure there's at least 1 activity matching 
					// the base activity type for this location type
					newLocation.AddRandomActivity(i == 0);
				}
				newLocation.AddRandomQuest();
			});
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
