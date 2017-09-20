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
	public sealed class SecureNotesPluginExt : Plugin
	{
		private SecureNotesPluginProv m_prov = null;

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

			m_prov = new SecureNotesPluginProv();
            
			m_host.ColumnProviderPool.Add(m_prov);

			return true;
		}

		public override void Terminate()
		{
			if(m_host == null) return;

			m_host.ColumnProviderPool.Remove(m_prov);
			m_prov = null;

			m_host = null;
		}
	}

	public sealed class SecureNotesPluginProv : ColumnProvider
	{
		private const string ColorColumnName = "Change Color";

		public override string[] ColumnNames
		{
			get { return new string[] { ColorColumnName }; }
		}

		public override string GetCellData(string strColumnName, PwEntry pe)
		{
			return string.Empty;
		}

		public override bool SupportsCellAction(string strColumnName)
		{
			return (strColumnName == ColorColumnName);
		}

		public override void PerformCellAction(string strColumnName, PwEntry pe)
		{
			if((strColumnName == ColorColumnName) && (pe != null))
			{
				pe.BackgroundColor = Color.FromArgb(
					Program.GlobalRandom.Next(0, 256),
					Program.GlobalRandom.Next(0, 256),
					Program.GlobalRandom.Next(0, 256));

				MainForm mf = SecureNotesPluginExt.Host.MainWindow;
				mf.UpdateUI(false, null, false, null, true, null, true);

				// Selected items are drawn with a selection color
				// background; as we want the new background color to
				// be visible instantly, we deselect the item
				ListView lv = (mf.Controls.Find("m_lvEntries", true)[0]
					as ListView);
				ListViewItem lvi = lv.FocusedItem;
				if(lvi != null) lvi.Selected = false;
			}
		}
	}
}
