using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsUpdatePacket : Packet {
	protected override void Handle()
	{
		var id = Consume(4).AsString();
		var value = Consume(8).AsDouble();
		
		/*
		 when (id) {
            "mspd" -> client.game.player1?.speedStat = value
            "frte" -> client.game.player1?.fireRateStat = value
        }
		 */
	}
}
