using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Shell;
using MediaToolkit;
using MediaToolkit.Model;
using Microsoft.Win32;
using System.IO;

namespace UserControls
{
    /// <summary>
    /// Interaction logic for VideoDisplay.xaml
    /// </summary>
    public partial class VideoDisplay : UserControl
    {
        private bool isPaused;
        private DispatcherTimer dispatchTimer;
        private bool timerSliderIsDragged;
        private bool showRemainingTime = true;
        private double volume;
        private double frameRate;
        private bool zoomingEnabled = false;
        private Point zoomStartingPoint;
        private Point zoomEndingPoint;
        private bool mouseDown=false;
        public Rectangle selectionBox=new Rectangle();
        private double currentZoomFactor = 1;
        private double zoomFactor;
        private bool drawingEnabled;
        private Point drawingStartingPoint;
        private double prevVolume;
        private bool playingByFrames = false;

        public VideoDisplay()
        {
            InitializeComponent();
            DispatchTimer = new DispatcherTimer();
            DispatchTimer.Interval = TimeSpan.FromSeconds(1);
            DispatchTimer.Tick += Timer_Tick;
            ZoomingEnabled = false;
        }

        #region [Media Functions]

        public void SetSource(Uri path)
        {
            timerSliderIsDragged = false;
            mediaDisplay.Source = path;
            mediaDisplay.Play();
            DispatchTimer.Start();
            IsPaused = false;
            string uriToString = path.ToString().Remove(0, 8);
            ShellFile shellFile = ShellFile.FromFilePath(uriToString);
            var inputFile = new MediaFile { Filename = @uriToString };
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
            }
            FrameRate = (inputFile.Metadata.VideoData.Fps);
            prevVolume = mediaDisplay.Volume;
        }

        public void PlayPauseMedia(object sender, RoutedEventArgs e)
        {            
            mediaDisplay.SpeedRatio = 1;
            PlayPauseMediaFunction();
        }

        public void PlayPauseMediaFunction()
        {
            if (IsPaused)
            {
                mediaDisplay.Play();
                playPauseButton.Content = "Pause";
            }
            else
            {
                mediaDisplay.Pause();
                playPauseButton.Content = "Play";
            }
            IsPaused = !IsPaused;
        }


        private void StopMedia(object sender, RoutedEventArgs e)
        {
            mediaDisplay.Stop();
            IsPaused = true;
        }

        #endregion

