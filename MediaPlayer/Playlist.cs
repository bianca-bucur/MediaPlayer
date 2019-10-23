using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UserControls;

namespace MediaPlayer
{
    public class Playlist
    {
        private ObservableCollection<Media> newFiles = new ObservableCollection<Media>();
        private ObservableCollection<Media> mediaCollection=new ObservableCollection<Media>();
        private VideoDisplay videoDisplay;

        public Playlist(VideoDisplay videoDisplay)
        {
            this.VideoDisplay = videoDisplay;
        }

        public void AddUniqueElements()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";
            fd.Multiselect = true;

            if (fd.ShowDialog() == true)
            {
                foreach (var media in fd.FileNames)
                {
                    NewFiles.Add(new Media() { MediaPath = new Uri(media), MediaName = media.Split('\\').Last() });
                }
            }
            foreach (var media in NewFiles)
            {
                if (!MediaCollection.Contains(media))
                {
                    MediaCollection.Add(media);
                }
            }
            NewFiles.Clear();
        }

        public void PlaySelectedItem(Media selectedItem)
        {
            VideoDisplay.SetSource(selectedItem.MediaPath);
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

        public ObservableCollection<Media> MediaCollection
        {
            get
            {
                return this.mediaCollection;
            }
            set
            {
                this.mediaCollection = value;
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
