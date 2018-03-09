using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnEntityPacket : Packet {
    protected override void handle()
    {
        var id = Consume(2).AsShort();
        
        //TODO Despawn based on id
    }
}
