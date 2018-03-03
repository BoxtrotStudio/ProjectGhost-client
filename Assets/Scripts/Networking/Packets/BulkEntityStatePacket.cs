
using System;
using UnityEngine;

public class BulkEntityStatePacket : Packet
{
    protected override void handle()
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
            var rotation = Consume(8).AsDouble();
            var serverMs = Consume(8).AsLong();
            var hasTarget = Consume(1).AsBool();

            var entity = GameManager.instance.FindObjectById(id);

            //translate x and y to libgdx's fucked up joke of a coordinate system
            //x -= entity.width / 2f;
            //y -= entity.height / 2f;

            /*if (Ghost.latency > 0) {
                val ticksPassed = Ghost.latency / (1000f / 60f)
                if (ticksPassed >= 2) {
                    val xadd = xVel * ticksPassed
                    val yadd = yVel * ticksPassed

                    System.out.println("" + ticksPassed + " ticks skipped since sent. " + xadd + ":" + yadd + " being added to movement");

                    x += xadd
                    y += yadd
                }
            }*/

            var xTarget = 0f;
            var yTarget = 0f;
            if (hasTarget)
            {
                xTarget = Consume(4).AsFloat();
                yTarget = Consume(4).AsFloat();
            }

            /*var shouldUpdate = !entity.isMoving
            when {
                hasTarget && entity.target != null -> {
                    if (xTarget != entity.target.x || yTarget != entity.target.y) shouldUpdate = true
                    else entity.isMoving = false
                }
                hasTarget && entity.target == null -> {
                    shouldUpdate = true
                }
            }*/
            //entity.rotation = Mathf.toDegrees(rotation).toFloat()

            if (Mathf.Abs(entity.transform.position.x - x) < 2 && Mathf.Abs(entity.transform.position.y - y) < 2)
            {
                
                var newPos = new Vector3(x + ((Ghost.latency / 60f) * xVel), y + ((Ghost.latency / 60f) * yVel));
                entity.transform.position = newPos;
            } 
            else
            {
                //TODO Lerp
                //entity.interpolateTo(x, y, Ghost.UPDATE_INTERVAL);
            }

            /*entity.velocity = new Vector2(xVel, yVel);

            if (hasTarget)
            {
                xTarget -= entity.width / 2f;
                yTarget -= entity.height / 2f;

                entity.target = new Vector2(xTarget, yTarget);
            }

            entity.alpha = alpha / 255f;*/
        }
    }
}