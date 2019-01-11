using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTextPacket : Packet {
	protected override void Handle()
	{
		var textId = Consume(8).AsLong();
		
		//Destory text by id
	}
}
