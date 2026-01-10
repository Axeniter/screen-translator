using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace Translator.View
{
    public partial class ScreenCaptureOverlay : Window
    {
        private Point _startPoint;
        private bool _isSelecting = false;
        private System.Windows.Shapes.Rectangle _selectionRect;
        private Rectangle _selectedArea;

        public Bitmap CapturedBitmap { get; set; }

        public ScreenCaptureOverlay()
        {
            InitializeComponent();
            Mouse.OverrideCursor = Cursors.Cross;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _startPoint = e.GetPosition(this);
                _isSelecting = true;

                _selectionRect = new System.Windows.Shapes.Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 175, 0, 0))
                };

                Canvas.SetLeft(_selectionRect, _startPoint.X);
                Canvas.SetTop(_selectionRect, _startPoint.Y);
                _selectionRect.Width = 0;
                _selectionRect.Height = 0;

                SelectionCanvas.Children.Add(_selectionRect);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isSelecting) return;

            var currentPoint = e.GetPosition(this);

            double left = Math.Min(_startPoint.X, currentPoint.X);
            double top = Math.Min(_startPoint.Y, currentPoint.Y);
            double width = Math.Abs(currentPoint.X - _startPoint.X);
            double height = Math.Abs(currentPoint.Y - _startPoint.Y);

            Canvas.SetLeft(_selectionRect, left);
            Canvas.SetTop(_selectionRect, top);
            _selectionRect.Width = width;
            _selectionRect.Height = height;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isSelecting) return;

            _isSelecting = false;
            var endPoint = e.GetPosition(this);

            double left = Math.Min(_startPoint.X, endPoint.X);
            double top = Math.Min(_startPoint.Y, endPoint.Y);
            double width = Math.Abs(endPoint.X - _startPoint.X);
            double height = Math.Abs(endPoint.Y - _startPoint.Y);


            if (width < 10 || height < 10)
            {
                MessageBox.Show("Selection area must be at least 10x10 pixels");
                SelectionCanvas.Children.Remove(_selectionRect);
                return;
            }

            var screenLeft = SystemParameters.VirtualScreenLeft;
            var screenTop = SystemParameters.VirtualScreenTop;
            var screenWidth = SystemParameters.VirtualScreenWidth;
            var screenHeight = SystemParameters.VirtualScreenHeight;

            var scaleX = screenWidth / this.ActualWidth;
            var scaleY = screenHeight / this.ActualHeight;

            _selectedArea = new Rectangle(
                (int)((left + screenLeft) * scaleX),
                (int)((top + screenTop) * scaleY),
                (int)(width * scaleX),
                (int)(height * scaleY)
            );

            try
            {
                CapturedBitmap = CaptureScreenArea(_selectedArea);

                if (CapturedBitmap != null)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed capturing screen");
                    SelectionCanvas.Children.Remove(_selectionRect);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing screen: {ex.Message}");
                SelectionCanvas.Children.Remove(_selectionRect);
            }
        }

        private Bitmap? CaptureScreenArea(Rectangle area)
        {
            if (area.Width <= 0 || area.Height <= 0)
                return null;

            try
            {
                var bitmap = new Bitmap(area.Width, area.Height);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(
                        new System.Drawing.Point(area.X, area.Y),
                        System.Drawing.Point.Empty,
                        area.Size
                    );
                }
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Mouse.OverrideCursor = null;
            base.OnClosed(e);
        }
    }
}