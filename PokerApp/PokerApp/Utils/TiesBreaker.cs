using PokerApp.Model;
using PokerApp.Rules;

namespace PokerApp.Utils;

public static class TiesBreaker
{
    public static List<Player> BreakTie(List<Player> players)
    {
        int score = (int)players.First().Score;
        int scoreType = score % 100 == 0 ? score : score - 1;


        switch (scoreType)
        {
            case (int)PkrScore.Straight:
            case (int)PkrScore.StraightFlush:
            case (int)PkrScore.RoyalFlush:
            case (int)PkrScore.FullHouse:
            case (int)PkrScore.Flush:
                return players;
            default:
                var counterToRecursion = 1;
                return StandardTieHandler(counterToRecursion, players);
        }
    }

    public static List<Player> StandardTieHandler(int cardPosition, List<Player> players)
    {
        List<Player> winners = players;
        
        var sortedPlayers = winners
            .OrderByDescending(w => w.HandPlusBoardDesc[cardPosition].Number)
            .ToList();
        
        var actualFirstCard = sortedPlayers
            .First().HandPlusBoardDesc[cardPosition].Number;
        
        winners
            .RemoveAll(player => player.HandPlusBoardDesc[cardPosition].Number != actualFirstCard);

        if (winners.Count == 1 || cardPosition == 6)
            return winners;
        
        cardPosition++;
        return StandardTieHandler(cardPosition,winners);
    }
}