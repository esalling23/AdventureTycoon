using System.Collections;

public class Data
{
	public Location[] locations;
	public LocationTypeData[] locationTypeDefaults;
	public Activity[] activities;
	public ActivityTypeData[] activityTypeDefaults;
	public VIP[] vips;
	public Quest[] quests;

	public Data(
		Location[] locs, 
		LocationTypeData[] locTypeDefaults,
		Activity[] acts,
		ActivityTypeData[] actTypeDefaults,
		VIP[] vips,
		Quest[] quests
	)
	{
		locations = locs;
		locationTypeDefaults = locTypeDefaults;
		activities = acts;
		activityTypeDefaults = actTypeDefaults;
		this.vips = vips;
		this.quests = quests;
	}
}