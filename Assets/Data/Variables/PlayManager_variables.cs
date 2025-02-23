public partial class PlayManager
{
	// Field
	private PlayData _data = new PlayData();

	// Properties
	public float Exploration{ get { return _data.Exploration; } set { _data.Exploration = value; } }

	// Structure
	private struct PlayData
	{
		public float Exploration;
	}
}