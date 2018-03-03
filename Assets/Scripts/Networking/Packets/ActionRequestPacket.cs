using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRequestPacket : Packet {
	protected override void Write(params object[] args)
	{
		var action = args[0] as byte?;
		var x = args[1] as float?;
		var y = args[2] as float?;

		client.SendCount++;
		
		Write((byte)0x08);
		Write(client.SendCount);
		Write(action);
		Write(x);
		Write(y);
		EndUdp();
	}
}
