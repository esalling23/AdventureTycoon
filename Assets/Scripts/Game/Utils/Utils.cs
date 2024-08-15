using UnityEngine;
using System.Collections;

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
}