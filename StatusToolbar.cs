using System;
using System.Collections.Generic;
using System.Text;

namespace SecureNotesPlugin
{
    partial class SecureNotesPluginExt
    {
        class StatusToolbar
        {
            public StatusToolbar()
            {
                m_toolMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                    addItem("Add new", "B16x16_KNotes"),
                    addItem("Add delete", "B16x16_KNotes"),

                    addItem("To do", "B16x16_KNotes"),
                    addItem("Urgent", "B16x16_WinFavs"),
                    addItem("In progress", "B16x16_Package_Development"),
                    addItem("Done", "B16x16_Apply"),
                });
            }
            
            private System.Windows.Forms.ToolStripItem addItem(string command, string imageName)
            {
                
                System.Windows.Forms.ToolStripButton item = new System.Windows.Forms.ToolStripButton();
                item.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
                item.Image = (System.Drawing.Image)m_host.Resources.GetObject(imageName);
                item.ImageTransparentColor = System.Drawing.Color.Magenta;
                item.Name = command;
                item.Size = new System.Drawing.Size(23, 22);
                item.ToolTipText = command;
                //item.Click += new System.EventHandler(this.OnFileLock);

                return item;
            }
            public void Close()
            {

            }

        }
    }
}
