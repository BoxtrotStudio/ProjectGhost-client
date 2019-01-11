using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfoPacket : Packet {
	protected override void Handle()
	{
		var startX = Consume(4).AsFloat();
		var startY = Consume(4).AsFloat();
		var selfID = Consume(2).AsShort();
		var selfC = Consume(1).AsByte();
		var allyCount = Consume(4).AsInt();
		var enemyCount = Consume(4).AsInt();

		var info = new MatchInfo
		{
			StartX = startX,
			StartY = startY,
			SelfID = selfID,
			SelfCharacter = selfC
		};

		for (int i = 0; i < enemyCount; i++)
		{
			var stringSize = Consume(4).AsInt();
			var name = Consume(stringSize).AsString();
			var character = Consume(1).AsByte();
			info.Enemies.Add(name, character);
		}

		for (int i = 0; i < allyCount; i++)
		{
			var stringSize = Consume(4).AsInt();
			var name = Consume(stringSize).AsString();
			var character = Consume(1).AsByte();
			info.Allies.Add(name, character);
		}
		
		client.JoinMatch(info);
	}
}
