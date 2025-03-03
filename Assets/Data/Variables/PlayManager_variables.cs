using System;

public partial class PlayManager
{
	// Properties
	public ushort Year{ get { return _data.Year; } set { _data.Year = value; } }
	public byte Month{ get { return _data.Month; } set { _data.Month = value; } }
	public float Fund{ get { return _data.Fund; } set { _data.Fund = value; } }
	public float Research{ get { return _data.Research; } set { _data.Research = value; } }
	public float Culture{ get { return _data.Culture; } set { _data.Culture = value; } }
	public ushort StoneTotal{ get { return _data.StoneTotal; } set { _data.StoneTotal = value; } }
	public ushort StoneUsage{ get { return _data.StoneUsage; } set { _data.StoneUsage = value; } }
	public ushort IronTotal{ get { return _data.IronTotal; } set { _data.IronTotal = value; } }
	public ushort IronUsage{ get { return _data.IronUsage; } set { _data.IronUsage = value; } }
	public ushort HeavyMetalTotal{ get { return _data.HeavyMetalTotal; } set { _data.HeavyMetalTotal = value; } }
	public ushort HeavyMetalUsage{ get { return _data.HeavyMetalUsage; } set { _data.HeavyMetalUsage = value; } }
	public ushort PreciousMetalTotal{ get { return _data.PreciousMetalTotal; } set { _data.PreciousMetalTotal = value; } }
	public ushort PreciousMetalUsage{ get { return _data.PreciousMetalUsage; } set { _data.PreciousMetalUsage = value; } }
	public ushort NuclearTotal{ get { return _data.NuclearTotal; } set { _data.NuclearTotal = value; } }
	public ushort NuclearUsage{ get { return _data.NuclearUsage; } set { _data.NuclearUsage = value; } }
	public ushort ElectricityTotal{ get { return _data.ElectricityTotal; } set { _data.ElectricityTotal = value; } }
	public ushort ElectricityUsage{ get { return _data.ElectricityUsage; } set { _data.ElectricityUsage = value; } }
	public float FacilitySupportRate{ get { return _data.FacilitySupportRate; } set { _data.FacilitySupportRate = value; } }
	public float ResearchSupportRate{ get { return _data.ResearchSupportRate; } set { _data.ResearchSupportRate = value; } }
	public float Explore{ get { return _data.Explore; } set { _data.Explore = value; } }
	public ushort ExploreCost{ get { return _data.ExploreCost; } set { _data.ExploreCost = value; } }
	public float ExploreAmn{ get { return _data.ExploreAmn; } set { _data.ExploreAmn = value; } }
	public float ExploreIncMlt{ get { return _data.ExploreIncMlt; } set { _data.ExploreIncMlt = value; } }
	public sbyte ExploreNum{ get { return _data.ExploreNum; } set { _data.ExploreNum = value; } }
	public float ExploreSpdMlt{ get { return _data.ExploreSpdMlt; } set { _data.ExploreSpdMlt = value; } }
	public ushort CityCostSmall{ get { return _data.CityCostSmall; } set { _data.CityCostSmall = value; } }
	public ushort CityCostMiddle{ get { return _data.CityCostMiddle; } set { _data.CityCostMiddle = value; } }
	public ushort CityCostLarge{ get { return _data.CityCostLarge; } set { _data.CityCostLarge = value; } }
	public ushort CityCapaSmall{ get { return _data.CityCapaSmall; } set { _data.CityCapaSmall = value; } }
	public ushort CityCapaMiddle{ get { return _data.CityCapaMiddle; } set { _data.CityCapaMiddle = value; } }
	public ushort CityCapaLarge{ get { return _data.CityCapaLarge; } set { _data.CityCapaLarge = value; } }

	// Structure
	[Serializable]
	public struct PlayData
	{
		public ushort Year;
		public byte Month;
		public float Fund;
		public float Research;
		public float Culture;
		public ushort StoneTotal;
		public ushort StoneUsage;
		public ushort IronTotal;
		public ushort IronUsage;
		public ushort HeavyMetalTotal;
		public ushort HeavyMetalUsage;
		public ushort PreciousMetalTotal;
		public ushort PreciousMetalUsage;
		public ushort NuclearTotal;
		public ushort NuclearUsage;
		public ushort ElectricityTotal;
		public ushort ElectricityUsage;
		public float FacilitySupportRate;
		public float ResearchSupportRate;
		public float Explore;
		public ushort ExploreCost;
		public float ExploreAmn;
		public float ExploreIncMlt;
		public sbyte ExploreNum;
		public float ExploreSpdMlt;
		public ushort CityCostSmall;
		public ushort CityCostMiddle;
		public ushort CityCostLarge;
		public ushort CityCapaSmall;
		public ushort CityCapaMiddle;
		public ushort CityCapaLarge;
	}
}