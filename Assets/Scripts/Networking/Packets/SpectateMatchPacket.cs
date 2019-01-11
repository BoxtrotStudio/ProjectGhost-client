using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectateMatchPacket : Packet {
	protected override void Write(params object[] args)
	{
		var matchId = args[0] as long?;

		Write((byte) 0x28);
		Write(matchId);
		EndTCP();
	}
}
