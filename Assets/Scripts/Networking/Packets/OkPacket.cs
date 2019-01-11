using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OkPacket : Packet {
	protected override void Handle()
	{
		var isOk = Consume(1).AsBool();

		client.GotOk(isOk);
	}
}
