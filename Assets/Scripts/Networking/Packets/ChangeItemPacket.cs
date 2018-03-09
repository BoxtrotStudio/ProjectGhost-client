using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeItemPacket : Packet {
	protected override void Write(params object[] args)
	{
		var item = args[0] as byte?;

		Write((byte)0x50);
		Write(item);
		EndTCP();
	}
}
