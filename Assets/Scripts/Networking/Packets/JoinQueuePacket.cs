using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinQueuePacket : Packet {
	protected override void Write(params object[] args)
	{
		var queue = args[0] as byte?;

		Write((byte)0x05);
		Write(queue);
		EndTCP();
	}
}
