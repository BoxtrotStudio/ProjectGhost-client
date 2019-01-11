using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInput : BindableMonoBehavior
{
	[BindComponent]
	private Entity entity;

	public PlayerClient Player
	{
		get { return Game.Manager<NetworkBattleManager>().Client; }
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{	
			var loc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			RequestMovement(loc);
		} 
		else if (Input.GetMouseButtonDown(1))
		{
			var loc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			RequestFire(loc);
		}
	}

	private void RequestMovement(Vector2 target, bool pathfinding = false)
	{
		byte actionByte = (byte) (pathfinding ? 0x3 : 0x0);
		
		SendActionRequest(target, actionByte);
	}

	private void RequestFire(Vector2 target)
	{
		byte actionByte = 0x1;
		SendActionRequest(target, actionByte);
	}

	private void SendActionRequest(Vector2 target, byte actionByte)
	{
		var packet = new ActionRequestPacket();
		packet.WritePacket(Player, actionByte, target.x, target.y);
	}
}
