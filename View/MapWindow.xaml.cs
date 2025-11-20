using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DMAssistant.View
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        private bool _snapToGrid = false;
        private const int gridSize = 40;
        private NoteBox currentNoteBox;
        private Point startPoint;

        //moving the notebox
        private bool _isDraggingNote = false;
        private Point _dragStartPoint;
        private NoteBox _draggingNote;

        Map _map;


        public MapWindow(Map map)
        {
            _map = map;
            InitializeComponent();
            Ink.StrokeCollected += Ink_StrokeCollected;

            LoadImageData();
        }

        private void SaveCanvas(object sender, RoutedEventArgs e)
        {
            // Save InkCanvas strokes
            using (var ms = new MemoryStream())
            {
                Ink.Strokes.Save(ms);
                _map.StrokeData = ms.ToArray();
            }

            // Save NoteBoxes
            _map.NoteBoxes.Clear();
            foreach (var child in NoteBoxes.Children)
            {
                if (child is NoteBox note)
                {
                    _map.NoteBoxes.Add(new NoteBoxData
                    {
                        X = Canvas.GetLeft(note),
                        Y = Canvas.GetTop(note),
                        Width = note.Width,
                        Height = note.Height,
                        Text = note.Text,
                        Title = note.Title
                    });
                }
            }

            MessageBox.Show("Map saved successfully!");
        }



        private void LoadImageData()
        {
            // Load strokes
            if (_map.StrokeData != null && _map.StrokeData.Length > 0)
            {
                using (var ms = new MemoryStream(_map.StrokeData))
                {
                    Ink.Strokes = new StrokeCollection(ms);
                }
            }

            // Load NoteBoxes
            foreach (var noteData in _map.NoteBoxes)
            {
                var note = new NoteBox(noteData.Width, noteData.Height);
                note.Width = noteData.Width;
                //note.Height = noteData.Height;
                note.Text = noteData.Text;
                note.Title = noteData.Title;
                

                Canvas.SetLeft(note, noteData.X);
                Canvas.SetTop(note, noteData.Y);

                //Capture note box move
                note.MouseLeftButtonDown += NoteBox_MouseLeftButtonDown;
                note.MouseMove += NoteBox_MouseMove;
                note.MouseLeftButtonUp += NoteBox_MouseLeftButtonUp;

                note.RemoveRequested += (s, e) =>
                {
                    NoteBoxes.Children.Remove(note);
                };

                NoteBoxes.Children.Add(note);
            }
        }

        private void EnableGridSnapping(object sender, RoutedEventArgs e)
        {
            Ink.EditingMode = InkCanvasEditingMode.Ink;
            _snapToGrid = true;
        }

        private void DisableGridSnapping(object sender, RoutedEventArgs e)
        {
            Ink.EditingMode = InkCanvasEditingMode.Ink;
            _snapToGrid = false;
        }

        private void EnableErasing(object sender, RoutedEventArgs e)
        {
            Ink.EditingMode = InkCanvasEditingMode.EraseByStroke;
            _snapToGrid = false;
        }


        private void Ink_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            if (!_snapToGrid)
                return;

            Stroke stroke = e.Stroke;

            StylusPointCollection snappedPoints = new StylusPointCollection();

            foreach (var p in stroke.StylusPoints)
            {
                double x = Math.Round(p.X / gridSize) * gridSize;
                double y = Math.Round(p.Y / gridSize) * gridSize;

                snappedPoints.Add(new StylusPoint(x, y));
            }

            stroke.StylusPoints = snappedPoints;
        }

        private void DrawNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Ink.EditingMode = InkCanvasEditingMode.None;
            Ink.MouseLeftButtonDown += InkCanvas_MouseDownForNote;
        }

        private void InkCanvas_MouseDownForNote(object sender, MouseButtonEventArgs e)
        {
            // Translate mouse position to NoteBoxes coordinates
            startPoint = e.GetPosition(NoteBoxes);

            var noteBox = new NoteBox();
            noteBox.Width = 5;  // tiny initial size to be visible
            noteBox.Height = 5;

            //Capture note box move
            noteBox.MouseLeftButtonDown += NoteBox_MouseLeftButtonDown;
            noteBox.MouseMove += NoteBox_MouseMove;
            noteBox.MouseLeftButtonUp += NoteBox_MouseLeftButtonUp;

            // Position the NoteBox
            Canvas.SetLeft(noteBox, startPoint.X);
            Canvas.SetTop(noteBox, startPoint.Y);

            noteBox.RemoveRequested += (s, args) =>
            {
                NoteBoxes.Children.Remove(noteBox);
            };

            NoteBoxes.Children.Add(noteBox);
            currentNoteBox = noteBox;

            // Capture mouse on NoteBoxes so we get all moves, even if cursor leaves
            NoteBoxes.CaptureMouse();
            NoteBoxes.MouseMove += NoteBoxes_MouseMoveForNote;
            NoteBoxes.MouseLeftButtonUp += NoteBoxes_MouseUpForNote;

            e.Handled = true; // prevent InkCanvas from processing it
        }

        private void NoteBoxes_MouseMoveForNote(object sender, MouseEventArgs e)
        {
            if (currentNoteBox == null || e.LeftButton != MouseButtonState.Pressed) return;

            // Translate position to NoteBoxes coordinates
            Point pos = e.GetPosition(NoteBoxes);

            double width = Math.Max(5, Math.Abs(pos.X - startPoint.X));
            double height = Math.Max(5, Math.Abs(pos.Y - startPoint.Y));

            currentNoteBox.Width = width;
            currentNoteBox.Height = height;

            Canvas.SetLeft(currentNoteBox, Math.Min(pos.X, startPoint.X));
            Canvas.SetTop(currentNoteBox, Math.Min(pos.Y, startPoint.Y));
        }

        private void NoteBoxes_MouseUpForNote(object sender, MouseButtonEventArgs e)
        {
            NoteBoxes.MouseMove -= NoteBoxes_MouseMoveForNote;
            NoteBoxes.MouseLeftButtonUp -= NoteBoxes_MouseUpForNote;
            NoteBoxes.ReleaseMouseCapture();
            Ink.MouseLeftButtonDown -= InkCanvas_MouseDownForNote;
            currentNoteBox = null;
        }

        private void NoteBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is TextBox) return;

            _draggingNote = sender as NoteBox;
            if (_draggingNote == null) return;

            _isDraggingNote = true;

            // Record the offset between mouse and top-left corner of the NoteBox
            Point mousePos = e.GetPosition(NoteBoxes);
            _dragStartPoint = new Point(
                mousePos.X - Canvas.GetLeft(_draggingNote),
                mousePos.Y - Canvas.GetTop(_draggingNote)
            );

            _draggingNote.CaptureMouse();
            e.Handled = true;
        }

        private void NoteBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDraggingNote || _draggingNote == null) return;

            Point pos = e.GetPosition(NoteBoxes);

            // Move the NoteBox
            Canvas.SetLeft(_draggingNote, pos.X - _dragStartPoint.X);
            Canvas.SetTop(_draggingNote, pos.Y - _dragStartPoint.Y);
        }

        private void NoteBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDraggingNote || _draggingNote == null) return;

            _draggingNote.ReleaseMouseCapture();
            _draggingNote = null;
            _isDraggingNote = false;
        }


    }
}
