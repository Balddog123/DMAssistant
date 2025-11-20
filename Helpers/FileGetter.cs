using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DMAssistant.Helpers
{
    public static class FileGetter
    {
        public static byte[]? LoadImageFromFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Title = "Select an Image"
            };

            if (dialog.ShowDialog() == true)
            {
                byte[] imageBytes = File.ReadAllBytes(dialog.FileName);
                return imageBytes;
            }
            else return null;
        }

        public static byte[]? LoadImageFromClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                var image = Clipboard.GetImage(); // Returns a BitmapSource

                // Convert BitmapSource to byte[]
                byte[] imageBytes;
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                using (var ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    imageBytes = ms.ToArray();
                }

                return imageBytes;
            }
            else
            {
                MessageBox.Show("Clipboard does not contain an image.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }
    }


}
