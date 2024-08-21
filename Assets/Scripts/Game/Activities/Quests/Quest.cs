using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest : ActivityBase
{
	#region Fields

	// 	prereqs: list of activity (quest) Ids that are required before adventurer can attempt them
	public List<System.Guid> preReqIds = new();
	// level: int representing min level an adventurer must be to attempt them
	public int minLevel = 1;
	public int maxLevel = 5;
	public int baseExperience = 100;
	// reward: int representing gold adventurers earn from success
	public int reward = 100;
	// items: List of items ?


	private float _minSuccessProbability = 0.25f;
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

	#endregion
}