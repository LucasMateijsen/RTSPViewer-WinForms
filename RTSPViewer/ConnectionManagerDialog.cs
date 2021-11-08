using System;
using System.Windows.Forms;
using RTSPViewer.AppRegistry;

namespace RTSPViewer
{
    public partial class ConnectionManagerDialog : Form
    {
        public ConnectionManagerDialog()
        {
            InitializeComponent();
        }

        private void ConnectionManagerDialog_Load(object sender, EventArgs e)
        {
            // Setup datagrid
            connectionsDataGrid.Columns.Add("connectionName", "Name");
            connectionsDataGrid.Columns[0].Width = (int)Math.Floor(connectionsDataGrid.Width * .15);
            connectionsDataGrid.Columns.Add("connectionUri", "URL");
            connectionsDataGrid.Columns[1].Width = (int)Math.Floor(connectionsDataGrid.Width * .5);
            var enabledColumn = new DataGridViewCheckBoxColumn()
            {
                Name = "connectionEnabled",
                HeaderText = "Enabled",
                Width = (int)Math.Floor(connectionsDataGrid.Width * .15)
            };
            connectionsDataGrid.Columns.Add(enabledColumn);

            // Get info from registry and put them into the datagrid.
            using (var regWrapper = new RegistryWrapper())
            {
                var existingConnections = regWrapper.GetConnections();
                foreach (var connection in existingConnections)
                {
                    connectionsDataGrid.Rows.Add(connection.Name, connection.Url, connection.Enabled);
                }
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var regWrapper = new RegistryWrapper())
            {
                // Get current keys in registry
                var allKeys = regWrapper.GetConnectionKeys();
                // Loop over keys in the datagrid
                for (int i = 0; i < connectionsDataGrid.Rows.Count; i++)
                {
                    // Get values and do null check (for the last row in particular)
                    var row = connectionsDataGrid.Rows[i];
                    var name = row.Cells[0].Value?.ToString();
                    var url = row.Cells[1].Value?.ToString();
                    if (name == null || url == null)
                    {
                        continue;
                    }
                    var enabled = (bool)row.Cells[2].Value;
                    
                    var connection = new Connection(name, url, enabled);

                    // Write the current row to the registry
                    regWrapper.WriteConnectionSetting(connection);

                    // Remove the key from the "known" registry keys
                    if (allKeys.Contains(name))
                    {
                        allKeys.Remove(name);
                    }
                }

                // All keys that we did not hit can be removed from the registry
                allKeys.ForEach(regWrapper.DeleteConnection);

                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
