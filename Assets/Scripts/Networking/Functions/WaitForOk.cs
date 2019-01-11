using UnityEngine;

public class WaitForOk : CustomYieldInstruction
{
    private bool _gotOk;
    private bool _isOk;

    public override bool keepWaiting
    {
        get { return !_gotOk; }
    }

    public bool IsOk
    {
        get { return _isOk; }
    }

    public void GotOk(bool value)
    {
        _gotOk = true;
        _isOk = value;
    }
}