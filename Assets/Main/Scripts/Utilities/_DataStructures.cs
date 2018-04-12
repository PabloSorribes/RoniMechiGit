/// <summary>
/// Contains the tags that are used by objects in the game.
/// By using this, we avoid a rogue typo-error in a random string.
/// </summary>
public class _Tags
{
	public const string player = "Player";
	public const string bullet = "Bullet";
	public const string icicle = "Icicle";
	public const string ground = "Ground";
	public const string wall = "Wall";
	public const string menuPlatforms = "MenuPlatforms";
}

/// <summary>
/// Contains the level names (strings) that are used when loading a level.
/// By using this, we avoid a rogue typo-error in a random string.
/// </summary>
public class _Levels
{
	public const string mainMenu = "Scene_Menu";
	public const string spikeyCavern = "Spikey Cavern_TestVersion2";
	public const string icicleStuff = "Icicle stuffs";
	public const string mayanTemple = "Mayan Tempel";
	public const string testMenu = "test_SpawningInMenu";
	public const string elton = "Scene_Elton";
}

public class _Inputs
{
	public const string shootButton = "Fire1";
	public const string jumpButton = "Jump";
	public const string deflectionButton = "Deflect";
	public const string horizontalAxis = "Horizontal";
	public const string verticalAxis = "Vertical";
}