using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Quest : ActivityBase
{
	#region Fields

	/// <summary>
	/// List of Quest IActivity IDs required before this quest is available to be done by Adventurers
	/// </summary>
	public string[] preReqIds;
	/// <summary>
	/// Minimum Adventurer level required to be available
	/// </summary>
	public int minLevel = 1;
	/// <summary>
	/// Caps the level Adventurers can be to attempt this quest
	/// </summary>
	public int maxLevel = 5;

	/// <summary>
	/// Median experience gained from Adventurer successful attempt. 
	/// Actual experience calculated using this value, the minLevel & maxLevel, and the Adventurer Level
	/// </summary>
	public int baseExperience = 100;

	public int reward = 100;

	/// <summary>
	/// Minimum success probability used for determining success for Adventurers whose level matches the minLevel for this Quest
	/// </summary>
	private float _minSuccessProbability = 0.25f;
	/// <summary>
	/// Minimum success probability used for determining success for Adventurers whose level matches the maxLevel for this Quest
	/// </summary>
	private float _maxSuccessProbability = 0.99f;

	#endregion

	#region Methods

	/// <summary>
	/// Calculates probability of succeeding quest
	/// </summary>
	/// <param name="minLevel">Quest min Level</param>
	/// <param name="maxLevel">Quest max Level</param>
	/// <returns></returns>
	public float CalculateSuccessProbability(int currLevel)
	{
		float midpoint = (minLevel + maxLevel) / 2f;
		float k = 0.2f;
		float probability = 1f / (1f + Mathf.Exp(-k * (currLevel - midpoint)));

		// Probability should range from 25% - 99%
		if (currLevel <= minLevel)
		{
			return _minSuccessProbability;
		}
		else if (currLevel >= maxLevel)
		{
			return _maxSuccessProbability;
		}
		else
		{
			float scaledProbability = _minSuccessProbability + (0.74f * probability);
			return scaledProbability;
		}
	}

	private int CalculateValue(float probability, int baseValue)
	{
		// 2x for lower probability or 0.5x for higher probability
		float multiplier = Mathf.Lerp(2.0f, 0.5f, probability);
		int change = Mathf.RoundToInt(baseValue * multiplier);

		return change;
	}

	public int CalculateExperienceGained(float probability)
	{
		return CalculateValue(probability, baseExperience);
	}

	public int CalculateHealthChange(float probability)
	{
		return CalculateValue(probability, healthEffect);
	}
	public int CalculateHappinessChange(float probability)
	{
		return CalculateValue(probability, happinessEffect);
	}

	public override bool Attempt(Adventurer adventurer)
	{
		float probability = CalculateSuccessProbability(adventurer.Level);
		float rand = Random.Range(0f, 1f);
		// Debug.Log($"Quest attempt: {rand} out of {probability}");
		bool isSuccess = rand <= probability;

		if (isSuccess)
		{
			adventurer.ChangeReward(reward);
			adventurer.ChangeExperience(CalculateExperienceGained(probability));
			adventurer.ChangeHappiness(CalculateHappinessChange(probability));
		}
		else
		{
			adventurer.ChangeHappiness(-CalculateHappinessChange(100f - probability));
			adventurer.ChangeHealth(CalculateHealthChange(probability));
		}

		return isSuccess;
	}

	public bool IsAvailable(Adventurer adv)
	{
		if (minLevel > adv.Level) return false;
		if (maxLevel < adv.Level) return false;

		List<System.Guid> prereqs = preReqIds.Select(id => System.Guid.Parse(id)).ToList();
		foreach (System.Guid id in prereqs)
		{
			// find the map activity that uses this Activity ID
			Map.Instance.FindMapActivityWithDataID(id, out MapActivity mapActivity);

			if (mapActivity == null)
			{
				// This prereq isn't on the map! Auto-false
				return false;
			}

			if (!adv.HistoryLog[mapActivity.locationParent.Id].activityAttempts[mapActivity.Id].hasCompleted)
			{
				return false;
			}
		}

		return true;
	}

	#endregion
}