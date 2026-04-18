
/// <summary>
/// 以下、VisualStudioを使用して、jsonから自動生成したクラス
/// </summary>

public class Rootobject
{
    public Systemdata SystemData { get; set; }
    public Gamedata GameData { get; set; }
}

public class Systemdata
{
    public string PlayerName { get; set; }
    public string Volume { get; set; }
    public Displaysize DisplaySize { get; set; }
}

public class Displaysize
{
    public string X { get; set; }
    public string Y { get; set; }
}

public class Gamedata
{
    public Slotdata[] SlotData { get; set; }
}

public class Slotdata
{
    public string SlotID { get; set; }
    public Playerdata PlayerData { get; set; }
    public Heroinedata[] HeroineData { get; set; }
}

public class Playerdata
{
    public string Turn { get; set; }
    public string Money { get; set; }
}

public class Heroinedata
{
    public string Name { get; set; }
}
