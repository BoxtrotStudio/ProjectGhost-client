using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingText : BindableMonoBehavior
{

	[BindComponent]
	private Text text;

	public FloatVariable Ping;
	
	void Update()
	{
		text.text = "Ping: " + Ping.Value + "ms";
	}
}
