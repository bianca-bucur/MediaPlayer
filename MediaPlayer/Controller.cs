using UserControls;

namespace MediaPlayer
{
    public class Controller
    {
        private bool menuVisible;
        private MainWindow window;
        private BottomMenu theBottomMenu;
        private Playlist fullPlaylist;
        private VideoDisplay videoDisplay;

        public Controller(MainWindow window)
        {
            Window = window;
            this.VideoDisplay = new VideoDisplay();
            this.FullPlaylist = new Playlist(this.VideoDisplay);
            Window.sideMenu.Content = new SideMenu(this.VideoDisplay);
            Window.mediaDisplayControl.Content = this.VideoDisplay;
            Window.bottomMenu.Content = new BottomMenu(this.VideoDisplay) {FullPlaylist=this.FullPlaylist, VideoDisplay=this.VideoDisplay };            
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

        public MainWindow Window
        {
            get
            {
                return this.window;
            }
            set
            {
                this.window = value;
            }
        }

        public BottomMenu TheBottomMenu
        {
            get
            {
                return this.theBottomMenu;
            }
            set
            {
                this.theBottomMenu = value;
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
