using Minesweeper.Models.Enums;
using Minesweeper.Models.Constants;
using System.Diagnostics;

namespace Minesweeper.Models
{
    public class GameBoard
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GameBoard()
        {
            Initialize();
        }

        /// <summary>
        /// Width for the game board
        /// </summary>
        public uint Width { get; set; } = GameBoardConstants.DefaultWidth;

        /// <summary>
        /// Height for the game board
        /// </summary>
        public uint Height { get; set; } = GameBoardConstants.DefaultHeight;

        /// <summary>
        /// Number of mines for the game board
        /// </summary>
        public uint NumberOfMines { get; set; } = GameBoardConstants.DefaultNumberOfMines;

        /// <summary>
        /// List of the tiles for the game board
        /// </summary>
        public List<Tile> GameTiles { get; set; } = new List<Tile>();

        /// <summary>
        /// State of the game
        /// </summary>
        public GameState CurrentState { get; set; }

        /// <summary>
        /// Stopwatch used for the game
        /// </summary>
        public Stopwatch Stopwatch { get; set; } = new Stopwatch();

        /// <summary>
        /// Remaining number of mines
        /// </summary>
        public uint RemainingMines 
        {  
            get
            {
                return NumberOfMines - Convert.ToUInt32(GameTiles.Where(tile => tile.IsFlagged).Count());
            } 
        }

        /// <summary>
        /// Reset the board with the specified width and height if specified
        /// </summary>
        /// <param name="uiWidth"> Board width </param>
        /// <param name="uiHeight"> Board height </param>
        public void Reset(uint uiWidth = GameBoardConstants.DefaultWidth, uint uiHeight = GameBoardConstants.DefaultHeight)
        {
            Width = uiWidth;
            Height = uiHeight;

            //! Initialize the board
            Initialize();
        }

        /// <summary>
        /// Make a move on the board for the tile specified in X,Y coordinates
        /// </summary>
        /// <param name="uiXCooridnate"> X coordinate </param>
        /// <param name="uiYCooridnate"> Y coordinate </param>
        public void MakeMove(uint uiXCooridnate, uint uiYCooridnate)
        {
            //! If the game is already finished, don't make the move and return
            if (GameState.Completed == CurrentState || GameState.Failed == CurrentState)
                return;

            //! Make the first move if it's the first
            if (GameState.Initialized == CurrentState)
                MakeFirstMove(uiXCooridnate, uiYCooridnate);

            //! Reveal the tile
            RevealTile(uiXCooridnate, uiYCooridnate);
        }

        /// <summary>
        /// Flag the tile specified by its X and Y coordinates
        /// </summary>
        /// <param name="uiXCooridnate"> X coordinate</param>
        /// <param name="uiYCooridnate"> Y coordinate</param>
        public void FlagTile(uint uiXCooridnate, uint uiYCooridnate)
        {
            //! If the game is already finished, don't make the move and return
            if (GameState.Completed == CurrentState || GameState.Failed == CurrentState)
                return;

            //! Get the tile to flag
            var tileToFlag = GameTiles.Where(tile => tile.XCoordinate == uiXCooridnate && tile.YCoordinate == uiYCooridnate).First();

            //! Check that there are more flags to use 
            if (0 >= RemainingMines && !tileToFlag.IsFlagged)
                return;

            //! Flip the flag on the tile
            tileToFlag.Flag();

            //! Check if the game is already completed by now (putting a flag at the end may be needed to mark the game as completed)
            CheckIfGameCompleted();
        }

        /// <summary>
        /// Initialize the game board
        /// </summary>
        private void Initialize()
        {
            //! 1- The number of mines = 20% width * height
            NumberOfMines = Convert.ToUInt32(Math.Ceiling(Width * Height / 5.0));

            //! 2- Reset the stop watch
            Stopwatch.Reset();

            //! 3- Clear the list
            GameTiles.Clear();

            //! 4- Populate the list with new tiles
            uint tileID = 0;
            for (uint uiWidthIndex = 0; uiWidthIndex < Width; uiWidthIndex++)
            {
                for (uint uiHeightIndex = 0; uiHeightIndex < Height; uiHeightIndex++)
                {
                    GameTiles.Add(new Tile(tileID, uiWidthIndex, uiHeightIndex));
                    ++tileID;
                }
            }

            //! 5- Set the current game state to be initialized
            CurrentState = GameState.Initialized;
        }

        /// <summary>
        /// Make the first move on board (at the beginning of game)
        /// </summary>
        /// <param name="uiXCooridnate"></param>
        /// <param name="uiYCooridnate"></param>
        private void MakeFirstMove(uint uiXCooridnate, uint uiYCooridnate)
        {
            //! 1- Create a random object to be used to mark the mines randomly 
            Random rand = new Random();

            //! 2- Avoid marking the selected tile or any adjacent tiles as mines
            var adjacentTiles = GetAdjacentTiles(uiXCooridnate, uiYCooridnate);
            adjacentTiles.Add(GameTiles.First(tile => tile.XCoordinate == uiXCooridnate && tile.YCoordinate == uiYCooridnate));
            var mineList = GameTiles.Except(adjacentTiles).OrderBy(tile => rand.Next());

            //! 3- Mark some tiles as mines and place them on board
            var mineIDs = mineList.Take(Convert.ToInt32(NumberOfMines)).ToList().Select(tile => tile.ID);
            foreach (var mineID in mineIDs)
                GameTiles.Single(tile => tile.ID == mineID).IsMine = true;

            //! 4- Set the number of mines adjacent tiles for all other normal tiles
            foreach (var normalTile in GameTiles.Where(tile => !tile.IsMine))
            {
                var adjacentTilesToNormalTile = GetAdjacentTiles(normalTile.XCoordinate, normalTile.YCoordinate);
                normalTile.AdjacentMines = Convert.ToUInt32(adjacentTilesToNormalTile.Count(tile => tile.IsMine));
            }

            //! 5- Set the game state to be in progress
            CurrentState = GameState.InProgress;

            //! 6- Start the timer
            Stopwatch.Start();
        }

