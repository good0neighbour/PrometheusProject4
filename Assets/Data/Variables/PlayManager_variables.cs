using System;

public partial class PlayManager
{
	// Properties
	public ushort Year{ get { return _data.Year; } set { _data.Year = value; } }
	public byte Month{ get { return _data.Month; } set { _data.Month = value; } }
	public float Fund{ get { return _data.Fund; } set { _data.Fund = value; } }
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