using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Media.Media3D;

namespace ChessFiguresWPF;

public partial class MainWindow : Window
{
    private const int SquareSize = 50;
    private King king = new King();
    private Queen queen = new Queen();
    private Bishop bishop = new Bishop();
    private Rook rook = new Rook();
    private Knight knight = new Knight();
    private Pawn pawn = new Pawn();
    private bool isInitialMove;
    private Point _previousClickPosition;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void ChessBoardCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        string selectedColor = (Color.SelectedItem as ComboBoxItem)?.Content.ToString();
        string selectedFigure = (Figure.SelectedItem as ComboBoxItem)?.Content.ToString();

        if (!string.IsNullOrEmpty(selectedColor) && !string.IsNullOrEmpty(selectedFigure))
        {
            Point mousePosition = e.GetPosition(ChessBoardCanvas);

            int column = (int)(mousePosition.X / SquareSize);
            int row = (int)(mousePosition.Y / SquareSize);

            bool positionEmpty = IsPositionEmpty(column, row);

            if (positionEmpty)
            {
                RemoveExistingFigure();

                double figureWidth = 50;
                double figureHeight = 50;

                double figureLeft = column * SquareSize;
                double figureTop = row * SquareSize;

                string imagePath = $"/jpg/{selectedFigure}{selectedColor.Substring(0, 1)}.png";

                Image figureImage = new Image();
                figureImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                figureImage.Width = figureWidth;
                figureImage.Height = figureHeight;

                Canvas.SetLeft(figureImage, figureLeft);
                Canvas.SetTop(figureImage, figureTop);

                if (_previousClickPosition != default(Point))
                {
                    bool isValidMove = false;

                    switch (selectedFigure)
                    {
                        case "King":
                            isValidMove = king.IsValidMove(_previousClickPosition, mousePosition);
                            break;
                        case "Queen":
                            isValidMove = queen.IsValidMove(_previousClickPosition, mousePosition);
                            break;
                        case "Rook":
                            isValidMove = rook.IsValidMove(_previousClickPosition, mousePosition);
                            break;
                        case "Bishop":
                            isValidMove = bishop.IsValidMove(_previousClickPosition, mousePosition);
                            break;
                        case "Knight":
                            isValidMove = knight.IsValidMove(_previousClickPosition, mousePosition);
                            break;
                        case "Pawn":
                            isValidMove = pawn.IsValidMove(_previousClickPosition, mousePosition, isInitialMove);
                            break;
                    }

                    if (!isValidMove)
                    {  
                        
                        MessageBox.Show("Invalid move.", "Invalid Move", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; 
                    }
                }

                ChessBoardCanvas.Children.Add(figureImage);

                _previousClickPosition = mousePosition;
            }
            else
            {
                MessageBox.Show("Position is already occupied.", "Invalid Position", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Please select color and figure.", "Incomplete Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }


    private void RemoveExistingFigure()
    {
        var existingFigure = ChessBoardCanvas.Children.OfType<Image>().FirstOrDefault(image =>
        {
            string imagePath = (image.Source as BitmapImage)?.UriSource?.OriginalString;
            return imagePath != null && imagePath.Contains("/jpg/");
        });

        if (existingFigure != null)
        {
            ChessBoardCanvas.Children.Remove(existingFigure);
        }
    }

    private bool IsPositionEmpty(int column, int row)
    {
        foreach (var child in ChessBoardCanvas.Children)
        {
            if (child is Image image)
            {
                double left = Canvas.GetLeft(image);
                double top = Canvas.GetTop(image);

                int col = (int)(left / SquareSize);
                int r = (int)(top / SquareSize);

                if (col == column && r == row)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        Color.SelectedItem = null;
        Figure.SelectedItem = null;

        var imagesToRemove = ChessBoardCanvas.Children.OfType<Image>().Where(image =>
        {
            string imagePath = (image.Source as BitmapImage)?.UriSource?.OriginalString;
            return imagePath != null && imagePath.Contains("/jpg/");
        }).ToList();

        foreach (var image in imagesToRemove)
        {
            ChessBoardCanvas.Children.Remove(image);
        }

        _previousClickPosition = default(Point);
    }

    private class King
    {
        public bool IsValidMove(Point currentPos, Point newPos)
        {
            int rowDifference = Math.Abs((int)(newPos.Y / SquareSize) - (int)(currentPos.Y / SquareSize));
            int columnDifference = Math.Abs((int)(newPos.X / SquareSize) - (int)(currentPos.X / SquareSize));

            return rowDifference <= 1 && columnDifference <= 1 && (rowDifference != 0 || columnDifference != 0);
        }
    }

    private class Queen
    {
        public bool IsValidMove(Point currentPos, Point newPos)
        {
            int rowDifference = Math.Abs((int)(newPos.Y / SquareSize) - (int)(currentPos.Y / SquareSize));
            int columnDifference = Math.Abs((int)(newPos.X / SquareSize) - (int)(currentPos.X / SquareSize));

            return rowDifference == 0 || columnDifference == 0 || rowDifference == columnDifference;
        }
    }

    private class Rook
    {
        public bool IsValidMove(Point currentPos, Point newPos)
        {
            int rowDifference = Math.Abs((int)(newPos.Y / SquareSize) - (int)(currentPos.Y / SquareSize));
            int columnDifference = Math.Abs((int)(newPos.X / SquareSize) - (int)(currentPos.X / SquareSize));

            return rowDifference == 0 || columnDifference == 0;
        }
    }

    class Bishop
    {
        public bool IsValidMove(Point currentPos, Point newPos)
        {

            int row = Math.Abs((int)(newPos.Y / SquareSize) - (int)(currentPos.Y / SquareSize));
            int col = Math.Abs((int)(newPos.X / SquareSize) - (int)(currentPos.X / SquareSize));

            return row == col;
        }
    }

    class Knight

    {
        public bool IsValidMove(Point currentPos, Point newPos)
        {

            int row = Math.Abs((int)(newPos.Y / SquareSize) - (int)(currentPos.Y / SquareSize));
            int col = Math.Abs((int)(newPos.X / SquareSize) - (int)(currentPos.X / SquareSize));

            return (row == 2 && col == 1) ||
                   (row == 1 && col == 2);
        }
    }
    class Pawn
    {
        public bool IsValidMove(Point currentPos, Point newPos, bool isInitialMove)
        {
            int rowDifference = (int)(newPos.Y / SquareSize) - (int)(currentPos.Y / SquareSize);
            int colDifference = Math.Abs((int)(newPos.X / SquareSize) - (int)(currentPos.X / SquareSize));

            if (newPos.X < 0 || newPos.X >= 8 * SquareSize || newPos.Y < 0 || newPos.Y >= 8 * SquareSize)
            {
                return false;
            }

            if (isInitialMove)
            {
                return (rowDifference == -1 || rowDifference == -2) && colDifference == 0;
            }
            else
            {
                return rowDifference == -1 && colDifference == 0;
            }
        }
    }
}