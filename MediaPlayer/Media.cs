using System;

namespace MediaPlayer
{
    public class Media : IEquatable<Media>
    {
        private int mediaID;
        private Uri mediaPath;
        private string mediaName;
        private string mediaDuration;
        private double frameRate;

        public Media()
        {

        }

        public Media(string mediaName, Uri mediaPath)
        {
            this.MediaName = mediaName;
            this.MediaPath = mediaPath;
        }

        public bool Equals(Media other)
        {
            return this.MediaPath == other.MediaPath;
        }

        #region properties
        
        public string MediaName
        {
            get
            {
                return this.mediaName;
            }
            set
            {
                this.mediaName = value;
            }
        }

        public int MediaID
        {
            get
            {
                return this.mediaID;
            }
            set
            {
                this.mediaID = value;
            }
        }

        public Uri MediaPath
        {
            get
            {
                return this.mediaPath;
            }
            set
            {
                this.mediaPath = value;
            }
        }

        public string MediaDuration
        {
            get
            {
                return this.mediaDuration;
            }
            set
            {
                this.mediaDuration = value;
            }
        }
        #endregion
    }
}
