using PokerApp.Model;
using PokerApp.Rules;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace PokerApp.Utils;

public static class ScoreChecker
{
    public static float Check(List<Card> cards)
    {
        var score = 0f;
        
        score = IsFourOfAKind(cards);
        if (score != Manual.StillToCheck) return score;
        
        score = IsFullHouse(cards);
        if (score != Manual.StillToCheck) return score;
        
        score = IsStraightsOrFlush(cards);
        if (score != Manual.StillToCheck) return score;
        
        score = IsThreeOfAKind(cards);
        if (score != Manual.StillToCheck) return score;
        
        score = IsPairsOrHighCard(cards);
        return score;
    }

    private static float IsFourOfAKind(List<Card> cards)
    {
        var fourOfAKind = cards
            .GroupBy(c => c.Number)
            .FirstOrDefault(g => g.Count() >= Manual.FourOfAKindSize);

        if (fourOfAKind is null) return Manual.StillToCheck;
        var highCard = fourOfAKind.Key;
        return (float)PkrScore.FourOfAKind + highCard / Manual.HighCardDivider;
    }
    private static float IsFullHouse(List<Card> cards)
    {
        var tris = cards
            .GroupBy(c => c.Number)
            .FirstOrDefault(g => g.Count() == Manual.ThreeOfAKindSize);
        var pair = cards
            .GroupBy(c => c.Number)
            .FirstOrDefault(g => g.Count() == Manual.PairSize);
        
        if (tris is null || pair is null) return Manual.StillToCheck;
        var highCardTris = tris.Key;
        var highCardPair = pair.Key;
        return ((float)PkrScore.FullHouse + highCardTris) + (highCardPair / Manual.HighCardDivider);
    }
    
    private static float IsStraightsOrFlush(List<Card> cards)
    {
        var straightNumsAndScore = IsStraight(cards);
        return straightNumsAndScore.Item2 is not null ?
            IsThatStraightFlushOrRoyalFlush(cards, straightNumsAndScore) : IsFlush(cards);
    }
    

    private static (float, List<int>?) IsStraight(List<Card> cards)
    {
        var potentialStraightNums = cards.Select(c => c.Number).Distinct().OrderByDescending(num => num).ToList();

        for (var i = 0; i < Manual.PotentialStraightStart; i++)
        {
            var howStraightShouldBe = Enumerable.Range(potentialStraightNums[i], Manual.StraightSize).Reverse().ToList();
            if(howStraightShouldBe.All(num => potentialStraightNums.Contains(num)))
                return ((float)PkrScore.Straight + potentialStraightNums[i] / Manual.StillToCheck, howStraightShouldBe);
        }
        return (Manual.StillToCheck, null);
    }

    private static float IsThatStraightFlushOrRoyalFlush(List<Card> cards, (float, List<int>?)numsAndScore)
    {
        var matchingCards = cards
            .Where(c => numsAndScore.Item2.Contains(c.Number))
            .GroupBy(c => c.Suit)
            .FirstOrDefault(g => g.Count() == numsAndScore.Item2.Count);

        if (matchingCards is null) return numsAndScore.Item1;
        if (matchingCards.OrderByDescending(c => c.Number).First().Number == Manual.Ace) return (float)PkrScore.StraightFlush;
        return numsAndScore.Item1;
    }
    
    private static float IsFlush(List<Card> cards)
    {
        var flush = cards
            .GroupBy(c => c.Suit)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault(g => g.Count() >= Manual.FlushSize);

        if (flush is null) return Manual.StillToCheck;
        var highCard = flush.MaxBy(c => c.Number);
        return (float)PkrScore.Flush + highCard.Number / Manual.HighCardDivider;
    }
    
    private static float IsThreeOfAKind(List<Card> cards)
    {
        var tris = cards
            .GroupBy(c => c.Number)
            .FirstOrDefault(g => g.Count() >= Manual.ThreeOfAKindSize);

        if (tris is null) return Manual.StillToCheck;
        var highCard = tris.Key;
        return (float)PkrScore.ThreeOfAKind + highCard / Manual.HighCardDivider;
    }
    
    private static float IsPairsOrHighCard(List<Card> cards)
    {
        var pair = cards
            .GroupBy(c => c.Number)
            .OrderByDescending(g => g.Key)
            .FirstOrDefault(g => g.Count() >= Manual.PairSize);

        if (pair is null) return FindHighCard(cards);
        var highCard = pair.Key;
        return FindSecondPair(highCard,cards);
    }

    private static float FindSecondPair(int pairOneNumber, List<Card> cards)
    {
        var pairTwo = cards
            .GroupBy(c => c.Number)
            .OrderByDescending(g => g.Key)
            .FirstOrDefault(g => g.Count() >= Manual.PairSize && g.Key != pairOneNumber);

        if (pairTwo is not null) return (float)PkrScore.TwoPair + pairOneNumber / Manual.HighCardDivider;
        return (float)PkrScore.Pair + pairOneNumber / Manual.HighCardDivider;
    }

    private static float FindHighCard(List<Card> cards)
    {
        return (float)cards.OrderByDescending(c => c.Number).First().Number;
    }
}