using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Cassandra.LinqPad
{
    public enum NetworkAddressType
    {
        IPV4 = 0,
        IPV6 = 1,
        HostName = 2       
    }

    public enum DialogActions
    {
        Unknown = 0,
        Cancel = 1,
        Next = 2
    }

    public class ConnectionModel : Common.WPF.BindableBase
    {
        public ConnectionModel()
        {
            this.ClusterProfiles = new Common.WPF.ObservableDictionary<string, ClusterOptions>();
            this.NetworkAddress = NetworkAddressType.IPV4;
            this.CQLPort = Properties.Settings.Default.CQLPort;

            this.CurrentClusterProfile = string.Format("New{0}", this._newIndex++);
            this.ClusterProfiles.Add(this.CurrentClusterProfile, new ClusterOptions(this.CurrentClusterProfile));  
        }

    #region Observable Members 

        #region public bool NoPromptOnDelete {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private bool _inpcNoPromptOnDelete;
        public bool NoPromptOnDelete
        {
            get { return this._inpcNoPromptOnDelete; }
            set
            {
                this.SetProperty(ref this._inpcNoPromptOnDelete, value);
            }
        }
        #endregion              

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

        #region public string NodeString {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private string _inpcNodeString;
        public string NodeString
        {
            get { return this._inpcNodeString; }
            set
            {
                this.SetProperty(ref this._inpcNodeString, value);
            }
        }
        #endregion
       
        #region public string CurrentClusterProfile {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private string _inpcCurrentClusterProfile;
        public string CurrentClusterProfile
        {
            get { return this._inpcCurrentClusterProfile; }
            set
            {
                this.SetProperty(ref this._inpcCurrentClusterProfile, value);
            }
        }
        #endregion

        #region public DialogActions DialogAction {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private DialogActions _inpcDialogAction;
        public DialogActions DialogAction
        {
            get { return this._inpcDialogAction; }
            set
            {
                this.SetProperty(ref this._inpcDialogAction, value);
            }
        }
        #endregion

        #region public Common.WPF.ObservableDictionary<string,ClusterOptions> ClusterProfiles {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private Common.WPF.ObservableDictionary<string, ClusterOptions> _inpcClusterProfiles;
        public Common.WPF.ObservableDictionary<string,ClusterOptions> ClusterProfiles
        {
            get { return this._inpcClusterProfiles; }
            set
            {
                this.SetProperty(ref this._inpcClusterProfiles, value);
            }
        }
        #endregion

        /// <summary>
        /// Gets the current Custer profile
        /// </summary>      
        public ObservableCollection<NodeState> CusterConnectionNodes
        {
            get { return this.ClusterProfiles[this.CurrentClusterProfile].ConnectionNodes; }            
        }

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

        #endregion //end of Observable Members

        #region Public Fields

        public Cassandra.ISession CassandraSession
        {
            get { return this.ClusterProfiles[this.CurrentClusterProfile].CassandraSession; }
            private set
            {
                this.ClusterProfiles[this.CurrentClusterProfile].CassandraSession = value;
            }
        }

        #endregion

        #region Public/Internal Methods 

        /// <summary>
        /// Adds a node to the current cluster profile
        /// </summary>
        /// <returns></returns>
        internal bool AddNodeState()
        {
            return this.ClusterProfiles[this.CurrentClusterProfile].AddNodeState(this.NetworkAddress, this.NodeString, this.CQLPort);
        }

    #endregion

    #region Private member Fields 

        private int _newIndex = 0;

    #endregion

    }
}