        /// <summary>
        /// Returns a list of adjacent tiles for a particular tile in the specified X and Y coordinates
        /// </summary>
        /// <param name="uiXCooridnate"> X coordinate </param>
        /// <param name="uiYCooridnate"> Y coordinate </param>
        /// <returns> List of adjacent tiles for the specied tile </returns>
        private List<Tile> GetAdjacentTiles(uint uiXCooridnate, uint uiYCooridnate)
        {
            var adjacentTiles = GameTiles.Where(tile => Convert.ToInt32(tile.XCoordinate) >= (Convert.ToInt32(uiXCooridnate) - 1) && tile.XCoordinate <= (uiXCooridnate + 1)
                                                    && Convert.ToInt32(tile.YCoordinate) >= (Convert.ToInt32(uiYCooridnate) - 1) && tile.YCoordinate <= (uiYCooridnate + 1));
            var currentTile = GameTiles.Where(tile => tile.XCoordinate == uiXCooridnate && tile.YCoordinate == uiYCooridnate);

            return adjacentTiles.Except(currentTile).ToList();
        }

        /// <summary>
        /// Revel the tile specified with the X and Y coordinates
        /// </summary>
        /// <param name="uiXCooridnate"> X coordinate </param>
        /// <param name="uiYCooridnate"> Y coordinate </param>
        private void RevealTile(uint uiXCooridnate, uint uiYCooridnate)
        {
            //! 1- Reveal the tile
            var tileToReveal = GameTiles.First(tile => tile.XCoordinate == uiXCooridnate && tile.YCoordinate == uiYCooridnate);
            if (null == tileToReveal)
                return;

           tileToReveal.Reveal();

            //! 2- If the tile is a mine, call the relevant handler
            if (tileToReveal.IsMine)
            {
                HandleMineRevealed();
                return;
            }

            //! 3- If the tile is a zero, meaning it has no adjacent mines, reveal all its adjacent tiles
            if (0 == tileToReveal.AdjacentMines)
                RevealZeros(uiXCooridnate, uiYCooridnate);

            //! 4- Check if the game is already completed by now
            CheckIfGameCompleted();
        }

        /// <summary>
        /// Recursively reveal all adjacent tiles in case a tile has no adjacent mines
        /// </summary>
        /// <param name="uiXCooridnate"> X coordinate </param>
        /// <param name="uiYCooridnate"> Y coordinate </param>
        private void RevealZeros(uint uiXCooridnate, uint uiYCooridnate)
        {
            //! DFS Approach to reveal all the adjacent tiles
            //! When an adjacent tile gets revealed, it's checked if it's itself a zero, then its adjacent tiles get revealed and also checked, and so on..

            //! If any adjacent tile is alredy revealed, then don't include it in the list
            var adjacentTiles = GetAdjacentTiles(uiXCooridnate, uiYCooridnate).Where(tile => !tile.IsRevealed);
            foreach (var adjacentTile in adjacentTiles)
            {
                //! Mark the tile as revealed
                adjacentTile.Reveal();

                //! Reveal the adjacent tiles if the tile has no mines adjacent to it
                if (0 == adjacentTile.AdjacentMines)
                    RevealZeros(adjacentTile.XCoordinate, adjacentTile.YCoordinate);
            }
        }

        /// <summary>
        /// Mark all mines as revealed
        /// </summary>
        private void RevealAllMines()
        {
            GameTiles.Where(tile => tile.IsMine).ToList().ForEach(tile => tile.IsRevealed = true);
        }

        /// <summary>
        /// Check if the game is completed
        /// </summary>
        private void CheckIfGameCompleted()
        {
            var hiddenGameTiles = GameTiles.Where(tile => !tile.IsRevealed).Select(tile => tile.ID);
            var mineGameTiles = GameTiles.Where(tile => tile.IsMine).Select(tile => tile.ID);
            var mineNonFlaggedGameTiles = GameTiles.Where(tile => tile.IsMine && !tile.IsFlagged).Select(tile => tile.ID);

            //! Return if there exists any hidden tile which is not a mine or if any mine is not yet flagged
            if (hiddenGameTiles.Except(mineGameTiles).Any() || mineNonFlaggedGameTiles.Any())
                return;

            //! Set the game state as completed
            CurrentState = GameState.Completed;

            //! Stop the stopwatch
            Stopwatch.Stop();
        }

        /// <summary>
        /// Handle the action of a mine getting revealed
        /// </summary>
        private void HandleMineRevealed()
        {
            //! 1- Mark the game as failed
            CurrentState = GameState.Failed;

            //! 2- Reveal all the mine tiles
            RevealAllMines();

            //! 3- Stop the stopwatch
            Stopwatch.Stop();
        }
    }
}
