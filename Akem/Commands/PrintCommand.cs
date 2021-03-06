﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Akem.VM;
using Processing;

namespace Akem.Commands
{
    public class PrintCommand : ICommand
    {
        private int _pageNumber;
        private List<MatrixPrintInfo> _matrixPrints;
        private readonly MatrixPrint _matrixPrint;
        private bool _mapProccessed;
        private const int MatrixSize = 29;

        public PrintCommand()
        {
            _matrixPrint = new MatrixPrint(MatrixSize);
        }

        public bool CanExecute(object parameter)
        {
            return true;
            /*
            var tiles = (ObservableCollection<ObservableCollection<PaletteTile>>)parameter;
            return tiles != null && tiles.Count > 0;*/
        }

        public void Execute(object parameter)
        {
            var tiles = (ObservableCollection<ObservableCollection<PaletteTile>>) parameter;

            _pageNumber = 0;
            _mapProccessed = false;

            _matrixPrints = _matrixPrint.PreparePrints(tiles);

            CreateAndShowPrintDialog();
        }

        private void CreateAndShowPrintDialog()
        {
            var printDocument = new PrintDocument();
            printDocument.PrintPage += PrintPage;

            var printPreviewDialog = new PrintPreviewDialog
            {
                ClientSize = new Size(400, 300),
                Location = new Point(29, 29),
                MinimumSize = new Size(375, 250),
                UseAntiAlias = true,
                Document = printDocument
            };

            printPreviewDialog.ShowDialog();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            var fontFamily = new FontFamily("Arial");
            var marginBounds = e.MarginBounds;

            if (!_mapProccessed)
            {
                DrawMatrixMap(e, fontFamily, marginBounds);
                _mapProccessed = true;
                e.HasMorePages = true;
                return;
            }

            _pageNumber++;

            var stepSize = Math.Min(marginBounds.Width, marginBounds.Height) / MatrixSize;

            var matrixPrint = _matrixPrints[_pageNumber - 1].MatrixPrint;

            for (var i = 0; i < matrixPrint.GetLength(0); i++)
            {
                for (var j = 0; j < matrixPrint.GetLength(1); j++)
                {
                    DrawMatrixTile(e.Graphics, matrixPrint[i, j], i, j, marginBounds, stepSize, fontFamily);
                }
            }

            const int pageNumberFontSize = 12;

            e.Graphics.DrawString(_pageNumber.ToString(),
                new Font(fontFamily, pageNumberFontSize, FontStyle.Regular),
                new SolidBrush(Color.Black),
                new RectangleF((marginBounds.Right - pageNumberFontSize) / (float)2, marginBounds.Bottom - pageNumberFontSize, marginBounds.Right / (float)2, pageNumberFontSize * (float)1.5));

            if (_pageNumber < _matrixPrints.Count)
            {
                e.HasMorePages = true;
            }
            else
            {
                _pageNumber = 0;
            }
        }

        private void DrawMatrixMap(PrintPageEventArgs e, FontFamily fontFamily, Rectangle marginBounds)
        {
            var rowCount = _matrixPrints.Count/2;

            if (rowCount == 0)
            {
                rowCount++;
            }

            var stepSize = Math.Min(marginBounds.Width, marginBounds.Height) / rowCount;

            foreach (var matrixPrintInfoOrderedByRow in _matrixPrints.GroupBy(mp => mp.MatrixRow))
            {
                foreach (var matrixPrintInfo in matrixPrintInfoOrderedByRow.OrderBy(mp => mp.MatrixCol))
                {
                    var page = _matrixPrints.IndexOf(matrixPrintInfo) + 1;
                    var rectangle = new Rectangle(marginBounds.Left + stepSize*matrixPrintInfo.MatrixCol, marginBounds.Top + stepSize*matrixPrintInfo.MatrixRow, stepSize, stepSize);
                    e.Graphics.DrawRectangle(Pens.Black, rectangle);
                    e.Graphics.DrawString(page.ToString(),
                        new Font(fontFamily, stepSize * (float)0.3, FontStyle.Regular),
                        new SolidBrush(Color.Black),rectangle);
                }
            }
        }

        private static void DrawMatrixTile(Graphics graphics, PaletteTile tile, int tileRow, int tileCol, Rectangle marginBounds, int stepSize, FontFamily fontFamily)
        {
            var matrix = new ColorMatrix {Matrix33 = (float) 0.3};

            var attributes = new ImageAttributes();

            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);    

            // draw tile
           /* graphics.DrawImage(tile.Bitmap,
                new Rectangle(marginBounds.Left + tileCol*stepSize, marginBounds.Top + tileRow*stepSize, stepSize,
                    stepSize), 0, 0, tile.Bitmap.Size.Width, tile.Bitmap.Size.Height, GraphicsUnit.Pixel, attributes);*/

            // draw tile id
            graphics.DrawString(tile.Id.ToString(),
                new Font(fontFamily, (float)(stepSize * 0.3), FontStyle.Regular),
                new SolidBrush(Color.Black),
                new RectangleF(marginBounds.Left + tileCol * stepSize, marginBounds.Top + tileRow * stepSize, stepSize, stepSize));
        }

        public event EventHandler CanExecuteChanged;
    }
}
