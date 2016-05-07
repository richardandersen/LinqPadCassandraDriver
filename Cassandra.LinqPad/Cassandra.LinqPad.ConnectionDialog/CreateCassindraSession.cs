using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Data;
using System.Net.Sockets;

namespace Cassandra.LinqPad
{
    public class CreateCassindraSession
    {
        public CreateCassindraSession(ClusterOptions clusterOptions)
        {
           
        }

        public ISession Session
        {
            get;
            private set;
        }

        public static bool DetermineIfCassandraPortIsOpen(System.Net.IPAddress checkAddress, int port)
        {
            using (var tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(checkAddress, port);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static System.Net.IPAddress DetermineCassandraNodeAddress(System.Net.IPHostEntry hostEntry, int cqlPort)
        {            
            if (hostEntry != null)
            {
                foreach (var nodeAddress in hostEntry.AddressList)
                {
                    if (DetermineIfCassandraPortIsOpen(nodeAddress, cqlPort))
                    {
                        return nodeAddress;
                    }
                }
            }

            return null;
        }
    }
}
