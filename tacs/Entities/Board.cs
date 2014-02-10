using tacs.Structures;

namespace tacs.Entities
{
    class Board
    {
        private TileState[] Tiles;

        public Board ()
        {
            Tiles = new TileState[9];
        }

        public bool Mark (byte tile, TileState state)
        {
            if (Tiles.Length > tile && Tiles[tile] == 0)
            {
                Tiles[tile] = state;
                return true;
            }
            return false;
        }

        public TileState GetState (byte tile)
        {
            if (Tiles.Length > tile) return Tiles[tile];
            return TileState.UnMarked;
        }

        public bool HasWon (TileState player)
        {
            // Row 1, Horizontal
            if (Tiles[0] == player && Tiles[1] == player && Tiles[2] == player) return true;
            // Row 2, Horizontal
            if (Tiles[3] == player && Tiles[4] == player && Tiles[5] == player) return true;
            // Row 3, Horizontal
            if (Tiles[6] == player && Tiles[7] == player && Tiles[8] == player) return true;
            // Column 1, Vertical
            if (Tiles[0] == player && Tiles[3] == player && Tiles[6] == player) return true;
            // Column 2, Vertical
            if (Tiles[1] == player && Tiles[4] == player && Tiles[7] == player) return true;
            // Column 3, Vertical
            if (Tiles[2] == player && Tiles[5] == player && Tiles[8] == player) return true;
            // Corner, Top Left, Diagonal
            if (Tiles[0] == player && Tiles[4] == player && Tiles[8] == player) return true;
            // Corner, Top Right, Diagonal
            if (Tiles[2] == player && Tiles[4] == player && Tiles[6] == player) return true;
            return false;
        }

        public bool IsFull ()
        {
            byte len = (byte)Tiles.Length;
            byte count = 0;
            foreach (byte tile in Tiles) { if ((TileState)tile != TileState.UnMarked) count++; }
            return count == len;
        }
    }
}