        #region [Timer Functions]

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((mediaDisplay.Source != null) && (mediaDisplay.NaturalDuration.HasTimeSpan) && (!timerSliderIsDragged))
            {
                timerSlider.Minimum = 0;
                timerSlider.Maximum = mediaDisplay.NaturalDuration.TimeSpan.TotalSeconds;
                timerSlider.Value = mediaDisplay.Position.TotalSeconds;
            }
        }

        private void TimerDragStarted(object sender, DragStartedEventArgs e)
        {
            this.timerSliderIsDragged = true;
        }

        private void TimerDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.timerSliderIsDragged = false;
            mediaDisplay.Position = TimeSpan.FromSeconds(timerSlider.Value);
            mediaDisplay.Play();
        }

        private void TimerValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentTimeDisplay.Text = TimeSpan.FromSeconds(timerSlider.Value).ToString(@"hh\:mm\:ss");
            mediaDisplay.Position = TimeSpan.FromSeconds(timerSlider.Value);
        }

        private void TimerDragDelta(object sender, DragDeltaEventArgs e)
        {
            mediaDisplay.Pause();
            mediaDisplay.Position = TimeSpan.FromSeconds(timerSlider.Value);
        }

        private void ChangeTimeDisplay(object sender, MouseButtonEventArgs e)
        {
            showRemainingTime = !showRemainingTime;
            if (showRemainingTime)
            {
                currentTimeDisplay.Text = TimeSpan.FromSeconds(timerSlider.Value).ToString(@"hh\:mm\:ss");
            }
            else
            {
                currentTimeDisplay.Text = (TimeSpan.FromSeconds(mediaDisplay.NaturalDuration.TimeSpan.Seconds) - TimeSpan.FromSeconds(timerSlider.Value)).ToString(@"hh\:mm\:ss");
            }
        }


        public void NextFrame()
        {
            mediaDisplay.Position += TimeSpan.FromSeconds(1 / FrameRate);            
        }

        public void PrevFrame()
        {
            mediaDisplay.Position -= TimeSpan.FromSeconds(1 / FrameRate);
        }

        #endregion

        #region [Volume Functions]

        private void ChangeVolume(object sender, MouseWheelEventArgs e)
        {
            if(mediaDisplay.Volume >=0 && e.Delta < 0)
            {
                mediaDisplay.Volume -= 0.1;
            }
            if (mediaDisplay.Volume <=1 && e.Delta > 0)
            {
                mediaDisplay.Volume += 0.1;
            }
        }

        private void MuteMedia(object sender, RoutedEventArgs e)
        {
            if (mediaDisplay.Volume > 0)
            {
                prevVolume = mediaDisplay.Volume;
                mediaDisplay.Volume = 0;
            }
            else if (mediaDisplay.Volume == 0)
            {
                mediaDisplay.Volume = prevVolume;
            }
        }
        #endregion

        #region [Canvas functions]

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ZoomingEnabled)
            {
                ZoomZoneEnd(e);
            }
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ZoomingEnabled)
            {
                ZoomZoneStart(e);
            }
            if (DrawingEnabled)
            {
                DrawZoneStart(e);
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (ZoomingEnabled)
            {
                ZoomZoneDefine(e);
            }
            if (DrawingEnabled)
            {
                DrawZoneDefine(e);
            }
        }

        #endregion

        #region [Zooming Functions]

        private void ZoomZoneStart(MouseButtonEventArgs e)
        {
            if (ZoomingEnabled)
            {
                mouseDown = true;
                zoomStartingPoint = e.GetPosition(mediaCanvas);
                mediaCanvas.CaptureMouse();
                Canvas.SetLeft(selectionBox, zoomStartingPoint.X);
                Canvas.SetTop(selectionBox, zoomStartingPoint.Y);
                selectionBox.Width = 0;
                selectionBox.Height = 0;
                selectionBox.Visibility = Visibility.Visible;
                
            }
        }

        private void ZoomZoneEnd(MouseButtonEventArgs e)
        {
            if (ZoomingEnabled)
            {
                mouseDown = false;
                zoomEndingPoint = e.GetPosition(mediaCanvas);
                mediaCanvas.ReleaseMouseCapture();
                selectionBox.Visibility = Visibility.Visible;
                double canvasHeight = mediaCanvas.ActualHeight;
                double canvasWidth = mediaCanvas.ActualWidth;
                ZoomingEnabled = false;
                if (selectionBox.ActualWidth > selectionBox.ActualHeight)
                {
                    zoomFactor = mediaCanvas.ActualWidth / selectionBox.ActualWidth;
                }
                else
                {
                    zoomFactor = mediaCanvas.ActualHeight / selectionBox.ActualHeight;
                }
                currentZoomFactor = zoomFactor;
                double centerX = Canvas.GetLeft(selectionBox) + selectionBox.ActualWidth / 2 - ((mediaCanvas.ActualWidth - mediaDisplay.ActualWidth) / 2);
                double centerY = Canvas.GetTop(selectionBox) + selectionBox.ActualHeight / 2 - ((mediaCanvas.ActualHeight - mediaDisplay.ActualHeight) / 2);
                ScaleTransform s = new ScaleTransform(currentZoomFactor, currentZoomFactor, centerX, centerY);
                mediaDisplay.RenderTransform = s;
                mediaCanvas.Children.Remove(selectionBox);
            }
        }

        private void ZoomZoneDefine(MouseEventArgs e)
        {
            if (ZoomingEnabled&&mouseDown)
            {
                Point currentRectPoint=e.GetPosition(mediaCanvas);
                if (zoomStartingPoint.X < currentRectPoint.X)
                {
                    Canvas.SetLeft(selectionBox, zoomStartingPoint.X);
                    selectionBox.Width = currentRectPoint.X - zoomStartingPoint.X;
                }
                else
                {
                    Canvas.SetLeft(selectionBox, currentRectPoint.X);
                    selectionBox.Width = zoomStartingPoint.X - currentRectPoint.X;
                }

                if (zoomStartingPoint.Y < currentRectPoint.Y)
                {
                    Canvas.SetTop(selectionBox, zoomStartingPoint.Y);
                    selectionBox.Height = currentRectPoint.Y - zoomStartingPoint.Y;
                }
                else
                {
                    Canvas.SetTop(selectionBox, currentRectPoint.Y);
                    selectionBox.Height = zoomStartingPoint.Y - currentRectPoint.Y;
                }
                if (!mediaCanvas.Children.Contains(selectionBox))
                {                    
                    selectionBox.Stroke = Brushes.White;
                    selectionBox.StrokeThickness = 1.5;
                    mediaCanvas.Children.Add(selectionBox);
                }
            }
        }

        public void ResetZoom()
        {
            mediaDisplay.RenderTransform = new ScaleTransform(1, 1);
            zoomFactor = 1;
            currentZoomFactor = 1;
            int noOfChildren=mediaCanvas.Children.Count;
            mediaCanvas.Children.RemoveRange(1, noOfChildren - 1);
        }

        #endregion

        #region [Drawing on canvas]

        private void DrawZoneStart(MouseButtonEventArgs e)
        {
            drawingStartingPoint = e.GetPosition(mediaCanvas);
            mouseDown = true;
        }

        

        private void DrawZoneDefine(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line a = new Line();
                a.X1 = drawingStartingPoint.X;
                a.X2 = e.GetPosition(mediaCanvas).X;
                a.Y1 = drawingStartingPoint.Y;
                a.Y2 = e.GetPosition(mediaCanvas).Y;
                a.StrokeThickness = 2;
                a.Stroke = Brushes.Red;
                drawingStartingPoint = e.GetPosition(mediaCanvas);
                mediaCanvas.Children.Add(a);
            }
        }

        #endregion

        #region [Save current image]
        public void SaveImage()
        {
            SaveFileDialog s = new SaveFileDialog();
            s.FileName = "Pic";
            s.DefaultExt = ".png";
            s.Filter = "Picture files (.png)|*.png";

            Nullable<bool> result = s.ShowDialog();

            if (result == true)
            {

                Size size = new Size(mediaCanvas.ActualWidth, mediaCanvas.ActualHeight);

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);

                mediaCanvas.Measure(size);
                mediaCanvas.Arrange(new Rect(size));

                renderBitmap.Render(mediaCanvas);

                string filename = s.FileName;
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (FileStream file = File.Create(filename))
                {
                    encoder.Save(file);
                }
            }
        }

        #endregion
        
        #region [Properties]

        public bool IsPaused
        {
            get
            {
                return this.isPaused;
            }
            set
            {
                this.isPaused = value;
            }
        }

        public DispatcherTimer DispatchTimer
        {
            get
            {
                return this.dispatchTimer;
            }
            set
            {
                this.dispatchTimer = value;
            }
        }

        public double Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                this.volume = value;
            }
        }

        public double FrameRate
        {
            get
            {
                return this.frameRate;
            }
            set
            {
                this.frameRate = value;
            }
        }

        public bool ZoomingEnabled
        {
            get
            {
                return this.zoomingEnabled;
            }
            set
            {
                this.zoomingEnabled = value;
            }
        }

        public bool DrawingEnabled
        {
            get
            {
                return this.drawingEnabled;
            }
            set
            {
                this.drawingEnabled = value;
            }
        }




        #endregion

        
    }
}
