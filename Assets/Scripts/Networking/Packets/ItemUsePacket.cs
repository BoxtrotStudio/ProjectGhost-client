using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUsePacket : Packet {
	protected override void Write(params object[] args)
	{
		var slot = args[0] as byte?;

		client.SendCount++;

		Write((byte)0x39);
		Write(client.SendCount);
		Write(slot);
		EndUdp();
	}
}
