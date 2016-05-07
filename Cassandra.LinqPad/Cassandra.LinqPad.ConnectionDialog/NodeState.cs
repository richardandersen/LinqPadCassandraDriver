using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Cassandra.LinqPad
{
    public class NodeState : Common.WPF.BindableBase
    {
        private Task _resolutionTask;
      
        public NodeState(NetworkAddressType addressType, string nodeString, int cqlPort)
        {

            nodeString = nodeString.Trim();
            this.CQLPort = cqlPort == 0 ? Properties.Settings.Default.CQLPort : cqlPort;

            switch (addressType)
            {              
                case NetworkAddressType.HostName:
                    this.HostName = nodeString;
                    break;
                case NetworkAddressType.IPV6:
                case NetworkAddressType.IPV4:
                    this.IPAdressString = nodeString;
                    break;
                default:
                    break;
            }
            this.NetworkAddress = addressType;
            this.HostNameEntered = nodeString;
            this.ResolutionStatus = Status.Unknown;
            this.ServerStatus = Status.Unknown;
            this.NodeEntry = null;

            this._resolutionTask = Task.Factory.StartNew(() =>
                                                {
                                                    this.ResolutionStatus = Status.Running;

                                                    try
                                                    {
                                                        this.NodeEntry = System.Net.Dns.GetHostEntry(nodeString);
                                                        this.ResolutionStatus = Status.Sucessful;
                                                    }
                                                    catch (System.Exception ex)
                                                    {
                                                        this.Exception = ex;
                                                        this.ResolutionStatus = Status.Error;
                                                    }
                                                })
                                        .ContinueWith(taskAction =>
                                                        {
                                                            try
                                                            {                                                                
                                                                if (this.NodeEntry != null)                                                                
                                                                {
                                                                    this.ServerStatus = Status.Running;
                                                                    if (CreateCassindraSession.DetermineCassandraNodeAddress(this.NodeEntry, cqlPort) == null)
                                                                    {
                                                                        throw new System.IO.IOException(string.Format("Could not find Open CQL Port {0} on any network Interfaces for HostName \"{1}\". Tried Interfaces {{{2}}}",
                                                                                                                    cqlPort,
                                                                                                                    this.HostNameEntered,
                                                                                                                    string.Join(", ", this.NodeEntry.AddressList.Select(address => address.ToString()))));
                                                                    }
                                                                    this.ServerStatus = Status.Sucessful;
                                                                }
                                                                else
                                                                {
                                                                    if (this.NetworkAddress != NetworkAddressType.HostName
                                                                            && !string.IsNullOrEmpty(this.HostNameEntered))
                                                                    {
                                                                        var parsedAdress = System.Net.IPAddress.Parse(this.HostNameEntered);

                                                                       if (CreateCassindraSession.DetermineIfCassandraPortIsOpen(parsedAdress, cqlPort))
                                                                        {
                                                                            this.ServerStatus = Status.Sucessful;
                                                                        }
                                                                        else
                                                                        {
                                                                            throw new System.IO.IOException(string.Format("Could not find Open CQL Port {0} on any network Interfaces for HostName \"{1}\".",
                                                                                                                        cqlPort,
                                                                                                                        this.HostNameEntered));
                                                                        }                                                                        
                                                                    }
                                                                    else
                                                                    {
                                                                        this.ServerStatus = Status.Unknown;
                                                                    }
                                                                }
                                                            }
                                                            catch (System.Exception ex)
                                                            {
                                                                this.Exception = ex;
                                                                this.ServerStatus = Status.Error;
                                                            }
                                                        });
            
        }

        #region public NetworkAddressType NetworkAddress {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private NetworkAddressType _inpcNetworkAddress;
        public NetworkAddressType NetworkAddress
        {
            get { return this._inpcNetworkAddress; }
            set
            {
                this.SetProperty(ref this._inpcNetworkAddress, value);                
            }
        }
        #endregion

        #region public System.Net.IPHostEntry NodeEntry {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private System.Net.IPHostEntry _inpcNodeEntry;
        public System.Net.IPHostEntry NodeEntry
        {
            get { return this._inpcNodeEntry; }
            set
            {
                this.SetProperty(value,
                                    () => this._inpcNodeEntry,
                                    (newValue) =>
                                    {
                                        this._inpcNodeEntry = newValue;

                                        if (newValue != null)
                                        {
                                            this.HostName = newValue.HostName;
                                            this.IPAdressString = string.Join(", ", value.AddressList.Select(address => address.ToString()));
                                        }
                                        this.ResolutionStatus = Status.Sucessful;
                                    });
            }
        }
        #endregion

        #region public string HostName {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private string _inpcHostName;
        public string HostName
        {
            get { return this._inpcHostName; }
            private set
            {
                this.SetProperty(ref this._inpcHostName, value);                
            }
        }
        #endregion

        #region public string HostNameEntered {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private string _inpcHostNameEntered;
        public string HostNameEntered
        {
            get { return this._inpcHostNameEntered; }
            set
            {
                this.SetProperty(ref this._inpcHostNameEntered, value);
            }
        }
        #endregion

        #region public string IPAdressString {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private string _inpcIPAdressString;
        public string IPAdressString
        {
            get { return this._inpcIPAdressString; }
            private set
            {
                this.SetProperty(ref this._inpcIPAdressString, value);
            }
        }
        #endregion

        #region public int CQLPort {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private int _inpcCQLPort;
        public int CQLPort
        {
            get { return this._inpcCQLPort; }
            set
            {
                this.SetProperty(ref this._inpcCQLPort, value);
            }
        }
        #endregion

        public enum Status
        {
            Unknown = 0,
            Error = 1,
            Running = 2,
            Up = 3,
            Down = 4,
            Sucessful = 5
        }

        #region public Status ResolutionStatus {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private Status _inpcResolutionStatus;
        public Status ResolutionStatus
        {
            get { return this._inpcResolutionStatus; }
            private set
            {
                this.SetProperty(ref this._inpcResolutionStatus, value);
            }
        }
        #endregion

        #region public Status ServerStatus {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private Status _inpcServerStatus;
        public Status ServerStatus
        {
            get { return this._inpcServerStatus; }
            private set
            {
                this.SetProperty(ref this._inpcServerStatus, value);
            }
        }
        #endregion

        #region public Exception Exception {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private Exception _inpcException;
        public Exception Exception
        {
            get { return this._inpcException; }
            set
            {
                this.SetProperty(ref this._inpcException, value);
            }
        }
        #endregion

        public class Compare : IEqualityComparer<NodeState>
        {
            public bool Equals(NodeState x, NodeState y)
            {
                if (x.NodeEntry != null && y.NodeEntry != null)
                {
                    return x.NodeEntry.HostName == y.NodeEntry.HostName;
                }
                else if (x.NodeEntry == null && y.NodeEntry == null)
                {
                    if (x.NetworkAddress == y.NetworkAddress)
                    {
                        if (x.NetworkAddress == NetworkAddressType.HostName)
                        {
                            return x.HostName == y.HostName;
                        }

                        return x.IPAdressString == y.IPAdressString;
                    }

                    return false;
                }
                else if(x.NodeEntry == null)
                {
                    if (x.NetworkAddress == NetworkAddressType.HostName)
                    {
                        return NameMatch(y.NodeEntry.HostName, x.HostName)
                                || MatchNames(y.NodeEntry.Aliases, x.HostName);
                    }

                    if (y.NodeEntry.AddressList != null)
                    {
                        foreach (var address in y.NodeEntry.AddressList)
                        {
                            if (NameMatch(address.ToString(), x.IPAdressString, false))
                            {
                                return true;
                            }
                        }
                    }
                    
                    return false;
                }

                return this.Equals(y, x);
            }
            public int GetHashCode(NodeState codeh)
            {
                return 0;
            }

            private static bool NameMatch(string fullName, string testName, bool dnsName = true)
            {
                if (fullName.Length == testName.Length)
                {
                    return fullName.Equals(testName, StringComparison.InvariantCultureIgnoreCase);
                }

                int nIndex = 0;

                for (nIndex = 0; nIndex < Math.Min(fullName.Length, testName.Length); ++nIndex)
                {
                    if (char.ToLower(fullName[nIndex]) != char.ToLower(testName[nIndex]))
                    {
                        return false;
                    }
                }                

                if ((nIndex < fullName.Length && ((dnsName && fullName[nIndex] == '.') || fullName[nIndex] == '%'))
                        || (nIndex < testName.Length && ((dnsName && testName[nIndex] == '.') || testName[nIndex] == '%')))
                {
                    return true;
                }
                   
                return false; 
            }

            private static bool MatchNames(string[] fullNames, string testName)
            {
                if(fullNames == null)
                {
                    return false;
                }

                foreach (var item in fullNames)
                {
                    if (NameMatch(item, testName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
