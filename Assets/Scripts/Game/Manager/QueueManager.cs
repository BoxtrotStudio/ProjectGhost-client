using UnityEngine;

[CreateAssetMenu]
public class QueueManager : IManager
{
	public MatchJoinEventFactory Event;
	public BattleManager BattleManager;
	public SceneSwitch SceneSwitch;

	public override void OnStart()
	{
		Event.Subscribe(delegate(MatchInfo arg0)
		{
			if (BattleManager != null)
			{
				BattleManager.info = arg0;
			}

			SceneSwitch.Switch();
		});
	}

	public override void OnUpdate()
	{
	}

	public override void OnStop()
	{
	}
}
