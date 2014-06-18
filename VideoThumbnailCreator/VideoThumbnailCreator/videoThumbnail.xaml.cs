using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace VideoThumbnailCreator
{
    /// <summary>
    /// Interaction logic for videoThumbnail.xaml
    /// </summary>
    public partial class videoThumbnail : UserControl
    {
        public videoThumbnail(int width, int height, Uri source)
        {
            InitializeComponent();
            this.Width = width;
            this.Height = height;
            canvas.Width = width;
            canvas.Height = height;
            theImage.Width = width;
            theImage.Height = height;

            startLoading(source);
           
        }

        private async void startLoading(Uri source)
        {
            await Task.Run(() => Thread.Sleep(50));
            theMedElement.Source = source;
        }

        private async void mediaOpened(object sender, RoutedEventArgs e)
        {
            var player = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };

            player.Open(theMedElement.Source);
            player.Pause();

            player.Position = TimeSpan.FromSeconds(3);
            await Task.Run(() => Thread.Sleep(1000));


            var rtb = new RenderTargetBitmap(300, 300, 96, 96, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen())
                dc.DrawVideo(player, new Rect(0, 0, 300, 300));

            rtb.Render(dv);
            var frame = BitmapFrame.Create(rtb).GetCurrentValueAsFrozen();

            var thumbnailFrame = BitmapFrame.Create(frame as BitmapSource).GetCurrentValueAsFrozen();

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(thumbnailFrame as BitmapFrame);
            player.Close();
            theImage.Source = thumbnailFrame as BitmapFrame;
        }
    }
}
