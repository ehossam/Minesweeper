namespace Minesweeper.Models
{
    public class Tile
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uiID"> Tile ID </param>
        /// <param name="uiXCoordinate"> X coordinate </param>
        /// <param name="uiYCoordinate"> Y coordinate </param>
        public Tile(uint uiID, uint uiXCoordinate, uint uiYCoordinate)
        {
            ID = uiID;
            XCoordinate = uiXCoordinate;
            YCoordinate = uiYCoordinate;
        }

        /// <summary>
        /// Tile ID
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// X-Coordinate position of the tile in the game board
        /// </summary>
        public uint XCoordinate { get; set; }

        /// <summary>
        /// Y-Coordinate position of the tile in the game board
        /// </summary>
        public uint YCoordinate { get; set; }

        /// <summary>
        /// The number of adjacent mines (in the immediate vicinity)
        /// </summary>
        public uint AdjacentMines { get; set; }

        /// <summary>
        /// A boolean flag to indicate whether the tile is actually a mine
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// A boolean flag to indicate whether the tile is revealed
        /// </summary>
        public bool IsRevealed { get; set; }

        /// <summary>
        /// A boolean flag to indicate whether the tile is flagged
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Put or remove a flag on a tile
        /// </summary>
        public void Flag()
        {
            //! Return if the tile is already revealed
            if (IsRevealed)
                return;

            IsFlagged = !IsFlagged;
        }

        /// <summary>
        /// Reveal the tile
        /// </summary>
        public void Reveal()
        {
            IsRevealed = true;

            //! Mark the tile as not flagged after being revealed
            IsFlagged = false;
        }
    }
}
