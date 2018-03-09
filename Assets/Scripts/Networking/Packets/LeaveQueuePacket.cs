using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveQueuePacket : Packet {
	protected override void Write(params object[] args)
	{
		Write((byte)0x20);
		EndTCP();
	}
}
