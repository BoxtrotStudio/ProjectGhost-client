using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEndPacket : Packet {
	protected override void Handle()
	{
		var isWinner = Consume(1).AsBool();
		var matchId = Consume(8).AsLong();
		var chunkSize = Consume(4).AsInt();
		var stats = Consume(chunkSize).AsType<TemporaryStats>();
		
		//TODO End match
	}
}
