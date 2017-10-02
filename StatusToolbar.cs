using System;
using System.Collections.Generic;
using System.Text;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Security;
using System.Windows.Forms;
using System.Diagnostics;

namespace SecureNotesPlugin
{
    partial class SecureNotesPluginExt
    {
        
        class StatusToolbar
        {

            public const string KNAddNew = "KNAddNew";
            public const string KNToDo = "KNToDo";
            public const string KNUrgent = "KNUrgent";
            public const string KNInProgress = "KNInProgress";
            public const string KNDone = "KNDone";

            public const string KNUp = "KNUP";
            public const string KNDOWN = "KNDown";

            public const string KNToDoStr = "To Do";

            new List<System.Windows.Forms.ToolStripItem> toolbarItems;
            

            public StatusToolbar()
            {
                toolbarItems = new List<System.Windows.Forms.ToolStripItem>()
                {
                    new System.Windows.Forms.ToolStripSeparator(),
                    addItem(KNAddNew, "Add new note", "B16x16_KOrganizer", PwIcon.Apple),

                    new System.Windows.Forms.ToolStripSeparator(),
                    addItem(KNToDo, KNToDoStr, "B16x16_KNotes", PwIcon.Note),
                    addItem(KNUrgent, "Urgent", "B16x16_WinFavs", PwIcon.Star),
                    addItem(KNInProgress, "In progress", "B16x16_Package_Development", PwIcon.Tool),
                    addItem(KNDone, "Done", "B16x16_Apply", PwIcon.Checked),


                };
                m_toolMain.Items.AddRange(toolbarItems.ToArray());
            }

            public void Show()
            {
                foreach (var item in toolbarItems)
                {
                    item.Visible = true;
                }
            }

            public void Hide()
            {
                foreach (var item in toolbarItems)
                {
                    item.Visible = false;
                }

            }
            private void StatusButtonHandler(object sender, EventArgs e)
            {
                System.Windows.Forms.ToolStripItem item = (System.Windows.Forms.ToolStripItem)sender;
                switch (item.Name)
                {
                    case KNAddNew:
                        AddNewNote();
                        break;
                    case KNUp:
                    case KNDOWN:
                        //moveItems(item.Name == KNUp ? -1 : 1);
                        break;
                    case KNDone:
                    case KNInProgress:
                    case KNToDo:
                    case KNUrgent:
                        bool changed = false;
                        foreach (ListViewItem listitem in m_lvEntries.SelectedItems)
                        {

                            changed = changed || SaveEntryStatus(listitem, (PwIcon)item.Tag, item.ToolTipText);
                        }
                        if (changed)
                        {
                            Util.UpdateSaveState();
                            m_host.MainWindow.Refresh();
                        }
                        break;
                }
            }

            private bool SaveEntryStatus(ListViewItem Item, PwIcon icon, string text)
            {
                PwListItem pli = (((ListViewItem)Item).Tag as PwListItem);
                if (pli == null) { Debug.Assert(false); return false; }
                PwEntry pe = pli.Entry;
                pe = m_host.Database.RootGroup.FindEntry(pe.Uuid, true);

                if (!pe.Strings.Get(PwDefs.PasswordField).IsEmpty) {
                    return false;
                }

                PwEntry peInit = pe.CloneDeep();
                pe.CreateBackup(null);
                pe.Touch(true, false); // Touch *after* backup

                
                pe.IconId = icon;
                Item.ImageIndex = (int)icon;

                pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, text));
                Item.SubItems[getSubitemOfField(KeePass.App.Configuration.AceColumnType.UserName)].Text = text;

                PwCompareOptions cmpOpt = (PwCompareOptions.IgnoreLastMod | PwCompareOptions.IgnoreLastAccess | PwCompareOptions.IgnoreLastBackup);
                if (pe.EqualsEntry(peInit, cmpOpt, MemProtCmpMode.None))
                {
                    pe.LastModificationTime = peInit.LastModificationTime;

                    pe.History.Remove(pe.History.GetAt(pe.History.UCount - 1)); // Undo backup

                    return false;
                }
                else
                {
                    return true;
                }
            }

            private int getSubitemOfField(KeePass.App.Configuration.AceColumnType field)
            {
                for (int i=0;i< KeePass.Program.Config.MainWindow.EntryListColumns.Count;i++)
                { 
                    if (KeePass.Program.Config.MainWindow.EntryListColumns[i].Type == KeePass.App.Configuration.AceColumnType.UserName)
                    {
                        return i;
                    }

                }
                Debug.Assert(false);
                return -1;
            }
            private System.Windows.Forms.ToolStripItem addItem(string name, string command, string imageName, KeePassLib.PwIcon icon)
            {
                
                System.Windows.Forms.ToolStripButton item = new System.Windows.Forms.ToolStripButton();
                item.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
                item.Image = (System.Drawing.Image)m_host.Resources.GetObject(imageName);
                item.ImageTransparentColor = System.Drawing.Color.Magenta;
                item.Name = name;
                item.Tag = icon;
                item.Size = new System.Drawing.Size(23, 22);
                item.ToolTipText = command;
                item.Click += new System.EventHandler(this.StatusButtonHandler);

                return item;
            }

            private void AddNewNote()
            {
                PwGroup pg = KeePass.Program.MainForm.GetSelectedGroup();
                
                // Create new entry which belongs to given group
                PwEntry pwe = Util.CreateEntry(pg);

                if (pwe == null) { return; }

                // Insort is only working in not sorted and not grouped listviews
                //ListViewItem lviFocus = Util.InsertListEntry(pwe, iIndex);
                pwe.IconId = PwIcon.Note;
                pwe.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, KNToDoStr));
                ListViewItem lviFocus = Util.AddEntryToList(pwe);

                lviFocus = Util.GuiFindEntry(pwe.Uuid);
                if (lviFocus != null) m_lvEntries.FocusedItem = lviFocus;

                m_host.MainWindow.EnsureVisibleEntry(pwe.Uuid);
                m_host.MainWindow.RefreshEntriesList();
                Util.UpdateSaveState();

                //PwObjectList<PwEntry> vSelect = new PwObjectList<PwEntry>();
                //vSelect.Add(pwe);
                //Util.SelectEntries(vSelect, true, true);

                m_lvEntries.Select();
                m_lvEntries.HideSelection = false;
                m_lvEntries.Focus();
                //MAYBE problem: ListView row is only focused but not highlighted (selected?)

                // Start inline editing with new item
                if (lviFocus != null)
                {
                    inlineEditing.StartEditing(lviFocus);
                }

                //IDEA: On esc press remove item, undo database changes and update save icon
            }

            private void moveItems()
            {

            }
            public void Close()
            {
                foreach (var item in toolbarItems)
                {
                    m_toolMain.Items.Remove(item);
                }
                
            }

        }
    }
}
