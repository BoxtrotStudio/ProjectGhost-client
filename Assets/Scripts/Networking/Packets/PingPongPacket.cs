using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongPacket : Packet {
    protected override void Write(params object[] args)
    {
        var toSend = Time.time;

        Write((byte) 0x09);
        Write(toSend);
        Write(new byte[24]);
        EndUdp();
    }

    protected override void Handle()
    {
        var pingStartTime = Consume(4).AsFloat();
        
        Game.instance.InvokeOnNextUpdate(delegate
        {
            var timeTook = Time.time - pingStartTime;
            Ghost.Latency = Mathf.Round(timeTook * 1000f);
            
            
            Debug.Log("Ping: " + Ghost.Latency + " : " + timeTook);
        });
    }
}
