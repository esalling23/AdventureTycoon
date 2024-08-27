using System.Collections.Generic;

public class ActivityAttemptsLog
{
	public int count = 0;
	public bool hasCompleted = false;
}

public class HistoryLog 
{
	public MapLocation location;

	/// <summary>
	/// Dict of MapActivity Ids with ActivityAttemptsLog objects for that MapActivity
	/// </summary>
	/// <typeparam name="System.Guid">MapActivity Ids</typeparam>
	/// <typeparam name="ActivityAttemptsLog">Log objects for this MapActivity</typeparam>
	public Dictionary<System.Guid, ActivityAttemptsLog> activityAttempts = new();

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

	public void LogAttemptActivity(MapActivity activity, bool isSuccess = false)
	{
		LogVisitLocation();
		activityAttempts.TryAdd(activity.Id, new ActivityAttemptsLog());
		// Only set for success
		if (isSuccess) 
		{
			activityAttempts[activity.Id].hasCompleted = true;
		}
	}
}