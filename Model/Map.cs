using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace DMAssistant.Model
{
    public class Map : ObservableObject
    {
        private ObservableCollection<InkLayerData> _layers = new();
        public ObservableCollection<InkLayerData> Layers
        {
            get => _layers;
            set => SetProperty(ref _layers, value);
        }

        private ObservableCollection<NoteBoxData> _noteBoxes;
        public ObservableCollection<NoteBoxData> NoteBoxes
        {
            get => _noteBoxes;
            set => SetProperty(ref _noteBoxes, value);
        }

        // Convenience properties: composite all visible layers into a thumbnail
        public byte[]? ThumbnailData => RenderThumbnail(100, 100);

        public byte[]? FullThumbnailData => RenderThumbnail(2000, 2000);

        public Map()
        {
            NoteBoxes = new ObservableCollection<NoteBoxData>();
        }

        /// <summary>
        /// Renders all visible layers into a bitmap of the specified size.
        /// </summary>
        private byte[] RenderThumbnail(double targetWidth, double targetHeight)
        {
            if (Layers == null || Layers.Count == 0 || Layers.All(l => l.StrokeData == null || l.StrokeData.Length == 0))
            {
                // Return placeholder image
                var rtb = new RenderTargetBitmap((int)targetWidth, (int)targetHeight, 96, 96, PixelFormats.Pbgra32);
                var dv = new DrawingVisual();
                using (var dc = dv.RenderOpen())
                {
                    dc.DrawRectangle(Brushes.LightGray, null, new Rect(0, 0, targetWidth, targetHeight));
                    dc.DrawLine(new Pen(Brushes.DarkGray, 1), new Point(0, 0), new Point(targetWidth, targetHeight));
                    dc.DrawLine(new Pen(Brushes.DarkGray, 1), new Point(0, targetHeight), new Point(targetWidth, 0));
                }
                rtb.Render(dv);

                using var ms = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(ms);
                return ms.ToArray();
            }

            // Assume original canvas size
            double originalWidth = 2000;
            double originalHeight = 2000;

            // Compute scaling factors for the thumbnail
            double scaleX = targetWidth / originalWidth;
            double scaleY = targetHeight / originalHeight;

            var rtbBitmap = new RenderTargetBitmap((int)targetWidth, (int)targetHeight, 96, 96, PixelFormats.Pbgra32);
            var dvComposite = new DrawingVisual();
            using (var dc = dvComposite.RenderOpen())
            {
                // Background
                dc.DrawRectangle(Brushes.LightGray, null, new Rect(0, 0, targetWidth, targetHeight));

                // Draw all visible layers
                dc.PushTransform(new ScaleTransform(scaleX, scaleY));
                foreach (var layer in Layers)
                {
                    if (!layer.IsVisible || layer.StrokeData == null || layer.StrokeData.Length == 0)
                        continue;

                    var strokes = new StrokeCollection(new MemoryStream(layer.StrokeData));
                    strokes.Draw(dc);
                }
                dc.Pop();
            }

            rtbBitmap.Render(dvComposite);

            using var output = new MemoryStream();
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtbBitmap));
            pngEncoder.Save(output);
            return output.ToArray();
        }
    }

    public class InkLayerData
    {
        public string Name { get; set; } = "Layer";
        public byte[] StrokeData { get; set; } = new byte[0];   // Serialized as Base64
        public bool IsVisible { get; set; } = true;
        public bool IsLocked { get; set; } = false;
        public double Opacity { get; set; } = 1.0;
    }
}
