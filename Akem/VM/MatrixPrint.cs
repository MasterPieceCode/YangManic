using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Processing;

namespace Akem.VM
{
    public class MatrixPrintInfo
    {
        public int MatrixRow { get; set; }
        public int MatrixCol { get; set; }
        public PaletteTile[,] MatrixPrint { get; set; }
    }

    public class MatrixPrint
    {
        private readonly int _matrixSize;

        public MatrixPrint(int matrixSize)
        {
            _matrixSize = matrixSize;
        }

        public List<MatrixPrintInfo> PreparePrints(IReadOnlyList<IReadOnlyList<PaletteTile>> tiles)
        {

            var newMatrix = true;
            var rowInd = 0;

            var currentRow = 0;
            var currentColumn = 0;


            var matrixStartRow = 0;
            var matrixStartColumn = 0;

            var matrixRow = 0;
            var matrixCol = 0;

            PaletteTile[,] paletteTiles = null;
            var result = new List<MatrixPrintInfo>();

            while (true)
            {
                var tileRow = tiles[currentRow];
                if (newMatrix)
                {
                    var rowCount = Math.Min(_matrixSize, tiles.Count - currentRow);
                    var colCount = Math.Min(_matrixSize, tileRow.Count - currentColumn);
                    paletteTiles = new PaletteTile[rowCount, colCount];
                    result.Add(new MatrixPrintInfo() { MatrixRow = matrixRow, MatrixCol = matrixCol, MatrixPrint = paletteTiles });
                    rowInd = 0;
                    matrixStartRow = currentRow;
                    matrixStartColumn = currentColumn;
                    newMatrix = false;
                }

                var colInd = 0;

                while (currentColumn < Math.Min(_matrixSize + matrixStartColumn, tileRow.Count))
                {
                    paletteTiles[rowInd, colInd] = tiles[currentRow][currentColumn];

                    colInd++;
                    currentColumn++;
                }

                if (AllTilesProccessed(tiles, currentRow, currentColumn, tileRow))
                {
                    break;
                }

                rowInd++;

                // row is procced for matrix
                if (MatrixRowsFilled(tiles, rowInd, currentRow))
                {
                    newMatrix = true;

                    // if not all colums, then make 
                    if (AnyColumnLeftUnProccessed(currentColumn, tileRow))
                    {
                        currentRow = matrixStartRow;
                        matrixCol++;
                    }
                    else
                    {
                        currentColumn = 0;
                        matrixRow++;
                        matrixCol = 0;
                    }
                }
                else
                {
                    currentRow++;
                    currentColumn = matrixStartColumn;
                }
            }

            return result;
        }

        private static bool AnyColumnLeftUnProccessed(int currentColumn, IReadOnlyList<PaletteTile> tileRow)
        {
            return currentColumn < tileRow.Count - 1;
        }

        private bool MatrixRowsFilled(IReadOnlyList<IReadOnlyList<PaletteTile>> tiles, int rowInd, int currentRow)
        {
            return rowInd == _matrixSize || currentRow == tiles.Count - 1;
        }

        private static bool AllTilesProccessed(IReadOnlyList<IReadOnlyList<PaletteTile>> tiles, int currentRow, int currentColumn, IReadOnlyList<PaletteTile> tileRow)
        {
            return currentRow == tiles.Count - 1 && currentColumn == tileRow.Count;
        }
    }
}