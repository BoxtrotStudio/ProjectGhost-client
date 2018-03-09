using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPacket : Packet {
	protected override void handle()
	{
		var packetNumber = Consume(4).AsInt();
		if (packetNumber < client.LastRead)
		{
			var dif = client.LastRead - packetNumber;
			if (dif >= Int32.MaxValue - 1000)
			{
				client.LastRead = packetNumber;
			}
			else return;
		}

		var eventID = Consume(2).AsShort();
		var causeID = Consume(2).AsShort();
		var direction = Consume(8).AsDouble();

		//TODO Find event trigger by id and causing entity by id
		
		/*var cause = client.game.findEntity(causeID) ?: return

		for (event in StandardEvent.values()) {
			if (event.id == eventID) {
				event.trigger(cause, direction, client.game.world)
				break
			}
		}*/
	}
}
