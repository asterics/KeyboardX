using Player.Model;
using System.Collections.Generic;

namespace Player.Conf
{
    /// <summary>
    /// Used for general configuration values. Specific config can be found in extra interfaces.<para />
    /// Description for properties can be found in XML Schema for config file.
    /// </summary>
    public interface GlobalConfig
    {
        string SchemaFileName { get; }

        bool SchemaValidation { get; }

        bool ClickTriggerActive { get; }

        bool ShowMouseChanges { get; }

        bool ButtonPressSynchronous { get; }

        /* net */

        IEnumerable<TcpDestinationModel> TcpDests { get; }

        bool TcpSinkActive { get; }

        int TcpSinkPort { get; }
    }
}
