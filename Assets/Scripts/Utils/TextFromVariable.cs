using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFromVariable : BindableMonoBehavior
{

	[BindComponent]
	private Text Field;

	public StringVariable Variable;
	
	// Update is called once per frame
	void Update ()
	{
		Field.text = Variable.Value;
	}
}
