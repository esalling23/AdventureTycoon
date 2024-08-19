
public interface IActivity 
{
	public System.Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public ActivityType Type { get; }
	public int Capacity { get; }
	public int CostToUse { get; }
}