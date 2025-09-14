using UnityEngine;

/// <summary>
/// Interface for objects that can be returned to an object pool
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Called when the object should be returned to the pool
    /// </summary>
    void ReturnToPool();
}

/// <summary>
/// Optional interface for objects that need to be reset when retrieved from pool
/// </summary>
public interface IResetable
{
    /// <summary>
    /// Called when object is retrieved from pool and needs to be reset
    /// </summary>
    void ResetForReuse();
}