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
using KeePassLib.Security;
using System.Windows.Forms.Integration;

namespace SecureNotesPlugin
{
    public partial class RTEControl : UserControl
    {
        private ElementHost ctrlHost;
        private WpfRichText.RichTextEditor rtfEditor;
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
            rtfEditor = new WpfRichText.RichTextEditor()
            {
                Background = System.Windows.SystemColors.ControlLightLightBrush,
                Foreground = System.Windows.SystemColors.ControlTextBrush,
                BorderThickness = new System.Windows.Thickness(1),
                BorderBrush = System.Windows.SystemColors.MenuBarBrush

            };
            rtfEditor.InitializeComponent();
            ctrlHost.Child = rtfEditor;

            // TODO: saveEntry on textChange event of rtfEditor

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
                if (mEntry != null)
                {
                    if (value != null && value.Uuid != mEntry.Uuid)
                    {
                        RecordEntryLastModificationTime();
                        saveEntry();

                    }

                }


                if (value != null)
                {
                    if (mEntry != null && value.Uuid == mEntry.Uuid)
                    {

                    }
                    else
                    {
                        saveEntry();
                        mEntry = value;
                        panel1.Show();
                        oldTitle = mEntry.Strings.Get(PwDefs.TitleField).ReadString();
                        if (mEntry.Strings.Get(PwDefs.NotesField) != null)
                        {
                            oldNote = mEntry.Strings.Get(PwDefs.NotesField).ReadString();
                        }
                        else
                        {
                            oldNote = "";
                        }
                        rtfEditor.Text = oldNote;
                        title.Text = oldTitle;
                        RecordEntryLastModificationTime();
                       
                    }
                }
                else
                {
                    panel1.Hide();
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
        
        public Action NotifyHostOfModification;

        private string oldTitle = null;
        private string oldNote = null;
        public void saveEntry()
        {
            if (mEntry != null)
            {

                if (oldTitle != null && (oldTitle != title.Text || oldNote != rtfEditor.Text))
                {
                    mEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(false, title.Text));
                    mEntry.Strings.Set(PwDefs.NotesField, new ProtectedString(false, rtfEditor.Text));
                  NotifyHostOfModification();
                }
                oldTitle = title.Text;
                oldNote = rtfEditor.Text;
 
            }
        }

        private void title_TextChanged(object sender, EventArgs e)
        {
            saveEntry();
        }
    }
}
