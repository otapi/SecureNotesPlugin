using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass;
using KeePassLib;
namespace SecureNotesPlugin
{
    public partial class RTEControl : UserControl
    {
        private System.Windows.Forms.TabPage mAllTextTab;

        public RTEControl()
        {
            InitializeComponent();
            mAllTextTab = new TabPage();
        }

        public Control AllTextControl
        {
            get { return mAllTextTab.Controls.Cast<Control>().FirstOrDefault(); }
            set
            {
                mAllTextTab.Controls.Clear();
                mAllTextTab.Controls.Add(value);
            }
        }
        

        public class EntryModifiedEventArgs : EventArgs
        {
            private readonly PwEntry[] mEntries;

            public EntryModifiedEventArgs(IEnumerable<PwEntry> entries)
            {
                mEntries = entries.ToArray();
            }

            public EntryModifiedEventArgs(PwEntry entry)
            {
                mEntries = new[] { entry };
            }

            public IEnumerable<PwEntry> Entries
            {
                get { return mEntries; }
            }
        }
        public event EventHandler<EntryModifiedEventArgs> EntryModified;
        public void RefreshItems()
        {
            //TODO
        }
        public void FinishEditing()
        {
            //TODO cancelledit
        }
    }
}
