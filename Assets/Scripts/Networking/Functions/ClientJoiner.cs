using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ClientJoiner : MonoBehaviour
{
	public PlayerClient client;
	public StringVariable Username;
	public UnityEvent OnJoin;

	public void Join()
	{
		StartCoroutine(DoJoin());
	}

	private IEnumerator DoJoin()
	{
		yield return client.MakeSession(Username.Value, "", true);
		
		client.Connect();

		client.SendSession();

		yield return client.WaitForOk();

		if (!client.IsOk)
		{
			Debug.LogError("Did not get OK from server..");
			yield break;
		}
		
		client.ConnectUDP();

		yield return client.WaitForOk();

		if (!client.IsOk)
		{
			Debug.LogError("Did not get OK from server..");
			yield break;
		}
		
		Debug.Log("Connected!");

		if (OnJoin != null)
		{
			OnJoin.Invoke();
		}
	}
}
