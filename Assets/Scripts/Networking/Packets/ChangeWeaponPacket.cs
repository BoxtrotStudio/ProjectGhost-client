using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeaponPacket : Packet {
    protected override void Write(params object[] args)
    {
        var weapon = args[0] as byte?;

        Write((byte)0x22);
        Write(weapon);
        EndTCP();
    }
}
