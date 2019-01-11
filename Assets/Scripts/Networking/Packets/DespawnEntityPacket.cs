using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnEntityPacket : Packet {
    protected override void Handle()
    {
        var id = Consume(2).AsShort();

        Game.Manager<EntityManager>().Despawn(id);
    }
}
