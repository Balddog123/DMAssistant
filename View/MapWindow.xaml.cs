using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace DMAssistant.View
{
    public partial class MapWindow : Window
    {
        private bool _snapToGrid = false;
        private const int gridSize = 40;
        private NoteBox currentNoteBox;
        private Point startPoint;

        private bool _isDraggingNote = false;
        private Point _dragStartPoint;
        private NoteBox _draggingNote;

        // Zoom
        private double _zoom = 1.0;
        private const double ZoomStep = 0.1;
        private const double MinZoom = 0.25;
        private const double MaxZoom = 4.0;

        private Map _map;
        private InkLayerData _activeLayer => _map.Layers[0]; // for now, last added layer
        private InkCanvas _activeInkCanvas => _activeLayer != null ? _layerCanvases[_activeLayer] : null;

        public static readonly RoutedCommand IncreaseStrokeSizeCommand = new();
        public static readonly RoutedCommand DecreaseStrokeSizeCommand = new();


        // Dictionary mapping each InkLayerData to its InkCanvas instance
        private Dictionary<InkLayerData, InkCanvas> _layerCanvases = new();

        public MapWindow(Map map)
        {
            _map = map;
            InitializeComponent();

            LoadLayers();
            LoadNoteBoxes();
        }

        #region Layer Management

        /// <summary>
        /// Loads all layers from the map and creates InkCanvas instances dynamically
        /// </summary>
        private void LoadLayers()
        {
            if (_map.Layers.Count == 0)
            {
                _map.Layers.Add(new InkLayerData());
            }

            foreach (var layer in _map.Layers)
            {
                CreateInkCanvasForLayer(layer);
            }

            Debug.WriteLine(_activeLayer == null);
        }
        private void SaveMap(object sender, RoutedEventArgs e)
        {
            if (_map.Layers == null) _map.Layers = new ObservableCollection<InkLayerData>();

            _map.Layers.Clear();

            foreach (var layer in _layerCanvases.Keys)
            {
                var canvas = _layerCanvases[layer];

                using (var ms = new MemoryStream())
                {
                    canvas.Strokes.Save(ms);

                    layer.StrokeData = ms.ToArray(); // save strokes to layer object
                }

                _map.Layers.Add(layer); // add layer to map's list
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
        }

        /// <summary>
        /// Creates an InkCanvas for the given layer and adds it to the container
        /// </summary>
        private void CreateInkCanvasForLayer(InkLayerData layer)
        {
            var ink = new InkCanvas
            {
                Width = 2000,
                Height = 2000,
                Background = Brushes.Transparent,
                IsHitTestVisible = !layer.IsLocked,
                Opacity = layer.Opacity
            };

            if (layer.StrokeData != null && layer.StrokeData.Length > 0)
            {
                using var ms = new MemoryStream(layer.StrokeData);
                ink.Strokes = new StrokeCollection(ms);
            }

            ink.StrokeCollected += (s, e) =>
            {
                if (_snapToGrid)
                {
                    SnapStrokeToGrid(e.Stroke);
                }
                SaveLayerStrokes(layer, ink);
            };

            // Insert into container at correct Z-index (top = last)
            LayerContainer.Children.Add(ink);
            _layerCanvases[layer] = ink;
        }

        /// <summary>
        /// Saves the strokes from an InkCanvas back to the layer
        /// </summary>
        private void SaveLayerStrokes(InkLayerData layer, InkCanvas ink)
        {
            using var ms = new MemoryStream();
            ink.Strokes.Save(ms);
            layer.StrokeData = ms.ToArray();            
        }

        /// <summary>
        /// Snaps a stroke to the grid
        /// </summary>
        private void SnapStrokeToGrid(Stroke stroke)
        {
            var snappedPoints = new StylusPointCollection();
            foreach (var p in stroke.StylusPoints)
            {
                double x = Math.Round(p.X / gridSize) * gridSize;
                double y = Math.Round(p.Y / gridSize) * gridSize;
                snappedPoints.Add(new StylusPoint(x, y));
            }
            stroke.StylusPoints = snappedPoints;
        }

        /// <summary>
        /// Adds a new layer to the map and UI
        /// </summary>
        private InkLayerData AddNewLayer(string name = "New Layer")
        {
            var layer = new InkLayerData { Name = name };
            _map.Layers.Add(layer);
            CreateInkCanvasForLayer(layer);
            return layer;
        }

        /// <summary>
        /// Removes a layer
        /// </summary>
        private void RemoveLayer(InkLayerData layer)
        {
            if (_layerCanvases.TryGetValue(layer, out var ink))
            {
                LayerContainer.Children.Remove(ink);
                _layerCanvases.Remove(layer);
            }
            _map.Layers.Remove(layer);
        }

        /// <summary>
        /// Moves a layer up in Z-order
        /// </summary>
        private void MoveLayerUp(InkLayerData layer)
        {
            if (!_layerCanvases.TryGetValue(layer, out var ink)) return;

            int index = LayerContainer.Children.IndexOf(ink);
            if (index < LayerContainer.Children.Count - 1)
            {
                LayerContainer.Children.RemoveAt(index);
                LayerContainer.Children.Insert(index + 1, ink);

                // Update model ordering
                int modelIndex = _map.Layers.IndexOf(layer);
                _map.Layers.RemoveAt(modelIndex);
                _map.Layers.Insert(modelIndex + 1, layer);
            }
        }

        /// <summary>
        /// Moves a layer down in Z-order
        /// </summary>
        private void MoveLayerDown(InkLayerData layer)
        {
            if (!_layerCanvases.TryGetValue(layer, out var ink)) return;

            int index = LayerContainer.Children.IndexOf(ink);
            if (index > 0)
            {
                LayerContainer.Children.RemoveAt(index);
                LayerContainer.Children.Insert(index - 1, ink);

                int modelIndex = _map.Layers.IndexOf(layer);
                _map.Layers.RemoveAt(modelIndex);
                _map.Layers.Insert(modelIndex - 1, layer);
            }
        }

        /// <summary>
        /// Sets layer visibility
        /// </summary>
        private void SetLayerVisibility(InkLayerData layer, bool visible)
        {
            layer.IsVisible = visible;
            if (_layerCanvases.TryGetValue(layer, out var ink))
            {
                ink.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets layer locked state
        /// </summary>
        private void SetLayerLocked(InkLayerData layer, bool locked)
        {
            layer.IsLocked = locked;
            if (_layerCanvases.TryGetValue(layer, out var ink))
            {
                ink.IsHitTestVisible = !locked;
            }
        }

        /// <summary>
        /// Sets layer opacity
        /// </summary>
        private void SetLayerOpacity(InkLayerData layer, double opacity)
        {
            layer.Opacity = opacity;
            if (_layerCanvases.TryGetValue(layer, out var ink))
            {
                ink.Opacity = opacity;
            }
        }

        #endregion

        #region Zoom Handling

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            _zoom += e.Delta > 0 ? ZoomStep : -ZoomStep;
            _zoom = Math.Clamp(_zoom, MinZoom, MaxZoom);

            ZoomTransform.ScaleX = _zoom;
            ZoomTransform.ScaleY = _zoom;

            e.Handled = true;
        }

        #endregion

        #region NoteBoxes (unchanged)

        private void LoadNoteBoxes()
        {
            foreach (var noteData in _map.NoteBoxes)
            {
                var note = new NoteBox(noteData.Width, noteData.Height)
                {
                    Width = noteData.Width,
                    Text = noteData.Text,
                    Title = noteData.Title
                };

                Canvas.SetLeft(note, noteData.X);
                Canvas.SetTop(note, noteData.Y);

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
        private void InkCanvas_MouseDownForNote(object sender, MouseButtonEventArgs e)
        {
            if (_activeInkCanvas == null) return;
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
            if (_activeInkCanvas == null) return;
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
            if (_activeInkCanvas == null) return;
            NoteBoxes.MouseMove -= NoteBoxes_MouseMoveForNote;
            NoteBoxes.MouseLeftButtonUp -= NoteBoxes_MouseUpForNote;
            NoteBoxes.ReleaseMouseCapture();
            _activeInkCanvas.MouseLeftButtonDown -= InkCanvas_MouseDownForNote;
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

        #endregion
        #region Controls
        private void EnablePointer_Click(object sender, RoutedEventArgs e)
        {
            if (_activeInkCanvas == null) return;
            _activeInkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private void EnableErasing_Click(object sender, RoutedEventArgs e)
        {
            if (_activeInkCanvas == null) return;
            _activeInkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void EnableGridSnapping_Click(object sender, RoutedEventArgs e)
        {
            if (_activeInkCanvas == null) return;
            _activeInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            _snapToGrid = true;
        }

        private void DisableGridSnapping_Click(object sender, RoutedEventArgs e)
        {
            if (_activeInkCanvas == null) return;
            _activeInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            _snapToGrid = false;
        }

        private void ToggleLayers_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void IncreaseStrokeSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_activeInkCanvas == null) return;
            _activeInkCanvas.DefaultDrawingAttributes.Height++;
            _activeInkCanvas.DefaultDrawingAttributes.Width++;
        }
        private void DecreaseStrokeSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_activeInkCanvas == null) return;
            _activeInkCanvas.DefaultDrawingAttributes.Height--;
            _activeInkCanvas.DefaultDrawingAttributes.Width--;
        }
        private void DrawNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeInkCanvas == null) return;
            _activeInkCanvas.EditingMode = InkCanvasEditingMode.None;
            _activeInkCanvas.MouseLeftButtonDown += InkCanvas_MouseDownForNote;
        }
        
        #endregion

    }
}
