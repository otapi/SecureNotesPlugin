using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using KeePass;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using System.Diagnostics;
using System.Threading;

namespace KeeNotesPlugin
{
    partial class KeeNotesPluginExt
    {
        class EntryRichEditor
        {
            private RichTextBox mOriginalEntryView;
            private RTEControl mRTEControl;

            public EntryRichEditor()
            {
                mOriginalEntryView = (RichTextBox)Util.FindControlRecursive(m_host.MainWindow, "m_richEntryView");
                var entryViewContainer = mOriginalEntryView.Parent;
                if (mOriginalEntryView == null || entryViewContainer == null)
                {
                    Debug.Fail("Couldn't find existing entry view to replace");
                }


                // Replace existing entry view with new one
                mRTEControl = new RTEControl()
                {
                    Name = "m_EntryRichEditor",
                    Dock = DockStyle.Fill,
                    Font = mOriginalEntryView.Font,
                    AutoValidate = AutoValidate.Disable, // Don't allow our internal validation to bubble upwards to KeePass
                };

                entryViewContainer.Controls.Add(mRTEControl);

                // Move the original entry view into a tab on the new view
                entryViewContainer.Controls.Remove(mOriginalEntryView);
                mRTEControl.AllTextControl = mOriginalEntryView;

                // Font is assigned, not inherited. So assign here too, and follow any changes
                mOriginalEntryView.FontChanged += mOriginalEntryView_FontChanged;

                // Hook UIStateUpdated to watch for current entry changing.
                m_host.MainWindow.UIStateUpdated += this.OnUIStateUpdated;

                // Database may be saved while in the middle of editing Notes. Watch for that and commit the current edit if that happens
                m_host.MainWindow.FileSaving += this.OnFileSaving;

                // Workspace may be locked while in the middle of editing. Also watch for that and commit the current edit if that happens
                m_host.MainWindow.FileClosingPre += this.OnFileClosingPre;

                
                
                mRTEControl.NotifyHostOfModification = this.NotifyHostOfModification;
               

            }

            public void Close()
            {

            }
            private bool saveNotified = false;

            
            private void NotifyHostOfModification()
            {
                m_host.MainWindow.RefreshEntriesList();
                if (!saveNotified)
                {
                    saveNotified = true;
                    m_host.MainWindow.UpdateUI(false, null, false, null, false, null, true);
                }
                
            }
            private void OnUIStateUpdated(object sender, EventArgs e)
            {
                mRTEControl.Entry = m_host.MainWindow.GetSelectedEntry(false);
            }

            private void OnFileSaving(object sender, FileSavingEventArgs e)
            {
                saveNotified = false;
            }

            private void OnFileClosingPre(object sender, FileClosingEventArgs e)
            {
                try
                {
                    mRTEControl.saveEntry();
                }
                finally
                {
                }
            }

            private void mOriginalEntryView_FontChanged(object sender, EventArgs e)
            {
                //mEntryView.Font = new System.Drawing.Font(mOriginalEntryView.Font, System.Drawing.FontStyle.Strikeout);
                mRTEControl.Font = mOriginalEntryView.Font;
            }


        }
    }
}
