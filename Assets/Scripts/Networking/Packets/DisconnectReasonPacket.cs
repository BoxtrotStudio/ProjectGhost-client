using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectReasonPacket : Packet {
	protected override void handle()
	{
		var length = Consume(1).AsByte();
		var reason = Consume(length).AsString();
		
		//TODO Show reason in error box
	}
}
