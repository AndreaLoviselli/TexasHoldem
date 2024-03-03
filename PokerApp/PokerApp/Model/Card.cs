

using PokerApp.Rules;

namespace PokerApp.Model;

public class Card
{
    public int Number { get; set; }
    public Suit Suit { get; set; }

    public Card(int number, Suit suit)
    {
        this.Number = number;
        this.Suit = suit;
    }

    public override string ToString()
    {
        var symbol = 'n';

        if (this.Number == Manual.Ace) symbol = 'A';
        if (this.Number == Manual.King) symbol = 'K';
        if (this.Number == Manual.Queen) symbol = 'Q';
        if (this.Number == Manual.Jack) symbol = 'J';
        if (this.Number == Manual.Ten) symbol = 'T';

        return Suit switch
        {
            Suit.Hrt => symbol != 'n' ? $" {symbol}\u2665 " : $" {this.Number}\u2665 ",
            Suit.Dmd => symbol != 'n' ? $" {symbol}\u2666 " : $" {this.Number}\u2666 ",
            Suit.Clb => symbol != 'n' ? $" {symbol}\u2663 " : $" {this.Number}\u2663 ",
            Suit.Spd => symbol != 'n' ? $" {symbol}\u2660 " : $" {this.Number}\u2660 ",
            _ => $"ERROR IN DISPLAYNG CARD",
        };
    }
}