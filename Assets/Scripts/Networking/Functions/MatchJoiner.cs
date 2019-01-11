using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MatchJoiner : MonoBehaviour
{
	public PlayerClient Client;
	public ByteVariable Queue;
	public ByteVariable Weapon;
	public UnityEvent OnQueueJoined;

	public void Join()
	{
		StartCoroutine(DoJoin());
	}

	private IEnumerator DoJoin()
	{
		if (Client.IsConnected)
		{
			var cw = new ChangeWeaponPacket();
			cw.WritePacket(Client, Weapon.Value);
			
			var jq = new JoinQueuePacket();
			jq.WritePacket(Client, Queue.Value);

			yield return Client.WaitForOk();

			if (!Client.IsOk)
			{
				Debug.LogError("Could not join queue!");
				yield break;
			}
			
			Debug.Log("Queue Join Success");

			if (OnQueueJoined != null)
			{
				OnQueueJoined.Invoke();
			}
		}
	}
}
