using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LoadFromPathSprite : MonoBehaviour
{
	public static readonly int folderNameLength = "sprites/".Length;
	public string spriteName;
	private SpriteRenderer renderer;
	// Use this for initialization
	void Start ()
	{
		if (spriteName.Contains(".png"))
			spriteName = spriteName.Replace(".png", "");

		if (spriteName.StartsWith("sprites/"))
			spriteName = spriteName.Substring(folderNameLength);
		
		renderer = GetComponent<SpriteRenderer>();

		var sprite = Resources.Load<Sprite>("Sprites/" + spriteName);

		renderer.sprite = sprite;
	}
}
