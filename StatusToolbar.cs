using System;
using System.Collections.Generic;
using System.Text;
using KeePass.UI;
using KeePassLib;
using System.Windows.Forms;
using System.Diagnostics;

namespace SecureNotesPlugin
{
    partial class SecureNotesPluginExt
    {
        
        class StatusToolbar
        {
            public const string KNAddNew = "KNAddNew";
            public const string KNDelete = "KNDelete";
            public const string KNToDo = "KNToDo";
            public const string KNUrgent = "KNUrgent";
            public const string KNInProgress = "KNInProgress";
            public const string KNDone = "KNDone";

            new List<System.Windows.Forms.ToolStripItem> toolbarItems;
            public StatusToolbar()
            {
                toolbarItems = new List<System.Windows.Forms.ToolStripItem>()
                {
                    new System.Windows.Forms.ToolStripSeparator(),
                    addItem(KNAddNew, "Add new note", "B16x16_KOrganizer", PwIcon.Apple),
                    addItem(KNDelete, "Delete note", "B16x16_File_Close", PwIcon.Apple),
                    new System.Windows.Forms.ToolStripSeparator(),
                    addItem(KNToDo, "To do", "B16x16_KNotes", PwIcon.Note),
                    addItem(KNUrgent, "Urgent", "B16x16_WinFavs", PwIcon.Star),
                    addItem(KNInProgress, "In progress", "B16x16_Package_Development", PwIcon.Tool),
                    addItem(KNDone, "Done", "B16x16_Apply", PwIcon.Checked),

                };
                m_toolMain.Items.AddRange(toolbarItems.ToArray());
            }

            public void show()
            {
                foreach (var item in toolbarItems)
                {
                    item.Visible = true;
                }
            }

            public void hide()
            {
                foreach (var item in toolbarItems)
                {
                    item.Visible = false;
                }

            }
            private void statusButtonHandler(object sender, EventArgs e)
            {
                System.Windows.Forms.ToolStripItem item = (System.Windows.Forms.ToolStripItem)sender;
                switch (item.Name)
                {
                    case KNAddNew:
                        break;
                    case KNDelete:
                        break;
                    case KNDone:
                    case KNInProgress:
                    case KNToDo:
                    case KNUrgent:
                        foreach (ListViewItem listitem in m_lvEntries.SelectedItems)
                        {
                            SaveEntryIcon(listitem, (PwIcon)item.Tag, item.Text);
                        }
                        Util.UpdateSaveState();
                        m_host.MainWindow.Refresh();
                        //TEST m_host.MainWindow.RefreshEntriesList();
                        break;
                }
            }

            private bool SaveEntryIcon(ListViewItem Item, PwIcon icon, string text)
            {
                PwListItem pli = (((ListViewItem)Item).Tag as PwListItem);
                if (pli == null) { Debug.Assert(false); return false; }
                PwEntry pe = pli.Entry;
                pe = m_host.Database.RootGroup.FindEntry(pe.Uuid, true);

                PwEntry peInit = pe.CloneDeep();
                pe.CreateBackup(null);
                pe.Touch(true, false); // Touch *after* backup

                pe.IconId = icon;

                /*
                int colID = SubItem;
                AceColumn col = GetAceColumn(colID);
                AceColumnType colType = col.Type;
                switch (colType)
                {
                    case AceColumnType.Title:
                        //if(PwDefs.IsTanEntry(pe))
                        //TODO tan list	 TanTitle ???		    pe.Strings.Set(PwDefs.TanTitle, new ProtectedString(false, Text));
                        //else
                        pe.Strings.Set(PwDefs.TitleField, new ProtectedString(pwStorage.MemoryProtection.ProtectTitle, Text));
                        break;
                    case AceColumnType.UserName:
                        pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(pwStorage.MemoryProtection.ProtectUserName, Text));
                        break;
                    case AceColumnType.Password:
                        //byte[] pb = Text.ToUtf8();
                        //pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(pwStorage.MemoryProtection.ProtectPassword, pb));
                        //MemUtil.ZeroByteArray(pb);
                        pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(pwStorage.MemoryProtection.ProtectPassword, Text));
                        break;
                    case AceColumnType.Url:
                        pe.Strings.Set(PwDefs.UrlField, new ProtectedString(pwStorage.MemoryProtection.ProtectUrl, Text));
                        break;
                    case AceColumnType.Notes:
                        pe.Strings.Set(PwDefs.NotesField, new ProtectedString(pwStorage.MemoryProtection.ProtectNotes, Text));
                        break;
                    case AceColumnType.OverrideUrl:
                        pe.OverrideUrl = Text;
                        break;
                    case AceColumnType.Tags:
                        List<string> vNewTags = StrUtil.StringToTags(Text);
                        pe.Tags.Clear();
                        foreach (string strTag in vNewTags) pe.AddTag(strTag);
                        break;
                    case AceColumnType.CustomString:
                        pe.Strings.Set(col.CustomName, new ProtectedString(pe.Strings.GetSafe(col.CustomName).IsProtected, Text));
                        break;
                    default:
                        // Nothing todo
                        break;
                }
                */

                // refresh UI
                Item.ImageIndex = (int)icon;

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
                item.Click += new System.EventHandler(this.statusButtonHandler);

                return item;
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
