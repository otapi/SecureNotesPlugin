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
using System.Windows.Forms.Integration;

namespace SecureNotesPlugin
{
    public partial class RTEControl : UserControl
    {
        private ElementHost ctrlHost;
        private WpfRichText.RichTextEditor wpfRTE;


        private System.Windows.Forms.TabPage mAllTextTab;

        System.Windows.FontWeight initFontWeight;
        double initFontSize;
        System.Windows.FontStyle initFontStyle;
        System.Windows.Media.SolidColorBrush initBackBrush;
        System.Windows.Media.SolidColorBrush initForeBrush;
        System.Windows.Media.FontFamily initFontFamily;

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

        private void RTEControl_Load(object sender, EventArgs e)
        {
            ctrlHost = new ElementHost
            {
                Dock = DockStyle.Fill
            };
            panel1.Controls.Add(ctrlHost);
            wpfRTE = new WpfRichText.RichTextEditor();
            wpfRTE.InitializeComponent();
            ctrlHost.Child = wpfRTE;

        }
    }
}
