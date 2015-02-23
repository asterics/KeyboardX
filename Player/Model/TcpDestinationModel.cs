using System;

namespace Player.Model
{
    /// <summary>
    /// Defines all properties a TcpDestination should have. Various different implementations exists.
    /// </summary>
    public interface TcpDestinationModel
    {
        /// <summary>An id which TCP actions can reference instead of providing full TCP destination.</summary>
        string Id { get; }

        string Host { get; }

        int Port { get; }
    }
}
