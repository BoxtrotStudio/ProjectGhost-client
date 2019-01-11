using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatePacket : Packet {
	protected override void Handle()
	{
		var id = Consume(2).AsShort();
		var lives = Consume(1).AsByte();
		var isDead = Consume(1).AsBool();
		var isFrozen = Consume(1).AsBool();
		var isInvincible = Consume(1).AsBool();
		
		//TODO Update player state
	}
}
