using MediaPlayer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace UserControls
{
    /// <summary>
    /// Interaction logic for BottomMenu.xaml
    /// </summary>
    public partial class BottomMenu : UserControl
    {
        private bool menuVisible;
        private Playlist fullPlaylist;
        private VideoDisplay videoDisplay;
        
        public BottomMenu(VideoDisplay videoDisplay)
        {
            InitializeComponent();
            this.MenuVisible = false;
            this.VideoDisplay = videoDisplay;
            this.FullPlaylist = new Playlist(this.VideoDisplay);
            this.playlistView.Content = new TextPlaylist(this.VideoDisplay) { FullPlaylist = this.FullPlaylist};
        }

        private void ChangeMenuVisibility(object sender, RoutedEventArgs e)
        {
            this.MenuVisible = !this.MenuVisible;
            if (MenuVisible)
            {
                Storyboard sb = Resources["hideMenu"] as Storyboard;
                sb.Begin(this);
            }
            else
            {
                Storyboard sb = Resources["showMenu"] as Storyboard;
                sb.Begin(this);
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

        public Playlist FullPlaylist
        {
            get
            {
                return this.fullPlaylist;
            }
            set
            {
                this.fullPlaylist = value;
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
