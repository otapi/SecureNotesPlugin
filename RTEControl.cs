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
using WpfRichText;
namespace SecureNotesPlugin
{
    public partial class RTEControl : UserControl
    {
        private ElementHost ctrlHost;
        private WpfRichText.RichTextEditor wpfRTE;


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

        private void RTEControl_Load(object sender, EventArgs e)
        {
            ctrlHost = new ElementHost
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(ctrlHost);
            wpfRTE = new RichTextEditor();
            wpfRTE.InitializeComponent();
            ctrlHost.Child = wpfRTE;
            
            /*
            wpfInputControl.OnButtonClick +=
                new WPFInputControl.MyControlEventHandler(
                Panel_OnButtonClick);
            wpfInputControl.Loaded += new RoutedEventHandler(
                Panel_Loaded);
                */
        }

        private void RTEControl_ControlAdded(object sender, ControlEventArgs e)
        {
            /*
             * initBackBrush = (SolidColorBrush)wpfInputControl.MyControl_Background;
            initForeBrush = wpfInputControl.MyControl_Foreground;
            initFontFamily = wpfInputControl.MyControl_FontFamily;
            initFontSize = wpfInputControl.MyControl_FontSize;
            initFontWeight = wpfInputControl.MyControl_FontWeight;
            initFontStyle = wpfInputControl.MyControl_FontStyle;*
            */
        }
    }
}
