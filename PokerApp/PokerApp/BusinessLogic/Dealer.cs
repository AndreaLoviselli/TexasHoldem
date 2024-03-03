
using PokerApp.Model;
using PokerApp.Rules;
using PokerApp.Utils;
using Action = PokerApp.Rules.Action;

namespace PokerApp.BusinessLogic;

public class Dealer //o Table?
{
    public Deck Deck { get; set; }
    public Card[] Board { get; }
    public List<Player> Players { get; }
    public List<Player> ActualPlayers { get; set; }
    public int Pot { get; set; }
    public int LastBet { get; set; } 

    // SI CREA DEALER, MAZZO, GIOCATORI E SI DANNO LE CARTE
    public Dealer( params string[] playerNames)
    {
        this.Deck = new();
        this.Board = new Card[Manual.BoardSize];
        this.Players = new();
        this.ActualPlayers = new();
        this.LastBet = Manual.BigBlind;
        foreach (var name in playerNames)
            this.Players.Add(new Player(name));
        
    }
    public void GiveHands()
    {
        for (int i = 0; i < Manual.DealingRound; i++)
        {
            foreach (Player pl in this.Players)
                pl.Hand.Add(this.Deck.Cards.Pop());
        }

        InizializeActualPlayers();
    }

    private void InizializeActualPlayers()
    {
        this.ActualPlayers = this.Players;
        SetUpBlinds();
    }

    private void SetUpBlinds()
    {
        this.Players[2].PersonalBet = Manual.SmallBlind;
        this.Players[2].Chips -= Manual.SmallBlind;
        this.Players[3].PersonalBet = Manual.BigBlind - 1;
        this.Players[3].Chips -= Manual.BigBlind;
        this.Pot = Manual.BigBlind + Manual.SmallBlind;
    }
    public void AddFlopTurnAndRiver()
    {
        Discard();

        for (int i = 0; i < Manual.BoardSize; i++)
        {
            if (i == Manual.TurnDiscard) Discard();
            if (i == Manual.RiverDiscard) Discard();
            this.Board[i] = this.Deck.Cards.Pop();
        } 
    }
    private void Discard()
    {
        this.Deck.Cards.Pop();
    }
    
    // SI CONTROLLA IL VINCITORE TRA I GIOCATORI CHE NON HANNO FOLDATO
    public void StartScoreChecking()
    {
        foreach (Player pl in this.ActualPlayers)
        {
                pl.AddBoardToHandDesc(this.Board);
                pl.Score = ScoreChecker.Check(pl.HandPlusBoardDesc);
            
        }
    }

    public List<Player> BreakTiesAndPickWinner()
    {
        var maxScore = this.ActualPlayers.Max(p => p.Score);
        
        var maxScorePlayers = this.ActualPlayers
            .OrderByDescending(p => p.Score)
            .TakeWhile(p => p.Score == maxScore)
            .ToList();
        
        if (maxScorePlayers.Count > 1) 
            return TiesBreaker.BreakTie(maxScorePlayers);

        return new List<Player> { maxScorePlayers.First() };
    }
    
    
    
    
    
    
    
    
        //GESTIONE TURNI E CHIPS || LE CHIPS DI PARTENZA SONO PRE IMPOSTATE.
        //I TURNI SEGUONO THIS.PLAYERS INDEX, CHE RUOTA OGNI FINE PARTITA
        public bool NewTurn()
        {
            if (CheckIfAllFolded()) return false;


            foreach (var pl in this.ActualPlayers)
            {
            if (pl.PersonalBet != this.LastBet) AskChoices(pl);
            }

        RemoveFoldedPlayers();
            
            if(ThereIsAnotherTurn()) NewTurn();

            CleanLastBetAndChipsPlayed();
            //Console.WriteLine("\n\n\nFINE TURNO");
            return true;
        }

        public bool CheckIfAllFolded()
        {
            if (this.ActualPlayers.Count == 1)
            {
                return DeclareWinner(this.ActualPlayers.First());
            }

            return false;
        }

        private void RemoveFoldedPlayers()
        {
            this.ActualPlayers = this.ActualPlayers.Where(p => p.HasFolded == false).ToList();
        }
        private bool ThereIsAnotherTurn()
        {
            return this.ActualPlayers.Any(p => p.PersonalBet != this.LastBet);
        }

        private void CleanLastBetAndChipsPlayed()
        {
            this.LastBet = 0;
            foreach (var pl in this.ActualPlayers)
                pl.PersonalBet = 1;
        }

