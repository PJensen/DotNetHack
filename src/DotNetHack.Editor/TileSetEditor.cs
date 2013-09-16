﻿using DotNetHack.Core.Game.Tiles;
using DotNetHack.Core.Game.World;
using DotNetHack.Shared.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNetHack.Editor.Forms
{
    /// <summary>
    /// TileSetEditor
    /// </summary>
    public partial class TileSetEditor : Form
    {
        /// <summary>
        /// FileName
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Saved
        /// </summary>
        public bool Saved { get; set; }

        /// <summary>
        /// LastUpdated
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// TileSetEditor
        /// </summary>
        public TileSetEditor()
        {
            InitializeComponent();
            TileMapping = new TileMapping();
            ImageCache = new Dictionary<Point, Image>();
            OriginalFormTitle = Text;
            TopLevel = false;
        }

        /// <summary>
        /// pictureBoxMain_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pictureBoxMain_Click(object sender, EventArgs e)
        {
            int tileSize = Shared.Properties.Settings.Default.TileSize;
            Point tmpOffset = pictureBoxMain.PointToScreen(pictureBoxMain.Location);

            tmpOffset.X += Math.Abs(CurrentOffset.X);
            tmpOffset.Y += Math.Abs(CurrentOffset.Y);

            int xTile = Math.Abs((tmpOffset.X - MousePosition.X) / tileSize);
            int yTile = Math.Abs((tmpOffset.Y - MousePosition.Y) / tileSize);

            CurrentTile = new TileMapping.MappedTile(xTile, yTile, Tile.TileType.None);

            UpdateImage(CurrentTile);
            UpdateTileProperties(CurrentTile);
        }

        /// <summary>
        /// UpdateTileProperties();
        /// </summary>
        private void UpdateTileProperties(TileMapping.MappedTile tile)
        {
            UpdateImage(CurrentTile);

            propertyGridMain.SelectedObject = CurrentTile;
            propertyGridMain.Refresh();
        }

        /// <summary>
        /// UpdateImage
        /// </summary>
        /// <param name="tile">tile</param>
        private void UpdateImage(TileMapping.MappedTile tile)
        {
            Point tmpPoint = new Point(tile.XMapping, tile.YMapping);
            if (!ImageCache.ContainsKey(tmpPoint))
                ImageCache.Add(tmpPoint, Shared.Util.GetTile(pictureBoxMain.Image, tmpPoint.X, tmpPoint.Y));
            pictureBoxSecondary.Image = ImageCache[tmpPoint];
        }

        /// <summary>
        /// SaveTileSet
        /// </summary>
        /// <param name="fullPath"></param>
        private void SaveTileSet(string fullPath)
        {
            TileMapping.Save(TileMapping, fullPath);
            UpdateStatus("Saved: {0}", saveFileDialog.FileName);

            Saved = true;
            FileName = saveFileDialog.FileName;
            LastUpdated = DateTime.Now;

            if (!Properties.Settings.Default.RecentTileSets.Contains(saveFileDialog.FileName))
                Properties.Settings.Default.RecentTileSets.Add(saveFileDialog.FileName);
            Properties.Settings.Default.Save();

            UpdateListBox();
        }

        /// <summary>
        /// LoadTileSet
        /// </summary>
        /// <param name="fullPath"></param>
        private void LoadTileSet(string fullPath)
        {
            TileMapping.Load(fullPath, out TileMapping);
            FileName = fullPath;

            UpdateStatus("Loaded: {0}", fullPath);
            UpdateListBox();
        }

        /// <summary>
        /// AddUpdateMapping();
        /// </summary>
        private void AddUpdateMapping(TileMapping.MappedTile tmpMappedTile)
        {
            Saved = false;

            if (TileMapping.Mapping.Contains(tmpMappedTile))
            {
                TileMapping.MappedTile tmpLookupTile =
                    TileMapping.Mapping.Single(t => t.Equals(tmpMappedTile));
                tmpLookupTile = tmpMappedTile;
            }
            else { TileMapping.Mapping.Add(tmpMappedTile); }
        }

        /// <summary>
        /// TileEditor_Load
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TileEditor_Load(object sender, EventArgs e)
        {
            LoadRecentMappings();

            if (!File.Exists(Shared.Properties.Settings.Default.TileSetImagePath) &&
                string.IsNullOrEmpty(Shared.Properties.Settings.Default.TileSetImagePath))
                SaveUpdateTileSetPath();
            UpdateTileSetTextBoxAndImage();
        }

        /// <summary>
        /// LoadRecent
        /// </summary>
        private void LoadRecentMappings()
        {
            #region Recent TileSets

            if (Properties.Settings.Default.RecentTileSets == null)
            {
                Properties.Settings.Default.RecentTileSets = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.RecentTileSets != null)
                foreach (string s in Properties.Settings.Default.RecentTileSets)
                {
                    ToolStripMenuItem tmpNewMenuItem = new ToolStripMenuItem(s);
                    tmpNewMenuItem.Click += (object senderInner, EventArgs eInner) => { LoadTileSet(s); };
                    recentToolStripMenuItem.DropDownItems.Add(tmpNewMenuItem);
                }

            #endregion
        }

        /// <summary>
        /// UpdateStatus
        /// </summary>
        private void UpdateStatus(string frmt, params object[] argv)
        {
            toolStripStatusLabel.Text = string.Format(frmt, argv);
        }

        /// <summary>
        /// UpdateTileSetTextBoxAndImage
        /// </summary>
        private void UpdateTileSetTextBoxAndImage()
        {
            if (!File.Exists(Shared.Properties.Settings.Default.TileSetImagePath))
                SaveUpdateTileSetPath();

            textBoxTileSetPath.Text = Shared.Properties.Settings.Default.TileSetImagePath;
            TileMapping.TileSetPath = Shared.Properties.Settings.Default.TileSetImagePath;
            pictureBoxMain.Image = Image.FromFile(Shared.Properties.Settings.Default.TileSetImagePath);

            pictureBoxMain.Width = pictureBoxMain.Image.Width * 2;
            pictureBoxMain.Height = pictureBoxMain.Image.Height * 2;

            CurrentOffset = new Point();
        }

        /// <summary>
        /// SaveUpdateTileSetPath
        /// </summary>
        private void SaveUpdateTileSetPath()
        {
            if (openFileDialogTileSetImage.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Shared.Properties.Settings.Default.TileSetImagePath = openFileDialogTileSetImage.FileName;
                Shared.Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// saveToolStripMenuItem_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                try { SaveTileSet(FileName); }
                catch (Exception ex) { UpdateStatus(ex.Message); }
            }
            else
            {
                switch (saveFileDialog.ShowDialog(this))
                {
                    case System.Windows.Forms.DialogResult.OK:
                        {
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// loadToolStripMenuItem_Click
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (openFileDialog.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                    {
                        LoadTileSet(openFileDialog.FileName);
                        break;
                    }
            }
        }

        /// <summary>
        /// buttonAddMapping_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddMapping_Click(object sender, EventArgs e)
        {
            AddUpdateMapping(CurrentTile);
            UpdateListBox();
        }

        /// <summary>
        /// UpdateListBox
        /// </summary>
        private void UpdateListBox()
        {
            listBoxMapping.Items.Clear();
            listBoxMapping.Items.AddRange(TileMapping.Mapping.ToArray());
        }

        /// <summary>
        /// buttonRemoveMapping_Click
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonRemoveMapping_Click(object sender, EventArgs e)
        {
            UpdateListBox();
        }

        /// <summary>
        /// TileEditor_FormClosing
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TileEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Saved)
            {
                switch (MessageBox.Show("Save your work?", "DotNetHack Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        e.Cancel = true;
                        saveToolStripMenuItem_Click(sender, e);
                        break;
                }
            }
        }

        /// <summary>
        /// listBoxMapping_SelectedIndexChanged
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void listBoxMapping_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxMapping.SelectedItem != null)
            {
                CurrentTile = (TileMapping.MappedTile)listBoxMapping.SelectedItem;

                UpdateTileProperties(CurrentTile);
                UpdateImage(CurrentTile);

                listBoxMapping.Refresh();
            }
        }

        /// <summary>
        /// exitToolStripMenuItem_Click
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// toolStripMenuItemRemove_Click
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void toolStripMenuItemRemove_Click(object sender, EventArgs e)
        {
            if (listBoxMapping.SelectedItem != null)
                listBoxMapping.Items.Remove(listBoxMapping.SelectedItem);
        }

        /// <summary>
        /// listBoxMapping_MouseDoubleClick
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void listBoxMapping_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    contextMenuStripTileSet.Show(this, e.Location);
                    break;
            }
        }

        /// <summary>
        /// textBoxTileSetPath_MouseDoubleClick
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void textBoxTileSetPath_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SaveUpdateTileSetPath();
            UpdateTileSetTextBoxAndImage();
        }

        /// <summary>
        /// pictureBoxMain_Move
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void pictureBoxMain_Move(object sender, EventArgs e)
        {
            CurrentOffset = pictureBoxMain.Location;
        }

        /// <summary>
        /// pictureBoxMain_Paint
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void pictureBoxMain_Paint(object sender, PaintEventArgs e) { }

        /// <summary>
        /// CurrentOffset
        /// </summary>
        Point CurrentOffset = new Point();

        /// <summary>
        /// CurrentTile
        /// <remarks>This tile may or may not be mapped & saved.</remarks>
        /// </summary>
        TileMapping.MappedTile CurrentTile { get; set; }

        /// <summary>
        /// TileMapping
        /// </summary>
        TileMapping TileMapping;

        /// <summary>
        /// OriginalFormTitle
        /// </summary>
        readonly string OriginalFormTitle;

        /// <summary>
        /// ImageCache
        /// </summary>
        Dictionary<Point, Image> ImageCache;
    }
}