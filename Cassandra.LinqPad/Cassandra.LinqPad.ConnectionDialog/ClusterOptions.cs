using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Cassandra.LinqPad
{
    public class ClusterOptions : Common.WPF.BindableBase
    {        
        public ClusterOptions(string connectionName)            
        {
            this.ConnectionNodes = new ObservableCollection<NodeState>();
        }

        #region Observable Members 

        #region public string Name {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _inpcName;
        public string Name
        {
            get { return this._inpcName; }
            set
            {
                this.SetProperty(ref this._inpcName, value);
            }
        }
        #endregion

        #region public ObservableCollection<NodeState> CusterConnectionNodes {get; set; }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ObservableCollection<NodeState> _inpcConnectionNodes;
        public ObservableCollection<NodeState> ConnectionNodes
        {
            get { return this._inpcConnectionNodes; }
            set
            {
                this.SetProperty(ref this._inpcConnectionNodes, value);
            }
        }
        #endregion

        #endregion

        #region Public Fields

        public Cassandra.ISession CassandraSession
        {
            get;
            set;
        }

        #endregion

        #region Public/Internal Methods

        public bool AddNodeState(NetworkAddressType addressType, string nodeString, int cqlPort)
        {

            var nodeState = new NodeState(addressType, nodeString, cqlPort);
            var nodeExists = this.ConnectionNodes.Contains(nodeState, new NodeState.Compare());

            if (nodeExists)
            {
                throw new System.Data.DuplicateNameException(string.Format("Node \"{0}\" already exits in the Connection List.", nodeString));
            }

            this.ConnectionNodes.Add(nodeState);

            return true;
        }

        #endregion
    }
}
