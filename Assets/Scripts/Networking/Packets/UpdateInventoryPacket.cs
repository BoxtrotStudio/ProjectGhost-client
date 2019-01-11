using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateInventoryPacket : Packet {
	protected override void Write(params object[] args)
	{
		var type = Consume(2).AsShort();
		var slot = Consume(1).AsByte();
		
		/*
		 if (type == (-1).toShort()) {
            client.game.player1?.inventory?.clearSlot(slot.toInt())
        } else {
            if (slot == 0.toByte()) {
                client.game.player1?.inventory?.setSlot1(type)
            } else {
                client.game.player1?.inventory?.setSlot2(type)
            }
        }
		 */
	}
}
