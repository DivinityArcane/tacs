using tacs.Network;
using tacs.Structures;

namespace tacs.Entities
{
    class Game
    {
        private Client PlayerOne, PlayerTwo;
        private Board GameBoard;
        private Client Turn;

        public Game (Client p1, Client p2)
        {
            PlayerOne = p1;
            PlayerTwo = p2;
            Turn = p1;
            GameBoard = new Board();
        }

        public bool IsTurn (Client c)
        {
            return Turn != null && Turn == c;
        }

        public bool MakeMove (Client c, byte tile)
        {
            TileState state = c == PlayerOne ? TileState.PlayerOne : TileState.PlayerTwo;
            Turn = c == PlayerOne ? PlayerTwo : PlayerOne;
            return GameBoard.Mark(tile, state);
        }

        public bool HasWon (Client c)
        {
            TileState state = c == PlayerOne ? TileState.PlayerOne : TileState.PlayerTwo;
            return GameBoard.HasWon(state);
        }

        public TileState GetState (Client c)
        {
            return c == PlayerOne ? TileState.PlayerOne : c == PlayerTwo ? TileState.PlayerTwo : TileState.UnMarked;
        }

        public bool IsBoardFull ()
        {
            return GameBoard.IsFull();
        }

        public void SendAll (Packet p)
        {
            if (PlayerOne != null && PlayerTwo != null)
            {
                PlayerOne.Send(p);
                PlayerTwo.Send(p);
            }
        }

        public void Winner (Client c)
        {
            Packet p;
            string winner = "nil";
            if (c == PlayerOne)
            {
                p = new Packet("WON");
                PlayerOne.Send(p);

                p = new Packet("LOST");
                PlayerTwo.Send(p);

                winner = PlayerOne.Name;
            }
            else if (c == PlayerTwo)
            {
                p = new Packet("LOST");
                PlayerOne.Send(p);

                p = new Packet("WON");
                PlayerTwo.Send(p);

                winner = PlayerTwo.Name;
            }

            PlayerOne.Game = null;
            PlayerTwo.Game = null;

            PlayerOne.SetState(ClientState.LobbyIdle);
            PlayerTwo.SetState(ClientState.LobbyIdle);

            Tacs.server.Announce(string.Format("The game between {0} and {1} resulted in a win for {2}!", PlayerOne.Name, PlayerTwo.Name, winner));
        }

        public void Forfeit (Client c)
        {
            Packet p;
            string winner = "nil";
            if (c == PlayerOne)
            {
                p = new Packet("LOST");
                PlayerOne.Send(p);

                p = new Packet("WON");
                PlayerTwo.Send(p);

                winner = PlayerTwo.Name;
            }
            else if (c == PlayerTwo)
            {
                p = new Packet("WON");
                PlayerOne.Send(p);

                p = new Packet("LOST");
                PlayerTwo.Send(p);

                winner = PlayerOne.Name;
            }

            PlayerOne.Game = null;
            PlayerTwo.Game = null;

            PlayerOne.SetState(ClientState.LobbyIdle);
            PlayerTwo.SetState(ClientState.LobbyIdle);

            Tacs.server.Announce(string.Format("The game between {0} and {1} resulted in a win for {2} by forfeit!", PlayerOne.Name, PlayerTwo.Name, winner));
        }

        public void Draw ()
        {
            Packet p = new Packet("DRAW");
            SendAll(p);

            PlayerOne.Game = null;
            PlayerTwo.Game = null;

            PlayerOne.SetState(ClientState.LobbyIdle);
            PlayerTwo.SetState(ClientState.LobbyIdle);

            Tacs.server.Announce(string.Format("The game between {0} and {1} resulted in a draw!", PlayerOne.Name, PlayerTwo.Name));
        }

        public void NegotiateTurns (Client c)
        {
            Packet p;
            if (c == PlayerOne)
            {
                p = new Packet("WAIT");
                PlayerOne.Send(p);

                p = new Packet("TURN");
                PlayerTwo.Send(p);
            }
            else if (c == PlayerTwo)
            {
                p = new Packet("TURN");
                PlayerOne.Send(p);

                p = new Packet("WAIT");
                PlayerTwo.Send(p);
            }
        }
    }
}
