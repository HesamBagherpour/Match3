namespace HB.Match3
{
    public class Path
    {
        // Boad has different path depend on each EndNodes
        // EndNode is the cell that has provider and no other cells use it as provider
        // StartNode is a cell that has a SpawnerModule

        #region Private Fields

        // private readonly Board _board;

        #endregion

        #region  Constructors

        // public Path(Board board)
        // {
        //     _board = board;
        //     BakeCellProviders();
        // }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update cells directions
        /// </summary>
        private void BakeCellProviders()
        {
            // int width = _board.Width;
            // int height = _board.Height;
        }

        #endregion

        #region Private Methods

        // private void SetHelperCells(int i, int j, Cell cell) { if (_board.HasEmptyCell(i, j)) cell.helperCells.Add((i, j)); }
        // private void AddCellProvider(int i, int j, Cell cell)
        // {
        //     //if (_board.HasEmptyCell(new Point(i, j)) || _board.HasSpawnerCell(new Point(i, j)))
        //     //    cell.providers.Add((i, j));
        // }

        #endregion
    }
}