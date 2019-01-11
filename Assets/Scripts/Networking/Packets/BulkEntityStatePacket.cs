
using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BulkEntityStatePacket : Packet
{
    protected override void Handle()
    {
        var packetNumber = Consume(4).AsInt();
        if (packetNumber < client.LastRead)
        {
            var dif = client.LastRead - packetNumber;
            if (dif >= Int32.MaxValue - 1000)
            {
                client.LastRead = packetNumber;
            }
            else return;
        }

        var bulkCount = Consume(4).AsInt();
        for (int i = 0; i < bulkCount; i++)
        {
            var id = Consume(2).AsShort();
            var x = Consume(4).AsFloat();
            var y = Consume(4).AsFloat();
            var xVel = Consume(4).AsFloat();
            var yVel = Consume(4).AsFloat();
            var alpha = Consume(4).AsInt();
            var rotation = (float) Consume(8).AsDouble();
            var serverMs = Consume(8).AsLong();
            var hasTarget = Consume(1).AsBool();

            var xTarget = 0f;
            var yTarget = 0f;
            if (hasTarget)
            {
                xTarget = Consume(4).AsFloat();
                yTarget = Consume(4).AsFloat();
            }

            if (Game.Manager<EntityManager>() == null)
                return;

            var entity = Game.instance.FindObjectById(id);

            if (entity == null)
                return; //Entity doesnt exist (mabye it'll spawn soon?)

            entity.data.alpha = alpha;
            entity.data.position = new Vector2(x, y);
            entity.data.rotation = rotation;
            entity.data.velocity = new Vector2(xVel, yVel);

            if (hasTarget)
            {
                entity.data.target = new Vector2(xTarget, yTarget);
            }
            else
            {
                entity.data.target = null;
            }

            
        }

        client.LastRead = packetNumber;
    }
}