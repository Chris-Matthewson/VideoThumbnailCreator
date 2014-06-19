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
        private int _width;
        private int _height;
        public videoThumbnail(int width, int height, Uri source)
        {
            _height = height;
            _width = width;
            InitializeComponent();
            this.Width = _width;
            this.Height = _height;
            canvas.Width = _width;
            canvas.Height = _height;
            theImage.Width = _width;
            theImage.Height = _height;

            startLoading(source);
           
        }

        private async void startLoading(Uri source)
        {
            theMedElement.Source = source;
            await Task.Run(() => Thread.Sleep(50));
            
        }

        private async void mediaOpened(object sender, RoutedEventArgs e)
        {
            theMedElement.Volume = 0.0;
            //make a media player
            var player = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };

            //open the media and pause to capture frame
            player.Open(theMedElement.Source);
            player.Pause();

            //set the position
            player.Position = TimeSpan.FromSeconds(3);
            //wait for the position to buffer
            await Task.Run(() => Thread.Sleep(3000));

            //create a rendertarget and a visual
            var rtb = new RenderTargetBitmap(_width, _height, 96, 96,PixelFormats.Pbgra32);
            var dv = new DrawingVisual();

            //draw the video to a rectangle
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawVideo(player, new Rect(0, 0, _width, _height));
            }

            //render the frame
            rtb.Render(dv);

            //create a bitmap frame
            var frame = BitmapFrame.Create(rtb).GetCurrentValueAsFrozen();

            //close the media player, we are done with it
            player.Close();

            //set the thumbnail
            theImage.Source = frame as BitmapFrame;
            canvas.Children.Remove(theMedElement);
        }
    }
}
