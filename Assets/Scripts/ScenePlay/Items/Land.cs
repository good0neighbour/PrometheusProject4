using System;
using static Constants;
using Random = UnityEngine.Random;

[Serializable]
public struct Land
{
    public byte Stone;
    public byte Iron;
    public byte HeavyMetal;
    public byte PreciousMetal;
    public byte Nuclear;

    public void RandomResources()
    {
        if (Random.Range(0, 100) < STONE_POS)
        {
            Stone = (byte)Random.Range(1, MAX_STONE);
        }
        if (Random.Range(0, 100) < IRON_POS)
        {
            Iron = (byte)Random.Range(1, MAX_IRON);
        }
        if (Random.Range(0, 100) < HEAVY_POS)
        {
            HeavyMetal = (byte)Random.Range(1, MAX_HEAVY);
        }
        if (Random.Range(0, 100) < PRECIOUS_POS)
        {
            PreciousMetal = (byte)Random.Range(1, MAX_PRECIOUS);
        }
        if (Random.Range(0, 100) < NUCLEAR_POS)
        {
            Nuclear = (byte)Random.Range(1, MAX_NUCLEAR);
        }
    }
}