        private void AskChoices(Player pl)
        {
            var flagTurnCanEnd = false;
            do
            {
                Console.WriteLine(pl);
                Console.WriteLine($"PRECEDENT BET: {this.LastBet} ");
                Console.WriteLine($"POT: {this.Pot} ");
                Console.WriteLine("\nSeleziona un'opzione: ");
                Console.WriteLine("0)FOLD 1)CALL 2)CHECK 3)RECALL 4)ALL IN");
                var ch = Console.ReadLine()!;
                var choice = Convert.ToInt32(ch);

                switch (choice)
                {
                    case (int)Action.Fold:
                        pl.HasFolded = true;
                        flagTurnCanEnd = true;
                        break;
                    case (int)Action.Call:
                        flagTurnCanEnd = MakeCall(pl);
                        break;
                    case (int)Action.Check:
                        flagTurnCanEnd = MakeCheck(pl);
                        break;
                    case (int)Action.Raise:
                        flagTurnCanEnd = MakeRaise(pl);
                        break;
                    case (int)Action.AllIn:
                        flagTurnCanEnd = AllIn(pl);
                        break;
                }
            } while (!flagTurnCanEnd);
            Console.Clear();
        }
        
        private bool MakeCall(Player pl)
        {
            if (this.LastBet == 0) MakeCheck(pl);
            if (pl.Chips < this.LastBet && pl.Chips != 0) return AllIn(pl);
            
            var chipsToCall = this.LastBet - pl.PersonalBet;
            pl.Chips -= chipsToCall;
            this.Pot += chipsToCall;
            pl.PersonalBet += chipsToCall;
            
            Console.WriteLine($"{pl.Name} called {chipsToCall}");
            
            return true;
        }

        private bool MakeCheck(Player pl)
        {
            if(pl.PersonalBet == 1 && this.LastBet == 0)
            {
                Console.WriteLine($"{pl.Name} ha checkato");
                pl.PersonalBet = this.LastBet;
                return true;
            }
            return false;
        }

        private bool MakeRaise(Player pl)
        {
            if (pl.Chips < this.LastBet && pl.Chips != 0) return AllIn(pl);
            
            Console.WriteLine($"Quanto vuoi rilanciare? Hai {pl.Chips}");
            string rec = Console.ReadLine()!;
            int recall = Convert.ToInt32(rec);
            this.LastBet = recall;
            pl.Chips -= recall;
            this.Pot += recall;
            pl.PersonalBet = recall;
            
            return true;
        }
        
        private bool AllIn(Player pl)
        {
            if (pl.Chips == 0) return false;
            
            Console.WriteLine($"{pl.Name} ha fatto ALL IN con {pl.Chips}!");
            this.Pot += pl.Chips;
            if(pl.Chips > this.LastBet) this.LastBet = pl.Chips;
            pl.PersonalBet = pl.Chips;
            pl.Chips = 0;
            
            return true;
        }
        
        //WIN LOGIC
        private bool DeclareWinner(Player pl)
        {
            Console.WriteLine($"{pl.Name} ha vinto {this.Pot}!");
            pl.Chips += this.Pot;
            SetupNewGame();
            return true;
        }
        
        public void ShowWinnerAndDividePot(List<Player> winners)
        {
            if (winners.Count > 1)
            {
                Console.WriteLine("Il pot viene diviso, hanno vinto: ");
                Console.WriteLine($"I giocatori vincono {this.Pot / winners.Count}");
                foreach (Player pl in winners)
                {
                    Console.WriteLine(pl.Name);
                    pl.Chips += this.Pot / winners.Count;
                }
                SetupNewGame();
            }
            else
            {
                Console.WriteLine($"Ha vinto {winners[0].Name} il pot : {this.Pot}");
                winners[0].Chips += this.Pot;
                SetupNewGame();
            }
        }
        private void SetupNewGame()
        {
            this.LastBet = Manual.BigBlind + 1;
            this.Pot = 0;
            this.Deck = new();
            foreach (Player pl in this.Players)
            {
                pl.PersonalBet = 0;
            }
            RotatePlayers();
        }
        
        private void RotatePlayers()
        {
            var first = this.Players.First();
            for (var i = 0; i < this.Players.Count - 1; i++)
            {
                this.Players[i] = this.Players[i + 1];
            }
            this.Players[this.Players.Count - 1] = first;
        }
        
        
        
        
        //SHOW FLOP || SHOW TURN || SHOW RIVER
        public void ShowFlop()
        {
            Console.WriteLine(this.Board[0]);
            Console.WriteLine(this.Board[1]);
            Console.WriteLine(this.Board[2]);
        }

        public void ShowTurn()
        {
            Console.WriteLine(this.Board[0]);
            Console.WriteLine(this.Board[1]);
            Console.WriteLine(this.Board[2]);
            Console.WriteLine(this.Board[3]);
        }
        
        public void ShowRiver()
        {
            Console.WriteLine(this.Board[0]);
            Console.WriteLine(this.Board[1]);
            Console.WriteLine(this.Board[2]);
            Console.WriteLine(this.Board[3]);
            Console.WriteLine(this.Board[4]);
        }

        public void ShowNamesAndChips()
        {
            foreach (Player pl in this.Players)
            {
                Console.WriteLine($"{pl.Name} chips: {pl.Chips}");
            }
        }
}
    
