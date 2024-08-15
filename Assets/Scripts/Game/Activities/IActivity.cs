
public interface IActivity 
{
	public string Name { get; }
	public string Description { get; }
	public ActivityType Type { get; }
	public int Capacity { get; }
	public int CostToUse { get; }
}