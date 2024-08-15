using System.Collections.Generic;

public class HistoryLog 
{
	public MapLocation location;
	public Dictionary<System.Guid, int> activityAttemptCount = new Dictionary<System.Guid, int>();

	public System.DateTime timeLastVisit;
	// other info - timestamps?

	public HistoryLog(MapLocation loc)
	{
		location = loc;
		LogVisitLocation();
	}

	public void LogVisitLocation()
	{
		timeLastVisit = System.DateTime.Now;
	}

	public void LogAttemptActivity(MapActivity activity)
	{
		LogVisitLocation();

		if (activityAttemptCount.TryGetValue(activity.Id, out int count)) {
			activityAttemptCount[activity.Id]++;
		} else {
			activityAttemptCount.Add(activity.Id, 1);
		}
	}
}