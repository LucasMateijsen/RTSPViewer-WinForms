using LibVLCSharp.WinForms;
using System;
using System.Windows.Forms;
using System.Linq;
using RTSPViewer.CustomEventArgs;
using RTSPViewer.AppRegistry;

namespace RTSPViewer.Helpers
{
    public class VideoManager
    {
        private FlowLayoutPanel videoWrapper;
        private VlcWrapper vlcWrapper;
        public bool connected { get; private set; }
        public event EventHandler<ConnectedStatusEventArgs> connectionStatusChanged;
        private bool isMuted = false;

        public VideoManager(FlowLayoutPanel flowPanel)
        {
            videoWrapper = flowPanel;
            vlcWrapper = new VlcWrapper();
        }

        /// <summary>
        /// Connects to all enabled connections
        /// </summary>
        public void Connect()
        {
            SetupUI();
            connected = true;
            connectionStatusChanged?.Invoke(this, new ConnectedStatusEventArgs(connected));
            vlcWrapper.SetMute(isMuted);
        }

        // Disconnects all open connections
        public void Disconnect()
        {
            CleanupUI();
            connected = false;
            connectionStatusChanged?.Invoke(this, new ConnectedStatusEventArgs(connected));
        }

        // Mutes every connection
        public void SetMute(bool mute)
        {
            isMuted = mute;
            vlcWrapper.SetMute(mute);
        }

        /// <summary>
        /// Enables a connection
        /// </summary>
        /// <param name="connection">Connection to be enabled</param>
        public void EnableConnection(Connection connection)
        {
            CreateVideoUI(connection);
        }

        /// <summary>
        /// Disables a connection
        /// </summary>
        /// <param name="connection">Connection to be disabled</param>
        public void DisableConnection(Connection connection)
        {
            CleanupVideoComponent((GroupBox)videoWrapper.Controls[$"groupbox{connection.Name}"]);
        }

        /// <summary>
        /// Resizes all GroupBoxex inside the videowrapper to make them fit (BROKEN)
        /// </summary>
        public void SetGroupBoxesSize()
        {
            for (int i = (videoWrapper.Controls.Count - 1); i >= 0; i--)
            {
                SetGroupboxSize((GroupBox)videoWrapper.Controls[i]);
            }
        }

        private void SetupUI()
        {
            using (var regWrapper = new RegistryWrapper())
            {
                var connections = regWrapper.GetConnections().Where(x => x.Enabled).ToList();
                connections.ForEach(CreateVideoUI);
            }
        }

        private void CreateVideoUI(Connection connection)
        {
            // Create UI Components
            var videoComponent = CreateUIComponents(connection.Name);

            // Add a new mediaplayer to the connectionVideo
            vlcWrapper.AddMediaPlayer(connection.Name);
            videoComponent.MediaPlayer = vlcWrapper.GetMediaPlayer(connection.Name);
            vlcWrapper.ConnectToVideo(connection, videoComponent);
        }

        private VideoView CreateUIComponents(string name)
        {
            // Create the video container
            VideoView connectionVideo = new VideoView();
            connectionVideo.Name = name;
            connectionVideo.Dock = DockStyle.Fill;
            connectionVideo.CreateControl();

            // Create the groupbox
            GroupBox connectionBox = new GroupBox();
            connectionBox.Name = $"groupbox{name}";
            connectionBox.Text = name;
            SetGroupboxSize(connectionBox);
            connectionBox.Controls.Add(connectionVideo);
            connectionBox.CreateControl();

            // Add the groupbox the flow panel
            videoWrapper.Controls.Add(connectionBox);

            return connectionVideo;
        }

        private void SetGroupboxSize(GroupBox connectionBox)
        {
            // Set Size
            if (videoWrapper.Width < videoWrapper.Height)
            {
                // Base size of height
                connectionBox.Width = videoWrapper.Width - 50;
                connectionBox.Height = (int)Math.Floor(connectionBox.Width * 0.5625);
            }
            else
            {
                // Base size of width
                connectionBox.Height = videoWrapper.Height - 50;
                connectionBox.Width = (int)Math.Floor(connectionBox.Height * 1.777);
            }
        }

        private void CleanupUI()
        {
            // Reverse for-loop because we are removing controls
            for (int i = (videoWrapper.Controls.Count - 1); i >= 0; i--)
            {
                var groupbox = (GroupBox)videoWrapper.Controls[i];
                CleanupVideoComponent(groupbox);
            }
        }

        private void CleanupVideoComponent(GroupBox groupbox)
        {
            var videoview = (VideoView)groupbox.Controls[0];
            vlcWrapper.DisconnectRTSPForVideo(videoview.Name, videoview);
            groupbox.Controls.Remove(videoview);
            videoview.Dispose();
            videoWrapper.Controls.Remove(groupbox);
            groupbox.Dispose();
        }
    }
}
