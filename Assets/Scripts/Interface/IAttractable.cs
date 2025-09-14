// IAttractable.cs
using UnityEngine;

public interface IAttractable
{
    void Attract(Vector3 attractorPosition, float attractionForce, float destroyRadius);
}