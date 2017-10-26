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

namespace SecureNotesPlugin
{
    partial class SecureNotesPluginExt
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

                // HACK: UIStateUpdated isn't called when toggling column value hiding on and off, so monitor the entries list for being invalidated
                if (m_lvEntries != null)
                {
                    m_lvEntries.Invalidated += mEntitiesListView_Invalidated;
                }

                // Hook events to update the UI when the entry is modified
                mRTEControl.EntryModified += this.mEntryView_EntryModified;
                
            }

            public void Close()
            {

            }
            private bool mNotifyHostImmediatelyOnModification;
            private volatile bool mHostRequiresModificationNotification;
            private void mEntryView_EntryModified(object sender, EventArgs e)
            {
                mHostRequiresModificationNotification = true;
                NotifyHostOfModification();
            }
            private void NotifyHostOfModification()
            {
                if (mNotifyHostImmediatelyOnModification)
                {
                    mHostRequiresModificationNotification = false;
                    m_host.MainWindow.UpdateUI(false, null, false, null, false, null, true);
                }
                else
                {
                    // Decouple the update from the current stack to avoid any recursive loops
                    m_host.MainWindow.BeginInvoke(new Action(delegate
                    {
                        if (!mHostRequiresModificationNotification)
                        {
                            // Host has already been notified, no further notification required.
                            return;
                        }

                        /*
                        // If it's already editing another cell, then don't bother mentioning that it's modified until editing finishes
                        var notificationDeferred = mRTEControl.DeferUntilCellEditingFinishes(NotifyHostOfModification);

                        if (notificationDeferred)
                        {
                            m_host.MainWindow.UpdateUI(false, null, false, null, false, null, false); // If the notification is deferred, don't notify as modified now, just notify the UI change without modified flag
                        }
                        else
                        {
                            // Notification is not deferred - notifying with modified flag now, and clearing the requires notification flag - further modifications after notification require further notifications.
                            mHostRequiresModificationNotification = false;
                            m_host.MainWindow.UpdateUI(false, null, false, null, false, null, true);
                        }*/
                        m_host.MainWindow.RefreshEntriesList();
                    }));
                }
            }
            private static readonly TimeSpan EntitiesListViewInvalidationTimeout = TimeSpan.FromMilliseconds(250); // Consolidate any Invalidated events that occur within 250ms of each other
            private readonly object mEntitiesListViewInvalidationTimerLock = new object();
            private Stopwatch mEntitiesListViewInvalidationTimer;

            private void mEntitiesListView_Invalidated(object sender, InvalidateEventArgs e)
            {
                // Whenever the entities list is invalidated, refresh the items of the entry view too (so that changes like column value hiding get reflected)

                // For performance, throttle refreshes to consume multiple invalidated events that occur within a short space of each other.
                lock (mEntitiesListViewInvalidationTimerLock)
                {
                    if (mEntitiesListViewInvalidationTimer == null)
                    {
                        mEntitiesListViewInvalidationTimer = Stopwatch.StartNew();
                        ThreadPool.QueueUserWorkItem(o => ConsolidateEntitiesListViewInvaidatedEvents());
                    }
                    else
                    {
                        // There's already a timer running, so just reset it counting from 0 again
                        mEntitiesListViewInvalidationTimer.Reset();
                        mEntitiesListViewInvalidationTimer.Start();
                    }
                }
            }
            private void ConsolidateEntitiesListViewInvaidatedEvents()
            {
                // Wait for timeout to expire
                do
                {
                    TimeSpan remainingTime;
                    lock (mEntitiesListViewInvalidationTimerLock)
                    {
                        remainingTime = EntitiesListViewInvalidationTimeout - mEntitiesListViewInvalidationTimer.Elapsed;

                        if (remainingTime <= TimeSpan.Zero)
                        {
                            // Discard the timer - subsequent Invalidated events will create a new one
                            mEntitiesListViewInvalidationTimer = null;
                            break;
                        }
                    }

                    Thread.Sleep(remainingTime);
                } while (true);

                if (mRTEControl != null)
                {
                    try
                    {
                        mRTEControl.BeginInvoke(new Action(delegate
                        {
                            if (mRTEControl != null)
                            {
                                mRTEControl.RefreshItems();
                            }
                        }));
                    }
                    catch (Exception)
                    {
                        // If it failed to invoke on the entry view it might be be because it's been disposed. Ignore.
                    }
                }
            }
            private void OnUIStateUpdated(object sender, EventArgs e)
            {
                mRTEControl.Entry = m_host.MainWindow.GetSelectedEntry(false);
            }

            private void OnFileSaving(object sender, FileSavingEventArgs e)
            {
                
            }

            private void OnFileClosingPre(object sender, FileClosingEventArgs e)
            {
                try
                {
                    mNotifyHostImmediatelyOnModification = true; // Modifications must be made before returning from this method if they are to be included in the save before closing
                    mRTEControl.FinishEditing();
                }
                finally
                {
                    mNotifyHostImmediatelyOnModification = false;
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
