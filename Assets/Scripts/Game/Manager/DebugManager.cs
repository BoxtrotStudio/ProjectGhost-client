using UnityEngine;

[CreateAssetMenu]
public class DebugManager : IManager
{
    public FloatVariable Latency;
    
    public override void OnStart()
    {
    }

    public override void OnUpdate()
    {
        Latency.Value = Ghost.Latency;
    }

    public override void OnStop()
    {
    }
}