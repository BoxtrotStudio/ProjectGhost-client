using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : BindableMonoBehavior
{
    [BindComponent(suppressError=true)]
    private Rigidbody2D _rigidbody;

    [BindComponent]
    private SpriteRenderer _renderer;
    
    
    public EntityData data;

    public float alpha
    {
        get
        {
            return _renderer == null ? 0f : _renderer.color.a;
        }
        set {
            if (_renderer != null)
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, value);
        }
    }

    public Vector2 Velocity
    {
        get { return _rigidbody.velocity; }
    }

    public void SetVelocity(Vector2 vel)
    {
        if (_rigidbody != null)
            _rigidbody.velocity = vel;
    }

    void Update()
    {
        data.UpdateEntity(this);
    }
}
