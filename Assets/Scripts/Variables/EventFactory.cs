using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu]
public class EventFactory : Variable<List<UnityAction>>
{
	void OnEnable()
	{
		Value = new List<UnityAction>();
	}

	public void Invoke()
	{
		foreach (var e in Value)
		{
			e.Invoke();
		}
	}

	public void Subscribe(UnityAction action)
	{
		Value.Add(action);
	}
}