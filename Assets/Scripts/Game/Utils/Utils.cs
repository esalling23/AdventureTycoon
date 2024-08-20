using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils 
{
	public static IEnumerator LerpObject(Transform transform, Vector3 targetPosition, float duration)
	{
		float time = 0;
		Vector3 startPosition = transform.position;

		while (time < duration)
		{
				transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, time / duration);
				time += Time.deltaTime;
				yield return null;
		}
		transform.position = targetPosition;
	}

	public static T GetRandomFromList<T>(List<T> list)
	{
		if (list.Count == 0)
				return default;
		if (list.Count == 1)
				return list[0];

		int rnd = Mathf.FloorToInt(Random.Range(0, list.Count));
		return list[rnd];
	}
}