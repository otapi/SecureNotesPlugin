using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using KeePass;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using System.Diagnostics;

namespace SecureNotesPlugin
{
    partial class SecureNotesPluginExt
    {
        class EntryRichEditor
        {
            private RichTextBox mOriginalEntryView;

            public EntryRichEditor()
            {
                mOriginalEntryView = (RichTextBox)Util.FindControlRecursive(m_host.MainWindow, "m_richEntryView");
                var entryViewContainer = mOriginalEntryView.Parent;
                if (mOriginalEntryView == null || entryViewContainer == null)
                {
                    Debug.Fail("Couldn't find existing entry view to replace");
                }

                /*
                // Replace existing entry view with new one
                mEntryView = new EntryView(mHost.MainWindow, mOptions)
                {
                    Name = "m_KPEnhancedEntryView",
                    Dock = DockStyle.Fill,
                    Font = mOriginalEntryView.Font,
                    AutoValidate = AutoValidate.Disable, // Don't allow our internal validation to bubble upwards to KeePass
                };

                entryViewContainer.Controls.Add(mEntryView);

                // Move the original entry view into a tab on the new view
                entryViewContainer.Controls.Remove(mOriginalEntryView);
                mEntryView.AllTextControl = mOriginalEntryView;

                // Font is assigned, not inherited. So assign here too, and follow any changes
                mOriginalEntryView.FontChanged += mOriginalEntryView_FontChanged;

                // Hook UIStateUpdated to watch for current entry changing.
                mHost.MainWindow.UIStateUpdated += this.OnUIStateUpdated;

                // Database may be saved while in the middle of editing Notes. Watch for that and commit the current edit if that happens
                mHost.MainWindow.FileSaving += this.OnFileSaving;

                // Workspace may be locked while in the middle of editing. Also watch for that and commit the current edit if that happens
                mHost.MainWindow.FileClosingPre += this.OnFileClosingPre;

                // HACK: UIStateUpdated isn't called when toggling column value hiding on and off, so monitor the entries list for being invalidated
                mEntriesListView = FindControl<ListView>("m_lvEntries");
                if (mEntriesListView != null)
                {
                    mEntriesListView.Invalidated += mEntitiesListView_Invalidated;
                }

                // Hook events to update the UI when the entry is modified
                mEntryView.EntryModified += this.mEntryView_EntryModified;
                */
            }

            public void Close()
            {

            }
        }
    }
}
