using System.Collections.Generic;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using RTSPViewer.AppRegistry;

namespace RTSPViewer.Helpers
{
    /// <summary>
    /// Wrapper around libVLC that contains an easier way of controlling the videostreams
    /// </summary>
    public class VlcWrapper
    {
        private LibVLC _libVlc;
        private Dictionary<string, MediaPlayer> _mediaPlayers = new Dictionary<string, MediaPlayer>();

        /// <summary>
        /// Initialize a new instance of this object
        /// </summary>
        public VlcWrapper()
        {
            Core.Initialize();
            _libVlc = new LibVLC();
        }

        /// <summary>
        /// Adds a new mediaplayer
        /// </summary>
        /// <param name="name">The name that the mediaplayer should get for internal usage</param>
        /// <returns></returns>
        public MediaPlayer AddMediaPlayer(string name)
        {
            if (_mediaPlayers.ContainsKey(name))
            {
                return _mediaPlayers[name];
            }
            var newMediaPlayer = new MediaPlayer(_libVlc);
            _mediaPlayers.Add(name, newMediaPlayer);

            return _mediaPlayers[name];
        }

        /// <summary>
        /// Get's a specific mediaplayer
        /// </summary>
        /// <param name="name">The name of the mediaplayer you want to receive</param>
        /// <returns></returns>
        public MediaPlayer GetMediaPlayer(string name)
        {
            if (_mediaPlayers.ContainsKey(name) == false)
            {
                throw new KeyNotFoundException($"No media player found with the name {name}");
            }
            return _mediaPlayers[name];
        }

        /// <summary>
        /// Binds a connection to a VideoView component. It uses the name property inside the Connection object to retreive the Mediaplayer
        /// </summary>
        /// <param name="connection">The connection that we want to bind</param>
        /// <param name="targetView">The target VideoView</param>
        public void ConnectToVideo(Connection connection, VideoView targetView)
        {
            var media = new Media(_libVlc, connection.Url, FromType.FromLocation);
            _mediaPlayers[connection.Name].Play(media);
            targetView.MediaPlayer = _mediaPlayers[connection.Name];
        }

        /// <summary>
        /// Unbinds a Connection from a VideoView
        /// </summary>
        /// <param name="name">The name of the connection</param>
        /// <param name="targetView">The target VideoView</param>
        public void DisconnectRTSPForVideo(string name, VideoView targetView)
        {
            _mediaPlayers[name].Stop();
            targetView.MediaPlayer = null;
        }

        /// <summary>
        /// Set's the mute state for all MediaPlayers
        /// </summary>
        /// <param name="mute">The mute state we want to set</param>
        public void SetMute(bool mute)
        {
            foreach (var player in _mediaPlayers)
            {
                player.Value.Mute = mute;
            }
        }
    }
}
