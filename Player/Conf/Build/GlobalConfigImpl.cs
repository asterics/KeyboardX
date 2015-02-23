using Player.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Player.Conf.Build
{
    class GlobalConfigImpl : GlobalConfig
    {
        public string SchemaFileName { get; set; }

        public bool SchemaValidation { get; set; }

        public bool ClickTriggerActive { get; set; }

        public bool ShowMouseChanges { get; set; }

        public bool ButtonPressSynchronous { get; set; }


        public IEnumerable<TcpDestinationModel> TcpDests { get; set; }

        public bool TcpSinkActive { get; set; }

        public int TcpSinkPort { get; set; }


        public GlobalConfigImpl()
        {
            /* hard coded default values go here */

            SchemaFileName = String.Empty;
            SchemaValidation = true;
            ClickTriggerActive = true;
            ShowMouseChanges = true;
            ButtonPressSynchronous = true;

            TcpDests = Enumerable.Empty<TcpDestinationModel>();
            TcpSinkActive = false;  // if true, port is needed also
        }
    }
}
