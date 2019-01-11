using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchStatusPacket : Packet {
	protected override void Handle()
	{
		var state = Consume(1).AsBool();
		var reasonLength = Consume(4).AsInt();
		var reason = Consume(reasonLength).AsString();

		var bm = Game.Manager<BattleManager>();

		if (bm != null)
			bm.MatchText.Value = reason;
	}
}
