using System.Collections.Generic;
using UnityEngine.Events;

public class GenericEventFactory<T> : Variable<List<UnityAction<T>>>
{
	void OnEnable()
	{
		Value = new List<UnityAction<T>>();
	}
    
	public void Invoke(T val)
	{
		foreach (var e in Value)
		{
			e.Invoke(val);
		}
	}

	public void Subscribe(UnityAction<T> action)
	{
		Value.Add(action);
	}
}
