using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextToVariable : BindableMonoBehavior
{
	public StringVariable variable;
	
	[BindComponent]
	private InputField field;

	void Start () {
		if (variable != null)
		{
			field.onValueChanged.AddListener(delegate(string arg0) { variable.Value = arg0; });
		}
	}
}
