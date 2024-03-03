using PokerApp.BusinessLogic;
using PokerApp.Model;
using PokerApp.Utils;

namespace PokerApp;

class Program
{
    static void Main(string[] args)
    {
        //logica di inserimento nomi, numero giocatori, (chips iniziali, small and big blind -> valore default)
        Dealer dealer = new("Marco", "Pietro", "Gianni", "Egidio");
        
        do
        {
            dealer.ShowNamesAndChips();
            bool gameNotEnd = true;
            
            dealer.GiveHands();
            dealer.AddFlopTurnAndRiver();
            
            Console.WriteLine("PRE-FLOP ROUND: ");
            gameNotEnd = dealer.NewTurn();
            
            
            Console.WriteLine("FLOP ROUND: ");
            dealer.ShowFlop();
            if (gameNotEnd)
            {
                gameNotEnd = dealer.NewTurn();
            }
            
            Console.WriteLine("TURN ROUND: ");
            dealer.ShowTurn();
            if (gameNotEnd)
            {
                gameNotEnd = dealer.NewTurn();
            }

            if (gameNotEnd) dealer.NewTurn();
            Console.WriteLine("RIVER ROUND: ");
            dealer.ShowRiver();
            
            dealer.StartScoreChecking();
            
            
            var winner = dealer.BreakTiesAndPickWinner();
            dealer.ShowWinnerAndDividePot(winner);
            

            Console.WriteLine("Digita 0) o la partità proseguirà");
            
            if (Console.ReadLine()! == "0") Environment.Exit(0);
            Console.Clear();
        } while (true);
        
    }
}