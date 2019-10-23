using MediaPlayer;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UserControls
{
    /// <summary>
    /// Interaction logic for TextPlaylist.xaml
    /// </summary>
    public partial class TextPlaylist : UserControl
    {
        private ObservableCollection<Media> newFiles=new ObservableCollection<Media>();
        private Playlist fullPlaylist;
        private VideoDisplay videoDisplay;

        public TextPlaylist(VideoDisplay videoDisplay)
        {
            InitializeComponent();
            this.VideoDisplay = videoDisplay;
            this.FullPlaylist = new Playlist(this.VideoDisplay);
        }

        private void AddFiles(object sender, RoutedEventArgs e)
        {
            
            FullPlaylist.AddUniqueElements();
            this.mediaPlaylist.ItemsSource = FullPlaylist.MediaCollection;
        }

        private void AddFolder(object sender, RoutedEventArgs e)
        {

        }

        private void OpenContextMenu(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ContextMenu contextMenu = btn.ContextMenu;
            contextMenu.PlacementTarget = btn;
            contextMenu.IsOpen = true;
        }

        private void PlaySelectedMedia(object sender, MouseButtonEventArgs e)
        {
            FullPlaylist.PlaySelectedItem((Media)mediaPlaylist.SelectedItem);
        }

        #region properties

        public ObservableCollection<Media> NewFiles
        {
            get
            {
                return this.newFiles;
            }
            set
            {
                this.newFiles = value;
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
        #endregion

    }
}
