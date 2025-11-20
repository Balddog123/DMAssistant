using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DMAssistant.Model
{
    public class Map : ObservableObject
    {
        private byte[] _strokeData;
        public byte[] StrokeData { get => _strokeData; set
            {
                if (SetProperty(ref _strokeData, value))
                {
                    // Tell UI that ThumbnailData also changed
                    OnPropertyChanged(nameof(ThumbnailData));
                    OnPropertyChanged(nameof(FullThumbnailData));
                }
            }
        }

        private ObservableCollection<NoteBoxData> _noteBoxes;
        public ObservableCollection<NoteBoxData> NoteBoxes { get => _noteBoxes; set => SetProperty(ref _noteBoxes, value); }

        public byte[]? ThumbnailData
        {
            get
            {
                if (StrokeData == null || StrokeData.Length == 0)
                {
                    // Generate a placeholder thumbnail
                    int width = 100;
                    int height = 100;

                    var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                    var dv = new DrawingVisual();

                    using (var dc = dv.RenderOpen())
                    {
                        dc.DrawRectangle(Brushes.LightGray, null, new Rect(0, 0, width, height));
                        dc.DrawLine(new Pen(Brushes.DarkGray, 1), new Point(0, 0), new Point(width, height));
                        dc.DrawLine(new Pen(Brushes.DarkGray, 1), new Point(0, height), new Point(width, 0));
                    }

                    rtb.Render(dv);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));

                    using (var ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        return ms.ToArray();
                    }
                }
                else
                {
                    // Recreate StrokeCollection from stored StrokeData
                    StrokeCollection strokes;
                    using (var ms = new MemoryStream(StrokeData))
                    {
                        strokes = new StrokeCollection(ms);
                    }

                    // Original canvas size (use the actual InkCanvas size)
                    double originalWidth = 2000;
                    double originalHeight = 2000;

                    // Thumbnail size
                    double thumbWidth = 100;
                    double thumbHeight = 100;

                    // Compute scaling factors
                    double scaleX = thumbWidth / originalWidth;
                    double scaleY = thumbHeight / originalHeight;

                    var rtb = new RenderTargetBitmap((int)thumbWidth, (int)thumbHeight, 96, 96, PixelFormats.Pbgra32);

                    var dv = new DrawingVisual();
                    using (var dc = dv.RenderOpen())
                    {
                        // Draw background
                        dc.DrawRectangle(Brushes.LightGray, null, new System.Windows.Rect(0, 0, thumbWidth, thumbHeight));

                        // Apply scale transform so strokes shrink to fit
                        dc.PushTransform(new ScaleTransform(scaleX, scaleY));

                        // Draw strokes
                        strokes.Draw(dc);

                        dc.Pop(); // remove scaling transform
                    }

                    rtb.Render(dv);

                    // Convert to byte[] using PNG
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    using (var ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        return ms.ToArray();
                    }
                }

                
            }
        }
        public byte[]? FullThumbnailData
        {
            get
            {
                if (StrokeData == null || StrokeData.Length == 0)
                {
                    // Generate a placeholder thumbnail
                    int width = 2000;
                    int height = 2000;

                    var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                    var dv = new DrawingVisual();

                    using (var dc = dv.RenderOpen())
                    {
                        dc.DrawRectangle(Brushes.LightGray, null, new Rect(0, 0, width, height));
                        dc.DrawLine(new Pen(Brushes.DarkGray, 1), new Point(0, 0), new Point(width, height));
                        dc.DrawLine(new Pen(Brushes.DarkGray, 1), new Point(0, height), new Point(width, 0));
                    }

                    rtb.Render(dv);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));

                    using (var ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        return ms.ToArray();
                    }
                }
                else
                {
                    // Recreate StrokeCollection from stored StrokeData
                    StrokeCollection strokes;
                    using (var ms = new MemoryStream(StrokeData))
                    {
                        strokes = new StrokeCollection(ms);
                    }

                    // Original canvas size (use the actual InkCanvas size)
                    double originalWidth = 2000;
                    double originalHeight = 2000;

                    var rtb = new RenderTargetBitmap((int)originalWidth, (int)originalHeight, 96, 96, PixelFormats.Pbgra32);

                    var dv = new DrawingVisual();
                    using (var dc = dv.RenderOpen())
                    {
                        // Draw background
                        dc.DrawRectangle(Brushes.LightGray, null, new System.Windows.Rect(0, 0, originalWidth, originalHeight));

                        // Draw strokes
                        strokes.Draw(dc);
                    }

                    rtb.Render(dv);

                    // Convert to byte[] using PNG
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    using (var ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        return ms.ToArray();
                    }
                }


            }
        }

        public Map()
        {            
            NoteBoxes = new ObservableCollection<NoteBoxData>();
        }

    }
}
