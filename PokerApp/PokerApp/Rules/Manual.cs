namespace PokerApp.Rules;

public static class Manual
{
    public static int Ace { get; } = 14;
    public static int King { get; } = 13;
    public static int Queen { get; } = 12;
    public static int Jack { get;  } = 11;
    public static int Ten { get; } = 10;
    public static int Deuce { get; } = 2;
    public static int BoardSize { get; } = 5;
    public static int StartingChips { get; } = 5000;
    public static int DealingRound { get; } = 2;
    public static int TurnDiscard { get; } = 3;
    public static int RiverDiscard { get; } = 4;
    public const float StillToCheck  = 0f;
    public static int FourOfAKindSize { get; } = 4;
    public static int ThreeOfAKindSize { get; } = 3;
    public static int PairSize { get; } = 2;
    public static int StraightSize { get; } = 5;
    public static int FlushSize { get; } = 5;
    public static float HighCardDivider { get; } = 10f;
    public static int PotentialStraightStart { get; } = 3;
    public static int HandBoardSize { get; } = 7;
    public static int BigBlind { get; } = 50;
    public static int SmallBlind { get; } = 25;
}