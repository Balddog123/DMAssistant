using System;
using System.Collections.Generic;
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
    public partial class NoteBox : UserControl
    {
        // This event will let the parent know to remove the NoteBox
        public event RoutedEventHandler RemoveRequested;

        private bool _isResizing = false;
        private Point _resizeStart;
        private string _resizeDirection; // "TopLeft", "BottomRight", etc.
        private double _origWidth, _origHeight, _origLeft, _origTop;

        private bool _isCollapsed = true;

        public NoteBox(double origWidth = double.NaN, double origHeight = double.NaN)
        {
            _origWidth = origWidth;
            _origHeight = origHeight;

            InitializeComponent();

            if (_origHeight != double.NaN)
            {
                NoteTextBox.Visibility = Visibility.Collapsed;
                Height = Double.NaN;
                BottomRightHandle.Visibility = Visibility.Collapsed;
                ToggleCollapseButton.Content = "▼";
            }

            BottomRightHandle.MouseLeftButtonDown += Resize_MouseDown;
            BottomRightHandle.MouseMove += Resize_MouseMove;
            BottomRightHandle.MouseLeftButtonUp += Resize_MouseUp;
        }

        private void ToggleCollapse_Click(object sender, RoutedEventArgs e)
        {
            _isCollapsed = !_isCollapsed;

            NoteTextBox.Visibility = _isCollapsed ? Visibility.Collapsed : Visibility.Visible;
            Height = _isCollapsed ? Double.NaN : _origHeight;
            BottomRightHandle.Visibility = _isCollapsed ? Visibility.Collapsed : Visibility.Visible;
            ToggleCollapseButton.Content = _isCollapsed ? "▼" : "▲";
        }

        private void TitleBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Make title editable on double-click
            TitleBox.IsReadOnly = false;
            TitleBox.Focusable = true;

            TitleBox.Focus();
            TitleBox.CaretIndex = TitleBox.Text.Length;

            // Revert to readonly after losing focus
            TitleBox.LostFocus += (s, args) =>
            {
                TitleBox.IsReadOnly = true;
                TitleBox.Focusable = false;
            };
        }

        private void Resize_MouseDown(object sender, MouseButtonEventArgs e)
        {
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
            get => NoteTextBox.Text;
            set => NoteTextBox.Text = value;
        }

        public string Title
        {
            get => TitleBox.Text;
            set => TitleBox.Text = value;
        }
    }
}
