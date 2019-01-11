using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLightPacket : Packet {
	protected override void Handle()
	{
		var id = Consume(2).AsShort();

		var x = Consume(4).AsFloat();
		var y = Consume(4).AsFloat();
		var radius = Consume(4).AsFloat();
		var intensity = Consume(4).AsFloat();

		var color = Consume(4).AsInt();

		var castShadows = Consume(1).AsBool();
		var isConeLight = Consume(1).AsBool();

		var directionDegrees = 90f;
		var coneDegrees = 30f;
		
		//TODO Spawn light
	}
}
