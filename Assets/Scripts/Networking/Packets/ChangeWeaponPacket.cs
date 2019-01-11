using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeaponPacket : Packet {
    protected override void Write(params object[] args)
    {
        var weapon = args[0] as byte?;

        if (weapon == 0)
            weapon = 1; //Fail safe if they choose default (0) weapon

        Write((byte)0x22);
        Write(weapon);
        EndTCP();
    }
}
