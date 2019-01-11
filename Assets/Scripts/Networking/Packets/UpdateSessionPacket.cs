using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSessionPacket : Packet {
	protected override void Handle()
	{
		var newSession = Consume(Consume(4).AsInt()).AsString();

		Ghost.Session = newSession;
	}
}
