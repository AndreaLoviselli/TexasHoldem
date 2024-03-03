namespace PokerApp.Utils;

public class Rand
{
    private static readonly Random Rnd = new();

    public static int UseNext()
    {
        return Rnd.Next();
    }
}