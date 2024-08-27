using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class VIPDisplay : MonoBehaviour 
{
	private Map _map;
	private VIP _vip;
	public MapLocationDetailItem locationDetailPrefab;
	public TMP_Text vipNameText;
	public TMP_Text questCountText;
	public TMP_Text questLevelsText;
	public TMP_Text appearsInText;
	public Image image;

	public GameObject locationsShelf;
	public GameObject locationsSelectModal;

	void Start()
	{
		gameObject.SetActive(false);
		locationsSelectModal.SetActive(false);
		_map = GameObject.FindWithTag("Map").GetComponent<Map>();
	}

	public void ShowVIP(VIP vip)
	{
		_vip = vip;

		vipNameText.text = vip.name;
		questCountText.text = "Quests: " + vip.quests.Count.ToString();
		questLevelsText.text = $"Levels: {vip.QuestLevelMin} - {vip.QuestLevelMax}";
		appearsInText.text = $"Can be placed in {GeneratePlaceSentence(vip.appearsInLocations.ToList())}.";
		image.sprite = vip.headshot;

		gameObject.SetActive(true);
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	private string GetTypeName(LocationType type)
	{
		return System.Enum.GetName(typeof(LocationType), type).ToLower();
	}

	private string GeneratePlaceSentence(List<LocationType> locations)
	{
		if (locations.Count == 0) return "anywhere";
		if (locations.Count == 1) {
			return $"in {GetTypeName(locations[0])} locations";
		}
		
		string allButLast = string.Join(", ", locations.Take(locations.Count - 1).Select(location => GetTypeName(location)));
		string last = GetTypeName(locations.Last());
		return $"in {allButLast} and {last} locations";
	}

	public void HandleClickCancelAddVIP()
	{
		locationsSelectModal.SetActive(false);
	}
	public void HandleClickRejectVIP()
	{
		_vip = null;
		VIPManager.Instance.RejectVIP();
		gameObject.SetActive(false);
	}

	public void HandleClickAddVIP()
	{
		if (_vip == null) return;

		List<MapLocation> availableLocations = _map.LocationsOnMap.Where(loc => _vip.appearsInLocations.Contains(loc.Type)).ToList();
		
		foreach (Transform child in locationsShelf.transform)
		{
			Destroy(child.gameObject);
		}

		// VIPs that have no "appears in" locations should be able to go anywhere
		if (availableLocations.Count == 0) availableLocations = _map.LocationsOnMap.ToList();

		foreach (MapLocation location in availableLocations)
		{
			MapLocationDetailItem item = Instantiate(
				locationDetailPrefab,
				Vector3.zero,
				Quaternion.identity,
				locationsShelf.transform
			);

			item.SetData(location);
		}

		locationsSelectModal.SetActive(true);
	}
}