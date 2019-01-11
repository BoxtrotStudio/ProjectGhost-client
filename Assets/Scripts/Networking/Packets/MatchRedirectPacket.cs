using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRedirectPacket : Packet {
	protected override void Handle()
	{
		var ip = Consume(Consume(1).AsByte()).AsString();
		var port = Consume(2).AsShort();

		//Ghost.client = PlayerClient.connect(ip + ":" + port);
		//var packet = new SessionPacket();

		//packet.writePacket(Ghost.client, Ghost.Session);
		//if (!Ghost.client.ok()) {
		//	throw IOException("Bad session!")
		//}
		//Ghost.client.isValidated = true
	}
}
