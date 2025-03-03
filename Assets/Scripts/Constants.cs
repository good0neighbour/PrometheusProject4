using UnityEngine;

static public partial class Constants
{
    // Game
    public const byte MAX_GAME_SPEED = 4;
    public const byte MAX_LAND_NUM = 64;
    public const float MONTH_PER_SEC = 1.0f;
    public const float MESSAGE_SHOW_TIME = 2.0f;
    public const float COST_ANIM_TIME = 0.2f;
    public const float ADOPT_ANIM_TIME = 1.0f;

    // Resources
    public const byte STONE_POS = 50;
    public const byte IRON_POS = 40;
    public const byte HEAVY_POS = 30;
    public const byte PRECIOUS_POS = 20;
    public const byte NUCLEAR_POS = 10;
    public const byte MAX_STONE = 5;
    public const byte MAX_IRON = 5;
    public const byte MAX_HEAVY = 3;
    public const byte MAX_PRECIOUS = 1;
    public const byte MAX_NUCLEAR = 2;

    // Colour
    static public readonly Color DISABLED = new Color(0.3f, 0.3f, 0.3f, 1.0f);
    static public readonly Color UNSELECTED = new Color(0.1f, 0.1f, 0.1f, 1.0f);
    static public readonly Color SELECTED = new Color(0.2f, 0.2f, 0.2f, 1.0f);
    static public readonly Color NORMALTEXT = new Color(0.3921568627450980392156862745098f, 0.58823529411764705882352941176471f, 1.0f, 1.0f);
    static public readonly Color FAILEDTEXT = new Color(1.0f, 0.3f, 0.0f, 1.0f);
}