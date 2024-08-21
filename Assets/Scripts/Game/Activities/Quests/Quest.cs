using System.Collections.Generic;
using System; 

[Serializable]
public class Quest : ActivityBase
{
	#region Fields

	// 	prereqs: list of activity (quest) Ids that are required before adventurer can attempt them
	public List<Guid> preReqIds = new List<Guid>();
	// level: int representing min level an adventurer must be to attempt them
	public int minLevel = 1;
	public int maxLevel = 5;
	// reward: int representing gold adventurers earn from success
	public int reward = 100;
	// items: List of items ?
	#endregion

	#region Properties

	

	#endregion
}