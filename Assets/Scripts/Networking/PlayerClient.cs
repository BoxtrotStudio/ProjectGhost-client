using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[CreateAssetMenu]
public class PlayerClient : ScriptableObject, IPlayerClient
{
	#region TCP Variables
	private TcpClient tcpSocket;
	private NetworkStream tcpStream;
	private Thread tcpListenThread;
	#endregion
	
	#region UDP Variables
	private UdpClient udpClient;
	private Thread udpListenThread;
	#endregion
	
	#region Internal Variables
	private string session;
	private List<WaitForOk> _okWaiters = new List<WaitForOk>();
	#endregion

	public StringVariable IP;
	public ShortVariable Port;
	public MatchInfo CurrentMatch;
	public MatchJoinEventFactory JoinEvent;

	[Obsolete]
	public string ip
	{
		get { return IP.Value; }
		set { IP.Value = value; }
	}

	[Obsolete]
	public short port
	{
		get { return Port.Value; }
		set { Port.Value = value; }
	}
	public bool isLocal;

	public bool IsConnected
	{
		get { return tcpReady && udpReady; }
	}

	private bool tcpReady, udpReady;

	void OnEnable()
	{
		//Always ensure these are 0 at start??
		LastRead = 0;
		SendCount = 0;
		
		GlobalConfig gConfig = new GlobalConfig();
		NetworkTransport.Init(gConfig);
	}

	private void OnDestroy()
	{
		disconnect();
	}

	public IEnumerator MakeSession(string username, string password, bool offline = false)
	{
		if (offline)
		{
			password = "offline";
		}
		
		var values = new Dictionary<string, string>();
		values.Add("username", username);
		values.Add("password", password);

		UnityWebRequest www = UnityWebRequest.Post("http://" + ip + ":8080/api/accounts/login", values);
		
		yield return www.SendWebRequest();
		
		if(www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		}
		else {
			if (www.responseCode == 202)
			{
				string cookie = www.GetResponseHeader("Set-Cookie");

				session = cookie.Split('=')[1].Split(';')[0];
			}
		}
	}

	public void ConnectUDP()
	{
		udpClient = new UdpClient();
		
		udpClient.Connect(IPAddress.Parse(ip), port);
		
		udpListenThread = new Thread(ListenForUdpData) {IsBackground = true};
		udpListenThread.Start();
		
		
		UdpSessionPacket packet = new UdpSessionPacket();
		packet.WritePacket(this, session);
		udpReady = true;
	}

	public void Connect()
	{
		string[] temp = ip.Split(':');
		if (temp.Length > 1) {
			port = short.Parse(temp[1]);
			ip = temp[0];
		}
		
		tcpSocket = new TcpClient(ip, port);
		tcpStream = tcpSocket.GetStream();

		tcpListenThread = new Thread(ListenForData) {IsBackground = true};
		tcpListenThread.Start();
		
		tcpReady = true;
	}

	public void writeUDP(byte[] data)
	{
		udpClient.Send(data, data.Length);
	}

	public void write(byte[] data)
	{
		if (tcpStream.CanWrite)
		{
			tcpStream.Write(data, 0, data.Length);
		}
	}

	public void flush()
	{
		tcpStream.Flush();
	}

	public int read(byte[] data, int index, int length)
	{
		if (!tcpStream.CanRead) return -1;
		byte[] temp = new byte[length];
		int read = tcpStream.Read(temp, 0, length);
			
		Array.Copy(temp, 0, data, index, read);

		return read;
	}

	public void disconnect()
	{
		if (tcpReady)
		{
			tcpStream.Close();
			tcpReady = false;
			tcpListenThread.Interrupt();
		}

		if (udpReady)
		{
			udpClient.Close();
			udpReady = false;
			udpListenThread.Interrupt();
		}
	}

	public int SendCount { get; set; }
	public int LastRead { get; set; }

	public void SendSession()
	{
		var packet = new SessionPacket();
		packet.WritePacket(this, session);
	}
	
	public bool IsOk { get; private set; }

	public IEnumerator WaitForOk()
	{
		var toReturn = new WaitForOk();
		_okWaiters.Add(toReturn);

		yield return toReturn;
	}

	public void GotOk(bool val)
	{
		IsOk = val;
		
		foreach (var waiter in _okWaiters)
		{
			waiter.GotOk(val);
		}
		
		_okWaiters.Clear();
	}

	public void JoinMatch(MatchInfo info)
	{
		if (Game.instance == null)
		{
			Debug.LogError("Joining a match requires a GameManager!");
			return;
		}
		
		Game.instance.InvokeOnNextUpdate(delegate
		{
			CurrentMatch = info;
			
			if (JoinEvent == null)
			{
				Debug.LogWarning("Match Join Packet not handled!");
				return;
			}
		
			JoinEvent.Invoke(info);
		});
	}

	private void ListenForUdpData()
	{
		int errCount = 0;
		IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		while (udpReady)
		{
			try
			{
				byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);

				byte opcode = receiveBytes[0];

				byte[] data = new byte[receiveBytes.Length - 1];

				Array.Copy(receiveBytes, 1, data, 0, receiveBytes.Length - 1);

				Packet p = PacketFactory.GetPacket(opcode, data);

				if (p != null)
				{
					p.HandlePacket(this);
				}
				else
				{
					Debug.LogError("Unknown Opcode: " + opcode);
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				errCount++;
				if (errCount >= 3)
					break;
			}
		}
	}

	private void ListenForData()
	{
		int errCount = 0;
		while (tcpReady)
		{
			try
			{
				int opcode = tcpStream.ReadByte();
				Debug.Log("Got " + opcode);
				if (opcode == -1)
				{
					//TODO Disconnected
					break;
				}

				Packet p = PacketFactory.GetPacket(opcode);

				if (p != null)
				{
					//Otherwise run in this thread (may be unsafe)
					p.HandlePacket(this);
				}
				else
				{
					Debug.LogError("Unknown Opcode: " + opcode);
				}

				errCount = 0;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				errCount++;
				if (errCount >= 3)
					break;
			}
		}
	}
}