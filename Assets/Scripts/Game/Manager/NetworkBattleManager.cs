using UnityEngine;

[CreateAssetMenu]
public class NetworkBattleManager : BattleManager {
	public PlayerClient Client;

	private float lastPingTime;
	
	public override void OnStart()
	{
		base.OnStart();
		
		var readyPacket = new ReadyPacket();
		readyPacket.WritePacket(Client, true);
		
		
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		
		if (Time.time - lastPingTime > 5) {
			var ping = new PingPongPacket();
			ping.WritePacket(Client);
			lastPingTime = Time.time;
		}
	}
}
