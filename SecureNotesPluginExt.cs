/*
  SecureNotesPlugin Plugin
  Copyright (C) 2017 otapiGems <otapiGems@gmail.com>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using KeePass;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;

using KeePassLib;

namespace SecureNotesPlugin
{
	public sealed partial class SecureNotesPluginExt : Plugin
	{
		
        private const string m_ctseName = "m_toolMain";
        private const string m_clveName = "m_lvEntries";
        //private const string m_ctveName = "m_tvGroups";
        //private const string m_csceName = "m_splitVertical";

        public static CustomToolStripEx m_toolMain = null;
        public static CustomListViewEx m_lvEntries = null;
        //public static CustomTreeViewEx m_tvGroups = null;
        //private CustomSplitContainerEx m_csceSplitVertical = null;

        // Declaration Sub Classes
        private static StatusToolbar statusToolbar;
        private static InlineEditing inlineEditing;


        private static IPluginHost m_host = null;
		internal static IPluginHost Host
		{
			get { return m_host; }
		}

		public override bool Initialize(IPluginHost host)
		{
			Terminate();

			if(host == null) return false;

			m_host = host;

            // Find the main controls 
            m_toolMain = (CustomToolStripEx)Util.FindControlRecursive(m_host.MainWindow, m_ctseName);
            m_lvEntries = (CustomListViewEx)Util.FindControlRecursive(m_host.MainWindow, m_clveName);
            m_lvEntries.ListViewItemSorter = new CompareByIndex(m_lvEntries);
            //m_tsmiMenuView = (ToolStripMenuItem)Util.FindControlRecursive(m_host.MainWindow, m_tsmiName);
            //m_tvGroups = (CustomTreeViewEx)Util.FindControlRecursive(m_host.MainWindow, m_ctveName);
            //m_csceSplitVertical = (CustomSplitContainerEx)Util.FindControlRecursive(m_host.MainWindow, m_csceName);

            statusToolbar = new StatusToolbar();
            inlineEditing = new InlineEditing();
 
            return true;
		}

        class CompareByIndex : System.Collections.IComparer
        {
            private readonly ListView _listView;

            public CompareByIndex(CustomListViewEx listView)
            {
                this._listView = listView;
            }
            public int Compare(object x, object y)
            {
                int i = this._listView.Items.IndexOf((ListViewItem)x);
                int j = this._listView.Items.IndexOf((ListViewItem)y);
                return i - j;
            }
        }

        public override void Terminate()
		{
			if(m_host == null) return;
            statusToolbar.Close();
            inlineEditing.Close();

            m_host = null;
		}
	}


}
