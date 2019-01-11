using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UdpSessionPacket : Packet {
	protected override void Write(params object[] args)
	{
		var session = args[0] as string;

		Write((byte) 0x00);
		Write((short) session.Length);
		Write(session);
		EndUdp();
	}
}
