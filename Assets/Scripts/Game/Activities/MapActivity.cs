using System.Linq;
using System.Collections.Generic;

// An activity that is in a location on the map RIGHT NOW
public class MapActivity : UniqueObject
{
	public IActivity data;
	public MapLocation locationParent;
	public List<Adventurer> adventurersPresent = new List<Adventurer>();

	private Dictionary<System.Guid, AttemptLog> _attemptLog = new Dictionary<System.Guid, AttemptLog>();

	public ActivityType Type { get { return data.Type; } }

	public Dictionary<System.Guid, AttemptLog> AttemptLog { get { return _attemptLog; } }

	public override string ToString()
	{
		return "Map Activity ID " + Id.ToString();
	}

	public void LogAttempt(System.Guid adventurerId, bool isSuccess)
	{
		if (_attemptLog.TryGetValue(adventurerId, out AttemptLog log)) {
			log.LogAttempt(isSuccess);
		}
		else 
		{
			_attemptLog.Add(adventurerId, new AttemptLog(adventurerId, isSuccess));
		}
	}

	public void Reroll()
	{
		data = locationParent.GetNewRandomActivity(data.Type);
		_attemptLog = new();
		KickOutAll();
	}
	public void RemoveSelf()
	{
		locationParent.RemoveActivity(Id);
		KickOutAll();
	}

	public void KickOutAll()
	{
		while (adventurersPresent.Count > 0)
		{
			adventurersPresent.First().KickOut();
		}
		
		adventurersPresent = new();
	}

	public MapActivity(IActivity activity, MapLocation mapLocation) 
	{
		data = activity;
		locationParent = mapLocation;
		adventurersPresent = new List<Adventurer>();
	}
}
