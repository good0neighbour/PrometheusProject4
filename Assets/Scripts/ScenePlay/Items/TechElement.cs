using System;

[Serializable]
public class TechElement
{
    public string Name = null;
    public string Description = null;
    public float FundCost = 0.0f;
    public float Maintenance = 0.0f;
    public float ResearchCost = 0.0f;
    public float CultureCost = 0.0f;
    public ushort StoneCost = 0;
    public ushort IronCost = 0;
    public ushort NuclearCost = 0;
    public ushort Electricity = 0;
    public float Population = 0.0f;
    public byte XPos = 0;
    public byte YPos = 0;
    public Requirement[] Requirements = null;


    [Serializable]
    public struct Requirement
    {
        public TechTreeType Type;
        public byte Index;

        public Requirement(TechTreeType type, byte index)
        {
            Type = type;
            Index = index;
        }
    }
}
