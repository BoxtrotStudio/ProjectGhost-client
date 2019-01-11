using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PacketFactory {
    
    
    private static readonly Packet[] PACKETS = new Packet[255];

    static PacketFactory()
    {
        PACKETS[0x01] = new OkPacket();
        PACKETS[0x02] = new MatchInfoPacket();
        PACKETS[0x04] = new BulkEntityStatePacket();
        PACKETS[0x06] = new MatchStatusPacket();
        PACKETS[0x07] = new MatchEndPacket();
        PACKETS[0x09] = new PingPongPacket();
        PACKETS[0x10] = new SpawnEntityPacket();
        PACKETS[0x11] = new DespawnEntityPacket();
        PACKETS[0x12] = new PlayerStatePacket();
        //DEPRECATED 0X19 PING
        PACKETS[0x26] = new MatchRedirectPacket();
        PACKETS[0x29] = new UpdateSessionPacket();
        //DEPRECATED 0X30 OVER EVENT API
        //PACKETS[0x30] = new SpawnEffectPacket();
        PACKETS[0x31] = new StatsUpdatePacket();
        PACKETS[0x32] = new ItemActivatedEffect();
        PACKETS[0x33] = new ItemDeactivatedPackets();
        PACKETS[0x35] = new MapSettingsPacket();
        PACKETS[0x37] = new SpawnLightPacket();
        PACKETS[0x38] = new UpdateInventoryPacket();
        PACKETS[0x40] = new EventPacket();
        PACKETS[0x42] = new DisconnectReasonPacket();
        PACKETS[0x43] = new DisplayTextPacket();
        PACKETS[0x44] = new RemoveTextPacket();
    }
    
    public static Packet GetPacket(int opCode) {
        Packet packet = PACKETS[opCode];

        if (packet == null) {
            Debug.Log("Invalid opcode: " + opCode);
            return null;
        }

        return packet;
    }

    public static Packet GetPacket(int opCode, byte[] data) {
        Packet p = GetPacket(opCode);
        if (p != null)
            p.AttachPacket(data);

        return p;
    }
}
