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
        private DateTime? mEntryLastModificationTime;
        private PwEntry mEntry;

        private System.Windows.Forms.TabPage mAllTextTab;

        
        public RTEControl()
        {
            InitializeComponent();
            mAllTextTab = new TabPage();
        }


        private void RTEControl_Load(object sender, EventArgs e)
        {
            title.Text = "This is a test";
            ctrlHost = new ElementHost
            {
                Dock = DockStyle.Fill
            };
            panel2.Controls.Add(ctrlHost);
            wpfRTE = new WpfRichText.RichTextEditor()
            {
                Background = System.Windows.SystemColors.ControlLightLightBrush,
                Foreground = System.Windows.SystemColors.ControlTextBrush,
                BorderThickness = new System.Windows.Thickness(1),
                BorderBrush = System.Windows.SystemColors.MenuBarBrush

            };
            wpfRTE.InitializeComponent();
            ctrlHost.Child = wpfRTE;

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void RecordEntryLastModificationTime()
        {
            if (mEntry != null)
            {
                mEntryLastModificationTime = mEntry.LastModificationTime;
            }
            else
            {
                mEntryLastModificationTime = null;
            }
        }
        private bool HasEntryBeenModifiedSinceLastModificationTime(PwEntry entry)
        {
            return entry == null || entry.LastModificationTime != mEntryLastModificationTime;
        }
        /// <summary>
		/// Gets or sets a single selected entry. Will clear any previous value set to <see cref="Entries"/>
		/// Returns null if a multiple selection has been set using <see cref="Entries"/>
		/// </summary>
		public PwEntry Entry
        {
            get { return mEntry; }
            set
            {
                if (value != mEntry || HasEntryBeenModifiedSinceLastModificationTime(value))
                {
                    mEntry = value;
                    

                    RecordEntryLastModificationTime();
                    OnEntryChanged(EventArgs.Empty);
                }
            }
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

        protected virtual void OnEntryChanged(EventArgs e)
        {
            
            if (Entry == null)
            {
                panel1.Hide();
            }
            else
            {
                panel1.Show();
                title.Text = mEntry.Strings.Get(PwDefs.TitleField).ReadString();
            }
        }

    }
}
