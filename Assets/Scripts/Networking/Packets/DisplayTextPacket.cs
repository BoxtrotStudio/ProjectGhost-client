using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTextPacket : Packet {
	protected override void Handle()
	{
		var textLength = Consume(4).AsInt();
		var text = Consume(textLength).AsString();

		var size = Consume(4).AsInt();
		var colorInt = Consume(4).AsInt();
		var x = Consume(4).AsFloat();
		var y = Consume(4).AsFloat();
		var options = Consume(4).AsInt();
		var id = Consume(8).AsLong();
		
		//TODO Extract rgba from colorInt (8 bits for each value)
	}
}
