
public class Rootobject
{
    public Systemdata SystemData { get; set; }
    public Gamedata GameData { get; set; }
}

public class Systemdata
{
    public string PlayerName { get; set; }
    public string AudioVolume { get; set; }
    public int DisplaySize { get; set; }
}

public class Gamedata
{
    public Slotdata[] SlotData { get; set; }
}

public class Slotdata
{
    public int PlayingTurn { get; set; }
    public Playerdata PlayerData { get; set; }
    public Heroinedata[] HeroineData { get; set; }
}

public class Playerdata
{
    public string Turn { get; set; }
    public string Money { get; set; }
    public int[] Items { get; set; }
    public int[] ShopBuyCount { get; set; }
    public int ActionLog { get; set; }
    public int[] MoneyRecord { get; set; }
    public int[] ThisWeekBuyLotteryNumbers { get; set; }
    public int[] LastWeekBuyLotteryNumbers { get; set; }
    public int DateTurn { get; set; }
    public bool[] EventFlg { get; set; }
}

public class Heroinedata
{
    public int RelationShip { get; set; }
    public int Likeability { get; set; }
    public int SupportMoney { get; set; }
    public int PregnancyStatus { get; set; }
    public int ConfinementTurn { get; set; }
    public int FertilizationTurn { get; set; }
    public int NoRelationshipCount { get; set; }
    public bool[] AskedQuestionFlg { get; set; }
    public bool[] MessageActionButtonFlg { get; set; }
}
