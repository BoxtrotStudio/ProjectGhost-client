using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSettingsPacket : Packet {
	protected override void Write(params object[] args)
	{
		var power = Consume(4).AsFloat();

		var red = Consume(4).AsInt();
		var green = Consume(4).AsInt();
		var blue = Consume(4).AsInt();
		
		var mapNameLength = Consume(4).AsInt();
		var mapName = Consume(mapNameLength).AsString();
		
		//TODO Handle
	}
}
