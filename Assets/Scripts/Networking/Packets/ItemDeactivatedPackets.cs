using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDeactivatedPackets : Packet {
	protected override void handle()
	{
		var id = Consume(2).AsShort();
		var owner = Consume(2).AsShort();

		/*var e = client.game.findEntity(owner);

		if (id == 12.toShort()) {
			if (e is NetworkPlayer) {
				val effect = OrbitEffect(e)
				effect.begin()
			}
		}*/
		
		Debug.Log("Item " + id + " was deactivated by " + owner + "!");
	}
}
