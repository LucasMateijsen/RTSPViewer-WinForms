using System;
using System.Windows.Forms;
using RTSPViewer.Helpers;
using RTSPViewer.CustomEventArgs;
using RTSPViewer.AppRegistry;

namespace RTSPViewer
{
    public partial class Form1 : Form
    {
        private VideoManager videoManager;

        public Form1()
        {
            InitializeComponent();

            videoManager = new VideoManager(videoWrapper);
            videoManager.connectionStatusChanged += SetConnectionStatus;
            AddConnectionsToMenuBar();
            SetConnectionStatus(null, new ConnectedStatusEventArgs { Connected = false });
        }

        private void AddConnectionsToMenuBar()
        {
            using (var regWrapper = new RegistryWrapper())
            {
                for (int i = currentConnectionsToolStripMenuItem.DropDownItems.Count - 1; i > 0; i--)
                {
                    currentConnectionsToolStripMenuItem.DropDownItems.RemoveAt(i);
                }

                var connections = regWrapper.GetConnections();
                connections.ForEach(connection =>
                {
                    var newItem = new ToolStripMenuItem(connection.Name, null, connectionToolStripMenuItem_Click, connection.Name);
                    newItem.Checked = connection.Enabled;
                    currentConnectionsToolStripMenuItem.DropDownItems.Add(newItem);
                });
            }
        }
        #region EventHandlers

        private void Form1_Resize(object sender, EventArgs e)
        {
            videoManager.SetGroupBoxesSize();
        }

        private void SetConnectionStatus(object sender, ConnectedStatusEventArgs e)
        {
            connectionStatusLabel.Text = e.Connected ? "Connected" : "Disconnected";
            connectToolStripMenuItem.Enabled = !e.Connected;
            disconnectToolStripMenuItem.Enabled = e.Connected;
            SetMuted();
        }

        private void SetMuted()
        {
            mutedStatusLabel.Text = videoManager.connected ? muteToolStripMenuItem.Checked ? "Muted" : "Unmuted" : "";
        }

        #endregion



        #region MenuItems

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            videoManager.Connect();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            videoManager.Disconnect();
        }

        private void muteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            muteToolStripMenuItem.Checked = !muteToolStripMenuItem.Checked;
            videoManager.SetMute(muteToolStripMenuItem.Checked);
            SetMuted();
        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            item.Checked = !item.Checked;
            using (var regWrapper = new RegistryWrapper())
            {
                var connection = regWrapper.GetConnectionForKey(item.Name);
                connection.Enabled = item.Checked;
                regWrapper.WriteConnectionSetting(connection);

                if (connection.Enabled)
                {
                    videoManager.EnableConnection(connection);
                }
                else
                {
                    videoManager.DisableConnection(connection);
                }
            }
        }

        private void manageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var connectionManager = new ConnectionManagerDialog();
            connectionManager.ShowDialog();
            if (connectionManager.DialogResult == DialogResult.OK)
            {
                AddConnectionsToMenuBar();
                videoManager.Disconnect();
                videoManager.Connect();
            }
        }

        #endregion


    }
}
