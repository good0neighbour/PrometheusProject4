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
	}
}