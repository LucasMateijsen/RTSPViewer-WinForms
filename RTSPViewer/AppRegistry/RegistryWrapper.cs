using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RTSPViewer.AppRegistry
{
    /// <summary>
    /// Wrapper around the Windows Registry. Makes it easier to CRUD keys
    /// </summary>
    public class RegistryWrapper: IDisposable
    {
        private RegistryKey hkcu;
        private RegistryKey companyKey;
        private RegistryKey softwareKey;
        private RegistryKey connectionsKey;

        /// <summary>
        /// Initialize a new instance of this object
        /// </summary>
        public RegistryWrapper()
        {
            hkcu = Registry.CurrentUser.OpenSubKey("Software", true);
            companyKey = hkcu.CreateSubKey("LuMaDynamic", true);
            softwareKey = companyKey.CreateSubKey("RTSPViewer", true);
            connectionsKey = softwareKey.CreateSubKey("Connections", true);
        }

        /// <summary>
        /// Returns the value for the specified key (connection name)
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The value for the given key(connection name)</returns>
        public Connection GetConnectionForKey(string key)
        {
            return new Connection(connectionsKey.GetValue(key).ToString());
        }

        /// <summary>
        /// Returns a List containing all connection objects that exist in the registry
        /// </summary>
        /// <returns>List with all Connection objects found in the registry</returns>
        public List<Connection> GetConnections()
        {
            var result = new List<Connection>();

            GetConnectionKeys().ForEach(key => {
                result.Add(GetConnectionForKey(key));
            });

            return result;
        }

        /// <summary>
        /// Returns a List containing all registry keys representing the names for the connections
        /// </summary>
        /// <returns>List containing key names</returns>
        public List<string> GetConnectionKeys()
        {
            return connectionsKey.GetValueNames().ToList();
        }

        /// <summary>
        /// Writes the connection information to the registry
        /// </summary>
        /// <param name="connection">The Connection object containing the information to be stored</param>
        public void WriteConnectionSetting(Connection connection)
        {
            connectionsKey.SetValue(connection.Name, connection.AsCsv);
        }

        /// <summary>
        /// Deletes the record for the connection from the registrykey
        /// </summary>
        /// <param name="key">The name of the connection to be deleted</param>
        public void DeleteConnection(string key)
        {
            connectionsKey.DeleteValue(key);
        }

        /// <summary>
        /// Dispose of the object
        /// </summary>
        public void Dispose()
        {
            connectionsKey.Dispose();
            softwareKey.Dispose();
            companyKey.Dispose();
            hkcu.Dispose();
        }
    }
}
