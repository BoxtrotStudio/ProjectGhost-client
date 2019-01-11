using UnityEngine;

public abstract class IManager : ScriptableObject
{   
    /// <summary>
    /// Invoked when this manager is initalized. This may be invoked by a GameObject or by another Manager, however
    /// this shouldn't affect the result.
    /// </summary>
    public abstract void OnStart();

    /// <summary>
    /// Invoked whenever Update() is invoked. This may be invoked by a GameObject or by another Manager, however
    /// this shouldn't affect the result.
    /// </summary>
    public abstract void OnUpdate();

    /// <summary>
    ///  Invoked when this manager is destroyied. This may be invoked by a GameObject or by another Manager, however
    /// this shouldn't affect the result.
    /// </summary>
    public abstract void OnStop();
}