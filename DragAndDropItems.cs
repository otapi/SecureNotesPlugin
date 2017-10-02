using System;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;

using KeePass;
using KeePass.App;
using KeePass.App.Configuration;
using KeePass.Plugins;
using KeePass.Forms;
using KeePass.UI;
using KeePass.Util;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace SecureNotesPlugin
{
    partial class SecureNotesPluginExt
    {
        class DragAndDropItems
        {
            bool privateDrag;

            public DragAndDropItems()
            {
                m_lvEntries.ItemDrag += new ItemDragEventHandler(ItemDrag);
                m_lvEntries.DragEnter += new DragEventHandler(DragEnter);
                m_lvEntries.DragDrop += new DragEventHandler(DragDrop);

                
            }

            public void Close()
            {
                m_lvEntries.ItemDrag -= new ItemDragEventHandler(ItemDrag);
                m_lvEntries.DragEnter -= new DragEventHandler(DragEnter);
                m_lvEntries.DragDrop -= new DragEventHandler(DragDrop);
            }
            private void ItemDrag(object sender, ItemDragEventArgs e)
            {
                privateDrag = true;
                m_lvEntries.DoDragDrop(m_lvEntries.SelectedItems, DragDropEffects.Move);
                privateDrag = false;
            }
            private void DragEnter(object sender, DragEventArgs e)
            {
                if (privateDrag)
                {
                    int len = e.Data.GetFormats().Length - 1;
                    int i;
                    for (i = 0; i <= len; i++)
                    {
                        //The data from the drag source is moved to the target.
                        e.Effect = DragDropEffects.Move;
                        //e.Effect = e.AllowedEffect;
                    }
                }
                    
            }



            private void DragDrop(object sender, DragEventArgs e)
            {
                /*
                var pos = m_lvEntries.PointToClient(new Point(e.X, e.Y));
                var hit = m_lvEntries.HitTest(pos);
                if (hit.Item != null && hit.Item.Tag != null)
                {
                    var dragItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                    move(dragItem, hit.Item.Index);
                }*/

                //Return if the items are not selected in the ListView control.
                if (m_lvEntries.SelectedItems.Count == 0)
                {
                    return;
                }
                //Returns the location of the mouse pointer in the ListView control.
                Point cp = m_lvEntries.PointToClient(new Point(e.X, e.Y));
                //Obtain the item that is located at the specified location of the mouse pointer.
                ListViewItem dragToItem = m_lvEntries.GetItemAt(cp.X, cp.Y);
                if (dragToItem == null)
                {
                    return;
                }
                //Obtain the index of the item at the mouse pointer.
                int dragIndex = dragToItem.Index;
                ListViewItem[] sel = new ListViewItem[m_lvEntries.SelectedItems.Count];
                for (int i = 0; i <= m_lvEntries.SelectedItems.Count - 1; i++)
                {
                    sel[i] = m_lvEntries.SelectedItems[i];
                }
                for (int i = 0; i < sel.GetLength(0); i++)
                {
                    //Obtain the ListViewItem to be dragged to the target location.
                    ListViewItem dragItem = sel[i];
                    int itemIndex = dragIndex;
                    if (itemIndex == dragItem.Index)
                    {
                        return;
                    }
                    if (dragItem.Index < itemIndex)
                        itemIndex++;
                    else
                        itemIndex = dragIndex + i;
                    //Insert the item at the mouse pointer.
                    ListViewItem insertItem = (ListViewItem)dragItem.Clone();
                    m_lvEntries.Items.Insert(itemIndex, insertItem);
                    //Removes the item from the initial location while 
                    //the item is moved to the new location.
                    m_lvEntries.Items.Remove(dragItem);
                }
            }


        }
    }
}
