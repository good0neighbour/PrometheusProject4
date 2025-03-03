using System;

[Serializable]
public class City
{
    public string Name { get; set; }
    public byte Land { get; set; }
    public ushort Capacity { get; set; }
    public float Population { get; set; }
    public float PopulationMovement { get; set; }
    public float Crime { get; set; }
    public float Death { get; set; }
    public float AnnualFund { get; set; }
    public float AnnualResearch { get; set; }
    public float AnnualCulture { get; set; }
    public float Stability { get; set; }
    public TechStatus[] Facilites { get; set; }


    public City(string name, byte landIndex, ushort capacity)
    {
        Name = name;
        Land = landIndex;
        Capacity = capacity;
        PopulationMovement = 1.0f;
    }
}
