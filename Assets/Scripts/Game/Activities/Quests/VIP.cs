using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class VIP
{
    #region Fields

		public string name;
		public string backstory;

		public int lifetime = 365;

		public List<Quest> quests = new();
		public LocationType[] appearsInLocations;

		public Sprite headshot;

		private int _questLevelMin;
		private int _questLevelMax;

		#endregion

		#region Properties

		public int QuestLevelMin { get { 
			if (_questLevelMin == 0)
			{
				SetQuestLevelRange();
			}
			return _questLevelMin; 
		}}
		public int QuestLevelMax { get { 
			if (_questLevelMax == 0)
			{
				SetQuestLevelRange();
			}
			return _questLevelMax; 
		}}


		#endregion

		#region Methods

		private void SetQuestLevelRange()
		{
			if (quests == null || quests.Count == 0)
			{
					_questLevelMin = 0;
					_questLevelMax = 0;
					return;
			}
			_questLevelMin = quests.Min(q => q.minLevel);
			_questLevelMax = quests.Max(q => q.maxLevel);
		}

		#endregion
}
