using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionPacket : Packet {
	protected override void Write(params object[] args)
	{
		var session = args[0] as string;

		Write((byte) 0x00);
		Write((short) session.Length);
		Write(session);

		if (args.Length == 2)
		{
			var stream = args[1] as Stream?;
			if (stream != null) Write((byte) stream);
		}

		EndTCP();
	}
}
