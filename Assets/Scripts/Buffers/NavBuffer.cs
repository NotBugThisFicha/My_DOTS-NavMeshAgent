using Unity.Entities;
using Unity.Mathematics;

public struct NavBuffer : IBufferElementData
{
    public float3 wayPoint;
}
