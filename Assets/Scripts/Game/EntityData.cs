using UnityEngine;

public class EntityData : ScriptableObject
{
    public long id;
    public Vector2? target;
    public Vector2 position;
    public Vector2 velocity;
    public float rotation;
    public float alpha = 1f;
    public bool isMoving;

    public void UpdateEntity(Entity entity)
    {
        entity.transform.Rotate(0f, 0f, Mathf.Rad2Deg * rotation);
        
        if (Mathf.Abs(entity.transform.position.x - position.x) < 2 && Mathf.Abs(entity.transform.position.y - position.y) < 2)
        {
            //We are pretty close to where we should be, just jump there
            var newPos = new Vector3(position.x + ((Ghost.Latency / 60f) * velocity.x), position.y + ((Ghost.Latency / 60f) * velocity.y));
            entity.transform.position = newPos;
        }
        else
        {
            //TODO Lerp
            //entity.interpolateTo(x, y, Ghost.UPDATE_INTERVAL);
            var newPos = new Vector3(position.x + ((Ghost.Latency / 60f) * velocity.x), position.y + ((Ghost.Latency / 60f) * velocity.y));
            entity.transform.position = newPos;
        }
        
        entity.SetVelocity(velocity);
        
        entity.alpha = alpha / 255f;
    }
}