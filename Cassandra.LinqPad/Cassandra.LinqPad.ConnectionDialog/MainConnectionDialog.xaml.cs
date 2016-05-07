using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Infragistics.Controls.Interactions;
using Common.WPF;
using Infragistics.Controls.Editors;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows;

namespace Cassandra.LinqPad
{
    /// <summary>
    /// Interaction logic for MainConnectionDialog.xaml
    /// </summary>
    public partial class MainConnectionDialog : Window
    {
        private Common.WPF.WaitCursor _waitCursor;

        public MainConnectionDialog()
        {
            InitializeComponent();

            this._waitCursor = new Common.WPF.WaitCursor(this, Common.Patterns.WaitCursorModes.GUI);
        }

        public ConnectionModel GetDataContextForConnectionPane()
        {
            return this.ConnectionGridPane.DataContext as ConnectionModel;
        }

        public Cassandra.ISession CassandraSession
        {
            get;
            private set;
        }

        public string CassandraKeySpace
        {
            get;
            private set;
        }

        private void buttonAddToConnectionList_Click(object sender, RoutedEventArgs e)
        {
            using (this._waitCursor.UsingCreate())
            {
                var dataContext = GetDataContextForConnectionPane();

                if (dataContext != null
                        && !string.IsNullOrEmpty(dataContext.NodeString))
                {
                    try
                    {
                        e.Handled = dataContext.AddNodeState();

                        if (e.Handled)
                        {
                            dataContext.NodeString = null;
                            dataContext.NoPromptOnDelete = false;
                            this.ResetHostNodeIcon();
                        }
                        else
                        {
                            this.SetHostNodeErrorIcon("Node Host was not added to the collection List");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        e.Handled = false;

                        this.SetHostNodeErrorIcon(ex);
                    }
                }
            }
        }

        private void SetHostNodeErrorIcon(string errorMessage)
        {
            this.imageNodeNotification.Source = new BitmapImage(new Uri(Properties.Settings.Default.ErrorURIIcon16));
            this.imageNodeNotification.ToolTip = errorMessage;
        }

        private void SetHostNodeErrorIcon(Exception ex)
        {
            this.SetHostNodeErrorIcon(string.Format("{0}: {1}", ex.GetType().Name, ex.Message));
        }

        private void ResetHostNodeIcon()
        {
            this.imageNodeNotification.Source = new BitmapImage(new Uri(Properties.Settings.Default.InfoURIIcon16));
            this.imageNodeNotification.ToolTip = Properties.Settings.Default.HostNodeInfoToolTip;
        }

        private void DeleteNode_Button_Click(object sender, RoutedEventArgs e)
        {

            DataRecordPresenter dr = Utilities.GetAncestorFromType(sender as DependencyObject,
                                                                    typeof(DataRecordPresenter), false) as DataRecordPresenter;

            dr.Record.IsSelected = true;
            this.ConnectionNodesGrid.ExecuteCommand(DataPresenterCommands.DeleteSelectedDataRecords);
        }

        private void EditNode_Button_Click(object sender, RoutedEventArgs e)
        {

            var dataItem = ((Utilities.GetAncestorFromType(sender as DependencyObject,
                                                            typeof(DataRecordPresenter),
                                                            false) as DataRecordPresenter)?.Record as DataRecord)?.DataItem as NodeState;

            if (dataItem == null)
            {
                System.Windows.MessageBox.Show("Could not obtain access to the selected data record for Edit");
            }
            else
            {
                var dataContext = GetDataContextForConnectionPane();

                dataContext.NetworkAddress = dataItem.NetworkAddress;
                dataContext.NodeString = dataItem.HostNameEntered;
                dataContext.CQLPort = dataItem.CQLPort;
                
                if (dataItem.ResolutionStatus == NodeState.Status.Error
                        || dataItem.ResolutionStatus == NodeState.Status.Unknown)
                {
                    dataContext.NoPromptOnDelete = true;
                    this.DeleteNode_Button_Click(sender, e);
                }
            }            
            
        }

        private void ConnectionNodesGrid_RecordsDeleting(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletingEventArgs e)
        {
            var dataContext = GetDataContextForConnectionPane();

            if (dataContext != null)
            {
                e.DisplayPromptMessage = !dataContext.NoPromptOnDelete;
            }           
        }

        private void XamMaskedNodeString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.buttonAddToConnectionList_Click(sender, new RoutedEventArgs(e.RoutedEvent));
                e.Handled = true;
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = GetDataContextForConnectionPane();

            if (dataContext != null)
            {
                dataContext.DialogAction = DialogActions.Cancel;
                this.Close();
            }
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            using (this._waitCursor.UsingCreate())
            {
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var dataContext = GetDataContextForConnectionPane();

            if (dataContext != null && dataContext.DialogAction == DialogActions.Unknown)
            {
                dataContext.DialogAction = DialogActions.Cancel;                
            }
        }
    }
}
