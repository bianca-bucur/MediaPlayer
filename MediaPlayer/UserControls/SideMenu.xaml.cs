using MediaPlayer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace UserControls
{
    /// <summary>
    /// Interaction logic for SideMenu.xaml
    /// </summary>
    public partial class SideMenu : UserControl
    {
        private bool menuVisible;
        private Dial dial;
        private VideoDisplay videoDisplay;
        private double angle;
        private int currentFrame=0;
        private double previousAngle=0;
        private bool dialWasClicked = false;

        public SideMenu(VideoDisplay videoDisplay)
        {
            InitializeComponent();
            this.VideoDisplay = videoDisplay;
            Dial = new Dial();
            this.MenuVisible = false;
            this.DialElement.DataContext = Dial;
            this.currentFrame = 0;
        }

        private void ChangeMenuVisibility(object sender, RoutedEventArgs e)
        {
            this.MenuVisible = !this.MenuVisible;
            if (this.MenuVisible == false)
            {
                Storyboard sb = Resources["showMenu"] as Storyboard;
                sb.Begin(this);
            }
            else
            {
                Storyboard sb = Resources["hideMenu"] as Storyboard;
                sb.Begin(this);
            }
        }
        
        private void GetDialPosition(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            dialWasClicked = !dialWasClicked;
            element.CaptureMouse();
        }

        private void ChangeDialPosition(object sender, MouseEventArgs e)
        {
            if (dialWasClicked)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.Angle = Dial.GetAngle(e.GetPosition(DialElement), DialElement.RenderSize);
                    RotateElement.Angle = this.Angle;
                    if (RotateDialForward() || DialOnZeroFwd())
                    {
                        DialValue.Text = (++currentFrame).ToString();
                        previousAngle = (int)(Angle / 360 * 10) % 10;
                        VideoDisplay.NextFrame();
                    }
                    else if ((RotateDialBackwards() || DialOnZeroBack()) && (currentFrame - 1) >= 0)
                    {
                        DialValue.Text = (--currentFrame).ToString();
                        previousAngle = (int)(Angle / 360 * 10) % 10;
                        VideoDisplay.PrevFrame();
                    }
                }
            }
        }

        private bool RotateDialForward()
        {
            if (previousAngle == (int)(Angle / 360 * 10) - 1 && currentFrame % 10 < (int)(Angle / 360 * 10))
                return true;
            return false;
        }

        private bool RotateDialBackwards()
        {
            if (previousAngle == (int)(Angle / 360 * 10) + 1 && currentFrame % 10 > (int)(Angle / 360 * 10))
                return true;
            return false;
        }

        private bool DialOnZeroFwd()
        {
            if ((int)(Angle / 360 * 10) % 10 == 0 && currentFrame % 10 == 9 && previousAngle == 9) return true;
            return false;
        }

        private bool DialOnZeroBack()
        {
            if ((int)(Angle / 360 * 10) % 10 == 9 && currentFrame % 10 == 0 && previousAngle == 0) return true;
            return false;
        }

        private void ReleaseDial(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            dialWasClicked = !dialWasClicked;
            element.ReleaseMouseCapture();
        }

        private void ResetTimer(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                currentFrame = 0;
                DialValue.Text = currentFrame.ToString();
                previousAngle = 0;
                RotateElement.Angle = 0;
            }
        }

        private void PrevFrame(object sender, RoutedEventArgs e)
        {
            VideoDisplay.PrevFrame();
            currentFrame--;
            DialValue.Text = currentFrame.ToString();
        }

        private void NextFrame(object sender, RoutedEventArgs e)
        {
            VideoDisplay.NextFrame();
            currentFrame++;
            DialValue.Text = currentFrame.ToString();
        }

        private void PlayPauseMedia(object sender, RoutedEventArgs e)
        {
            VideoDisplay.mediaDisplay.SpeedRatio = 1 / VideoDisplay.FrameRate;
            VideoDisplay.PlayPauseMediaFunction();
        }

        private void StopMedia(object sender, RoutedEventArgs e)
        {
            VideoDisplay.mediaDisplay.Stop();
        }

        private void EnableZoom(object sender, RoutedEventArgs e)
        {
            VideoDisplay.ZoomingEnabled = !VideoDisplay.ZoomingEnabled;
        }

        private void ResetZoom(object sender, RoutedEventArgs e)
        {
            VideoDisplay.ResetZoom();
        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            VideoDisplay.SaveImage();
        }

        private void DrawWithPen(object sender, RoutedEventArgs e)
        {
            VideoDisplay.DrawingEnabled = !VideoDisplay.DrawingEnabled;
        }

        public Dial Dial
        {
            get
            {
                return this.dial;
            }
            set
            {
                this.dial = value;
            }
        }

        public bool MenuVisible
        {
            get
            {
                return this.menuVisible;
            }
            set
            {
                this.menuVisible = value;
            }
        }

        public double Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                this.angle = value;
            }
        }

        public VideoDisplay VideoDisplay
        {
            get
            {
                return this.videoDisplay;
            }
            set
            {
                this.videoDisplay = value;
            }
        }
    }
}
