using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyPacket : Packet {
    protected override void Write(params object[] args)
    {
        var ready = args[0] as bool?;

        Write((byte)0x03);
        Write(ready);
        EndTCP();
    }
}
