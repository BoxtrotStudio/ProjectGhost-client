using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DropdownToByte : BindableMonoBehavior
{
	[BindComponent]
	private Dropdown Dropdown;
	
	public ByteVariable Variable;
	public int Offset;
	public ReplaceValueDictionary ReplaceValue = new ReplaceValueDictionary();

	void Start () {
		Dropdown.onValueChanged.AddListener(delegate(int arg0)
		{
			if (ReplaceValue.ContainsKey(arg0))
			{
				Variable.Value = (byte)ReplaceValue[arg0];
			}
			else
			{
				Variable.Value = (byte) (arg0 + Offset);
			}
		});
	}
}

[Serializable]
public class ReplaceValueDictionary : SerializableDictionary<int, int> { }

