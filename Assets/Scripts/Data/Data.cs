using System.Collections;

public class Data
{
	public Location[] locations;
	public LocationTypeData[] locationTypeDefaults;
	public Activity[] activities;
	public ActivityTypeData[] activityTypeDefaults;

	public Data(
		Location[] locs, 
		LocationTypeData[] locTypeDefaults,
		Activity[] acts,
		ActivityTypeData[] actTypeDefaults
	)
	{
		locations = locs;
		locationTypeDefaults = locTypeDefaults;
		activities = acts;
		activityTypeDefaults = actTypeDefaults;
	}
}