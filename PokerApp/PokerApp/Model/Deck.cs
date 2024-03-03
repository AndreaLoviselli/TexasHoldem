using PokerApp.Rules;
using PokerApp.Utils;

namespace PokerApp.Model;

public class Deck
{
    public Stack<Card> Cards { get; set; }

    public Deck()
    {
        this.Cards = AddStraightsDesc();
        Shuffle();
    }

    private Stack<Card> AddStraightsDesc()
    {
        List<Card> cards = new();
        
        cards.AddRange(InizializeAStraight(Suit.Hrt));
        cards.AddRange(InizializeAStraight(Suit.Dmd));
        cards.AddRange(InizializeAStraight(Suit.Clb));
        cards.AddRange(InizializeAStraight(Suit.Spd));

        return new Stack<Card>(cards);
    }

    private List<Card> InizializeAStraight(Suit st)
    {
        List<Card> straight = new();

        for (var i = Manual.Deuce; i <= Manual.Ace; i++)
        {
            Card card = new(i, st);
            straight.Add(card);
        }

        return straight;
    }

    public void Shuffle()
    {
        this.Cards = new Stack<Card>(this.Cards.OrderBy(_ => Rand.UseNext()));
    }
}