using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LoadFromPathSprite : MonoBehaviour
{

	public string spriteName;
	private SpriteRenderer renderer;
	// Use this for initialization
	void Start ()
	{
		if (spriteName.Contains(".png"))
			spriteName = spriteName.Replace(".png", "");
		
		renderer = GetComponent<SpriteRenderer>();

		var sp = Resources.Load("Sprites/" + spriteName) as Texture2D;

		var sprite = Sprite.Create(sp, new Rect(0, 0, sp.width, sp.height), new Vector2(0.5f, 0.5f));

		renderer.sprite = sprite;
	}
}
