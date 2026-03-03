using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DMAssistant.View
{
    /// <summary>
    /// Interaction logic for MapTextBox.xaml
    /// </summary>
    public partial class MapTextBox : UserControl
    {
        // This event will let the parent know to remove the NoteBox
        public event RoutedEventHandler RemoveRequested;

        private bool _isResizing = false;
        private Point _resizeStart;
        private string _resizeDirection; // "TopLeft", "BottomRight", etc.
        private double _origWidth, _origHeight, _origLeft, _origTop;

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(MapTextBox), new PropertyMetadata(false));
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        public MapTextBox(double origWidth = double.NaN, double origHeight = double.NaN)
        {
            _origWidth = origWidth;
            _origHeight = origHeight;

            InitializeComponent();

            BottomRightHandle.MouseLeftButtonDown += Resize_MouseDown;
            BottomRightHandle.MouseMove += Resize_MouseMove;
            BottomRightHandle.MouseLeftButtonUp += Resize_MouseUp;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsSelected = true;
            e.Handled = true; // IMPORTANT
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Make title editable on double-click
            MainTextBox.IsReadOnly = false;
            MainTextBox.Focusable = true;

            MainTextBox.Focus();
            MainTextBox.CaretIndex = MainTextBox.Text.Length;

            // Revert to readonly after losing focus
            MainTextBox.LostFocus += (s, args) =>
            {
                MainTextBox.IsReadOnly = true;
                MainTextBox.Focusable = false;
            };
        }

        private void Resize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Clicked resize!");
            _isResizing = true;
            _resizeStart = e.GetPosition(this.Parent as Canvas);
            _resizeDirection = ((Rectangle)sender).Name; // identify which handle
            _origWidth = this.Width;
            _origHeight = this.Height;
            _origLeft = Canvas.GetLeft(this);
            _origTop = Canvas.GetTop(this);

            ((Rectangle)sender).CaptureMouse();
            e.Handled = true;
        }

        private void Resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isResizing) return;

            var pos = e.GetPosition(this.Parent as Canvas);
            double deltaX = pos.X - _resizeStart.X;
            double deltaY = pos.Y - _resizeStart.Y;

            switch (_resizeDirection)
            {
                case "BottomRightHandle":
                    this.Width = Math.Max(20, _origWidth + deltaX);
                    this.Height = Math.Max(20, _origHeight + deltaY);
                    break;

                case "BottomLeftHandle":
                    this.Width = Math.Max(20, _origWidth - deltaX);
                    this.Height = Math.Max(20, _origHeight + deltaY);
                    Canvas.SetLeft(this, _origLeft + deltaX);
                    break;

                case "TopLeftHandle":
                    this.Width = Math.Max(20, _origWidth - deltaX);
                    this.Height = Math.Max(20, _origHeight - deltaY);
                    Canvas.SetLeft(this, _origLeft + deltaX);
                    Canvas.SetTop(this, _origTop + deltaY);
                    break;
            }
        }

        private void Resize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isResizing = false;
            ((Rectangle)sender).ReleaseMouseCapture();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRequested?.Invoke(this, new RoutedEventArgs());
        }

        public string Text
        {
            get => MainTextBox.Text;
            set => MainTextBox.Text = value;
        }
    }
}
