using System;
using System.Linq;

namespace RTSPViewer.AppRegistry
{
    public class Connection
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public bool Enabled { get; set; }
        public string AsCsv { get
            {
                return ConnectionAsCsv();
            }
        }

        public Connection(string name, string url, bool enabled = true)
        {
            if (name.Contains(','))
            {
                throw new ArgumentException("Name cannot contain a comma(,).");
            }
            if (url.Contains(','))
            {
                throw new ArgumentException("Url cannot contain a comma(,).");
            }

            Name = name;
            Url = url;
            Enabled = enabled;
        }

        public Connection(string csv)
        {
            var items = csv.Split(',');
            Name = items[0];
            Url = items[1];

            bool boolParsed;
            var boolParsedSucceeded = bool.TryParse(items[2], out boolParsed);
            Enabled = boolParsedSucceeded ? boolParsed : true;
        }

        private string ConnectionAsCsv()
        {
            return $"{Name},{Url},{Enabled}";
        }
    }
}
