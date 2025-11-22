using IniParser;
using RGBuild.IO;
using RGBuild.NAND;
using RGBuild.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RGBuild
{
    
    public partial class Main : Form
    {
        public static Main main;
        public NANDImage Image;
        private readonly ListViewColumnSorter sorter = new ListViewColumnSorter();

        public static byte[] prvKey = {
            0x3C, 0x71, 0xC1, 0xA9, 0xF8, 0xB6, 0x88, 0x70, 0xCF, 0x7E, 0x1E, 0xAC,
            0x1D, 0xAD, 0xDA, 0x10, 0x48, 0xEF, 0x23, 0x9E, 0xC7, 0xF5, 0xA4, 0x73,
            0xFE, 0xEA, 0xC4, 0xB1, 0xA6, 0x18, 0xC8, 0xD3, 0x91, 0xB5, 0x3B, 0x80,
            0xD5, 0xBB, 0x8A, 0x3B, 0xC6, 0x24, 0x23, 0x7F, 0x27, 0xCB, 0xE1, 0x1C,
            0xA1, 0xEA, 0xF2, 0x01, 0x10, 0xA1, 0x18, 0xD8, 0xAC, 0x16, 0xFB, 0xFD,
            0x08, 0xAD, 0xB1, 0xED, 0x18, 0x29, 0x9A, 0x87, 0x8F, 0x37, 0xE1, 0x4F,
            0x00, 0xA6, 0x76, 0xC6, 0xB9, 0x22, 0x6C, 0x14, 0xE7, 0x53, 0x38, 0xD6,
            0x37, 0x45, 0xE8, 0xF3, 0x46, 0xC1, 0xE0, 0x9E, 0x7C, 0xB3, 0x7B, 0xD0,
            0x13, 0x49, 0x8D, 0xDD, 0xE0, 0xE1, 0x14, 0x35, 0x20, 0x10, 0x4B, 0xB5,
            0x8E, 0xDC, 0x48, 0x57, 0x3F, 0xB1, 0xC8, 0x80, 0x9A, 0x4B, 0x38, 0xB1,
            0x05, 0x49, 0x9C, 0x21, 0x55, 0xEE, 0x4F, 0xAE, 0x52, 0x76, 0xC1, 0x52,
            0xF3, 0x5C, 0x30, 0x64, 0x49, 0x7F, 0x05, 0x8B, 0xAE, 0x7F, 0x3E, 0xF6,
            0xD9, 0xC4, 0x1F, 0x2A, 0x3A, 0x4D, 0x6D, 0x93, 0x31, 0x71, 0xE8, 0xC2,
            0x74, 0xC5, 0x04, 0x5B, 0xED, 0x26, 0x81, 0x9E, 0xC6, 0xB6, 0x07, 0xA4,
            0x03, 0x6D, 0x9D, 0xF5, 0x07, 0x37, 0x43, 0xC1, 0xE6, 0xA8, 0xC8, 0xCF,
            0x38, 0xCE, 0x01, 0x53, 0x68, 0x23, 0xC8, 0x3E, 0xBF, 0xC7, 0xE3, 0x8C,
            0xFA, 0xE9, 0x29, 0x41, 0x3C, 0xBF, 0xA8, 0x7B, 0x98, 0xD7, 0x00, 0x89,
            0x38, 0xF4, 0x34, 0xFE, 0x44, 0xDC, 0x22, 0x13, 0xA0, 0x63, 0x9A, 0x16,
            0x8C, 0x17, 0xFE, 0xCC, 0x6A, 0x0A, 0xC2, 0xBD, 0xDD, 0xCC, 0x01, 0xA1,
            0x74, 0xD8, 0x9B, 0x7D, 0x03, 0x36, 0x71, 0xDC, 0x9B, 0xF0, 0x0A, 0x83,
            0x6E, 0x99, 0xB4, 0x75, 0x6A, 0x79, 0x0C, 0x23, 0xD0, 0xFF, 0x69, 0x9B,
            0xB8, 0xE0, 0xCA, 0x58, 0x8F, 0xFA, 0x77, 0x9B, 0x2A, 0xE0, 0xE9, 0xA0,
            0xA1, 0xD7, 0xD9, 0x80, 0x2A, 0xDB, 0xD4, 0x51, 0x05, 0xA2, 0xDA, 0xAA,
            0xA8, 0xC3, 0x3C, 0x54, 0x4E, 0xA8, 0x48, 0xEF, 0x1B, 0xC7, 0x1B, 0x7D,
            0x04, 0xF1, 0xEF, 0x2B, 0xDE, 0x28, 0x85, 0xB5, 0x6C, 0x3A, 0x8B, 0x7A,
            0x37, 0x6D, 0x3D, 0x35, 0x7F, 0x69, 0xE0, 0x0C, 0x97, 0x79, 0x42, 0x47,
            0x23, 0xB4, 0x2F, 0x62, 0xCF, 0x03, 0x2B, 0x92, 0x62, 0xD6, 0x83, 0xF6,
            0x52, 0xC2, 0xF7, 0xF9, 0x83, 0xDB, 0x19, 0xB0, 0x91, 0xBD, 0x07, 0xA3,
            0x21, 0xFF, 0x83, 0xD0, 0x14, 0xB0, 0xC4, 0x4D, 0xB6, 0x14, 0x34, 0x9E,
            0x45, 0x7A, 0xA8, 0xB8, 0x87, 0xF5, 0xD5, 0xF4, 0xD6, 0x1C, 0x31, 0xB6,
            0x6D, 0x04, 0x13, 0x64, 0xEA, 0xDD, 0x6F, 0x1D, 0x04, 0xE8, 0xA3, 0x22,
            0x72, 0x9F, 0xD9, 0x05, 0xB3, 0xEF, 0x75, 0x89, 0xAD, 0xEE, 0x84, 0x2D,
            0x1F, 0x33, 0xB4, 0xCD, 0x27, 0xF7, 0xDF, 0xF0, 0xC0, 0x52, 0x09, 0x8F,
            0xED, 0x30, 0x68, 0x77, 0xC4, 0x98, 0x50, 0x89, 0x98, 0x4A, 0x67, 0xC7,
            0x8F, 0x50, 0xB3, 0x06, 0x50, 0x2C, 0x90, 0x28, 0xF2, 0xB8, 0x32, 0x5B,
            0x8B, 0x1C, 0x7F, 0xE3, 0x7B, 0xB8, 0xCB, 0x39, 0xE1, 0xEE, 0x39, 0x58,
            0x91, 0xF6, 0x34, 0xD8, 0xAE, 0xF8, 0x50, 0x0B, 0x3C, 0x99, 0x09, 0x58,
            0xCC, 0x6C, 0xA7, 0xC2, 0x4B, 0x19, 0x3C, 0x5B, 0x94, 0xE7, 0x75, 0xA9,
            0xB6, 0xB9, 0x4E, 0x60, 0x86, 0xBB, 0x35, 0x8F, 0xEF, 0xC3, 0x66, 0x4F,
            0x5D, 0x98, 0x8F, 0xF7, 0xBE, 0xD9, 0x88, 0xD0, 0x4A, 0x54, 0x9A, 0x78,
            0x35, 0x36, 0x6B, 0x26, 0xD5, 0x22, 0x76, 0x13, 0x67, 0xD5, 0xFC, 0xFA,
            0xA5, 0xAC, 0xD3, 0x03, 0x40, 0xD3, 0x8F, 0xB8, 0x2A, 0xAE, 0xEA, 0xD1,
            0xBE, 0xFC, 0x3C, 0x10, 0xEB, 0x6D, 0xF5, 0x90, 0x8D, 0xA9, 0xF9, 0xD8,
            0x90, 0x54, 0xCF, 0x74, 0x04, 0x2A, 0x60, 0xA0, 0xD4, 0x13, 0x56, 0xA2,
            0xEF, 0x12, 0x35, 0x16, 0x76, 0xCA, 0x6A, 0x5D, 0x12, 0x6B, 0xCA, 0x71,
            0x99, 0x03, 0xF7, 0x91, 0x1D, 0xF6, 0x53, 0x4F, 0x8E, 0xB5, 0x3A, 0x88,
            0x16, 0x58, 0x57, 0xFE, 0xAC, 0x0B, 0xE8, 0xE2, 0x4A, 0x1F, 0x4A, 0x80,
            0xF9, 0x0C, 0xA3, 0x0A, 0xC9, 0x6C, 0xE4, 0xD1, 0x61, 0xD9, 0x05, 0x09,
            0xD2, 0xD1, 0x57, 0xBF, 0xCC, 0x97, 0xD0, 0x23, 0xE2, 0x82, 0x46, 0x47,
            0x5B, 0x9F, 0xFB, 0xB1, 0xFA, 0xDE, 0xBC, 0x93, 0x1A, 0xE6, 0x03, 0x38,
            0x7E, 0x26, 0xD3, 0xEF, 0x28, 0x7F, 0x94, 0xFD, 0x4B, 0xDF, 0xA4, 0xBD,
            0x03, 0x5B, 0x1C, 0x69, 0x4A, 0x04, 0x7D, 0xBD, 0xBC, 0xF2, 0xEB, 0x31,
            0x01, 0x6D, 0xED, 0x9E, 0x40, 0xFB, 0x06, 0xAC, 0xF4, 0x1E, 0xC7, 0xD1,
            0x69, 0x11, 0x72, 0xB0, 0x62, 0xE5, 0x3F, 0x8C, 0x8F, 0x8C, 0xD4, 0xF9,
            0x52, 0xC4, 0xEF, 0xA6, 0xB1, 0xC4, 0xC7, 0xA5, 0xD4, 0x9F, 0x04, 0xDE,
            0x8B, 0xE9, 0xBE, 0x94, 0xCE, 0x19, 0x78, 0x98, 0xBB, 0xCF, 0x1C, 0x64,
            0x41, 0x4B, 0xAB, 0xA6, 0xA3, 0x3F, 0x97, 0x93, 0xF4, 0xEC, 0xD5, 0x41,
            0x03, 0x37, 0xC9, 0xFE, 0x28, 0xC3, 0x54, 0x35, 0xA0, 0x6E, 0x7C, 0xC6,
            0xBC, 0x0D, 0xCE, 0x04, 0x20, 0xFD, 0x3D, 0x91, 0x89, 0x9D, 0xB9, 0x31,
            0xC0, 0x1F, 0x37, 0xED, 0x89, 0xFF, 0xCB, 0xDE, 0xC8, 0x3A, 0x1C, 0x19,
            0x4A, 0xA6, 0xE8, 0x70, 0x50, 0xD7, 0xCF, 0x19, 0x72, 0xE4, 0x82, 0x7E,
            0xA7, 0xF3, 0x97, 0xDD, 0x6F, 0x1B, 0x9A, 0xF2, 0x6C, 0xD4, 0x26, 0xB8,
            0x54, 0xBA, 0xB9, 0x0B, 0xCA, 0x02, 0x1D, 0x19, 0xFD, 0x61, 0xBF, 0x7A,
            0xB2, 0xB0, 0xA7, 0xB1, 0x80, 0xDF, 0x91, 0x2E, 0xD1, 0x29, 0x9E, 0xD0,
            0x2C, 0xEA, 0xFD, 0x01, 0x2C, 0x52, 0xA6, 0x4A, 0xE2, 0x7D, 0x8B, 0x2D,
            0xE5, 0xBF, 0x8B, 0x17, 0x3B, 0x07, 0xFE, 0x29, 0x45, 0xE2, 0x6A, 0x31,
            0x9B, 0xB0, 0x07, 0xC5, 0xC4, 0xA4, 0xB5, 0x78, 0xCB, 0x88, 0x79, 0x6C,
            0x2F, 0x66, 0x39, 0x04, 0xBE, 0x62, 0xA2, 0x45, 0x90, 0xA0, 0x8A, 0x95,
            0xC8, 0x33, 0xDE, 0x91, 0xE7, 0x87, 0xAF, 0x22, 0x94, 0x0E, 0xAD, 0x6E,
            0x41, 0xD7, 0x54, 0x9C, 0xAF, 0x4B, 0x75, 0x4B, 0x99, 0x1E, 0xC4, 0xEB,
            0xC5, 0x40, 0xA3, 0x6B, 0x3A, 0x88, 0xDC, 0x9A, 0xD8, 0x4A, 0x89, 0x56,
            0x4A, 0x9E, 0x86, 0xBA, 0x33, 0xAB, 0x3A, 0x98, 0x51, 0x54, 0xD3, 0x58,
            0x6F, 0x35, 0x80, 0xB6, 0x15, 0xD3, 0xBA, 0xDA, 0x41, 0x5D, 0x9B, 0xC1,
            0x9A, 0xFF, 0xCF, 0x61, 0x6D, 0x10, 0x87, 0x39, 0x4A, 0x6A, 0x78, 0xB4,
            0x16, 0x9D, 0x2C, 0x97, 0x18, 0xFF, 0xF8, 0xF0, 0xC8, 0x45, 0x67, 0x36,
            0x30, 0x56, 0x6F, 0x7B, 0xA2, 0x66, 0x2B, 0xD7, 0xD3, 0x8D, 0x15, 0x40
        };

        public static byte[] bl1Key = {
            0xE1, 0xF9, 0x6C, 0x85, 0x66, 0x61, 0xE1, 0x96, 0x7A, 0x19, 0x8A, 0x57, 0x75, 0xFB, 0xE4, 0xEA
        };

        byte[] rgbuildKey = {
            0x52, 0x47, 0x42, 0x75, 0x69, 0x6C, 0x64
        };

        public Main()
        {
            main = this;
            InitializeComponent();
            lvLoaders.ListViewItemSorter = sorter;

            Shared.rc4(rgbuildKey, ref prvKey);
            Shared.rc4(rgbuildKey, ref bl1Key);

            Console.WriteLine(Shared.BytesToHexString(prvKey, ""));
            Console.WriteLine(Shared.BytesToHexString(bl1Key, ""));
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "bin";
            ofd.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            ofd.Title = "Open binary data";
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[][] keys = new byte[Program.StoredKeys.Count][];
                for(int i = 0; i < Program.StoredKeys.Count; i++)
                    keys[i] =
                        Shared.HexStringToBytes(Program.StoredKeys[i].Split(new[] {"|-|"}, StringSplitOptions.None)[1]);
                byte[] key = NANDImage.CheckKeysAgainstImage(ofd.FileName, keys);
                Image = new NANDImage();

                // check W1 nand
                FileStream fs = File.OpenRead(ofd.FileName);
                byte[] data = new byte[0x2];
                fs.Read(data, 0, data.Length);
                fs.Close();
                if (data[1] == 0x3F)
                {
                    Image.CPUKey = new byte[0x10];
                    Image._1BLKey = new byte[0x10];
                } 
                else if (key == null || key.Length != 0x10)
                {
                    OpenImageDialog dia = new OpenImageDialog(ofd.FileName);
                    if (dia.ShowDialog() != DialogResult.OK)
                        return;
                    Image.CPUKey = dia.CPUKey;
                    Image._1BLKey = dia.BLKey;
                }
                else
                {
                    Image.CPUKey = key;
                }

                Image.OpenImage(ofd.FileName, 0x200);
                refreshBootloaders();
            }
        }

        private void refreshBootloaders()
        {
            lvLoaders.Items.Clear();
            if (Image.Header != null)
            {
                ListViewItem header = new ListViewItem("Header");
                header.SubItems.Add(Image.Header.Build.ToString());
                header.SubItems.Add("0x0");
                header.SubItems.Add("0x80");

                header.Tag = Image.Header;
                lvLoaders.Items.Add(header);
            }
            if (Image.Payloads != null)
            {
                ListViewItem payloads = new ListViewItem("Payload list");
                payloads.SubItems.Add(Image.Header.Build.ToString());
                payloads.SubItems.Add("0x80");
                payloads.SubItems.Add("0x" + (Image.Payloads.Length + 4).ToString("X"));
                lvLoaders.Items.Add(payloads);
                foreach (RGBPayloadEntry payload in Image.Payloads.Payloads)
                {
                    ListViewItem item = new ListViewItem(payload.Description);
                    item.SubItems.Add("Custom payload");
                    item.SubItems.Add("0x" + (chOffsetWithSpare.Checked && ((NANDImageStream)Image.IO.Stream).SpareDataType != SpareDataType.None ? ((payload.Address / 0x200) * 0x210) : payload.Address).ToString("X"));
                    item.SubItems.Add("0x" + payload.Size.ToString("X"));
                    item.Tag = payload;
                    lvLoaders.Items.Add(item);
                }
            }
            if (Image.SMC != null)
            {
                ListViewItem smc = new ListViewItem("SMC");
                smc.SubItems.Add("?");
                smc.SubItems.Add("0x" + (chOffsetWithSpare.Checked && ((NANDImageStream)Image.IO.Stream).SpareDataType != SpareDataType.None ? ((Image.Header.SmcAddress / 0x200) * 0x210) : Image.Header.SmcAddress).ToString("X"));
                smc.SubItems.Add("0x" + Image.Header.SmcSize.ToString("X"));
                smc.Tag = Image.SMC;
                lvLoaders.Items.Add(smc);
            }
            if (Image.KeyVault != null)
            {
                ListViewItem kv = new ListViewItem("KeyVault");
                kv.SubItems.Add("0x" + Image.Header.KeyVaultVersion.ToString("X"));
                kv.SubItems.Add("0x" + (chOffsetWithSpare.Checked && ((NANDImageStream)Image.IO.Stream).SpareDataType != SpareDataType.None ? ((Image.Header.KeyVaultAddress / 0x200) * 0x210) : Image.Header.KeyVaultAddress).ToString("X"));
                kv.SubItems.Add("0x" + Image.Header.KeyVaultSize.ToString("X"));
                kv.Tag = Image.KeyVault;
                lvLoaders.Items.Add(kv);
            }
            foreach (Bootloader bootloader in Image.Bootloaders)
            {
                ListViewItem item = new ListViewItem(getBootloaderName(bootloader));
                item.SubItems.Add(bootloader.Build.ToString());
                item.SubItems.Add("0x" + (chOffsetWithSpare.Checked && ((NANDImageStream)Image.IO.Stream).SpareDataType != SpareDataType.None ? ((bootloader.Position / 0x200) * 0x210) : bootloader.Position).ToString("X"));
                item.SubItems.Add("0x" + bootloader.Size.ToString("X"));
                item.SubItems.Add(bootloader.SecureType.ToString());
                item.Tag = bootloader;
                lvLoaders.Items.Add(item);
            }
            foreach(FileSystemRoot fs in Image.FileSystems)
            {
                ListViewItem item = new ListViewItem("FileSystem");
                int loc = (int)(fs.BlockNumber * ((NANDImageStream)Image.IO.Stream).BlockLength);
                item.SubItems.Add(fs.Version.ToString());
                item.SubItems.Add("0x" + (chOffsetWithSpare.Checked && ((NANDImageStream)Image.IO.Stream).SpareDataType != SpareDataType.None ? ((loc / 0x200) * 0x210) : loc).ToString("X"));
                item.SubItems.Add("0x" + ((NANDImageStream) Image.IO.Stream).BlockLength.ToString("X"));
                item.Tag = fs;
                lvLoaders.Items.Add(item);
            }
            foreach(MobileXFile xfile in Image.MobileData)
            {
                ListViewItem item = new ListViewItem(xfile.Type.ToString());
                item.SubItems.Add(xfile.Version.ToString());
                int loc = xfile.StartPage * ((NANDImageStream)Image.IO.Stream).PageLength;
                int size = xfile.PageCount * ((NANDImageStream)Image.IO.Stream).PageLength;
                item.SubItems.Add("0x" + (chOffsetWithSpare.Checked && ((NANDImageStream)Image.IO.Stream).SpareDataType != SpareDataType.None ? ((loc / 0x200) * 0x210) : loc).ToString("X") + " (p:0x" + xfile.StartPage.ToString("X") + ")");
                item.SubItems.Add("0x" + size.ToString("X"));
                item.Tag = xfile;
                lvLoaders.Items.Add(item);
            }
           /* if (Image.CurrentFileSystem != null && Image.CurrentFileSystem.Entries != null)
            {
                foreach (FileSystemEntry entry in Image.CurrentFileSystem.Entries)
                {
                    if (entry.Deleted)
                        continue;
                    ListViewItem item = new ListViewItem(entry.FileName);
                    item.SubItems.Add("");
                    int loc = entry.BlockNumber*(int) ((NANDImageStream) Image.IO.Stream).BlockLength;
                    item.SubItems.Add("0x" + loc.ToString("X"));
                    item.SubItems.Add("0x" + entry.Size.ToString("X"));
                    item.Tag = Image.CurrentFileSystem;
                    lvLoaders.Items.Add(item);
                }
            }*/
            if(Image.ConfigBlock != null)
            {
                ListViewItem item = new ListViewItem("SMC config");
                item.SubItems.Add("");
                int loc = (int)((NANDImageStream)Image.IO.Stream).ConfigBlockStart * (int)((NANDImageStream)Image.IO.Stream).BlockLength;
                int size = 4 * (int)((NANDImageStream)Image.IO.Stream).BlockLength;
                item.SubItems.Add("0x" + (chOffsetWithSpare.Checked && ((NANDImageStream)Image.IO.Stream).SpareDataType != SpareDataType.None ? ((loc / 0x200) * 0x210) : loc).ToString("X") + " (b:0x" + ((NANDImageStream)Image.IO.Stream).ConfigBlockStart.ToString("X") + ")");
                item.SubItems.Add("0x" + size.ToString("X"));
                item.Tag = Image.ConfigBlock;
                lvLoaders.Items.Add(item);
            }
            lvLoaders.Sort();
        }
        private string getBootloaderName(Bootloader bootloader)
        {
            string name = bootloader.Magic.ToString();
            name = name.Replace("_", "");
            if (bootloader.GetType() == typeof(Bootloader2BL) && !bootloader.Magic.ToString().Substring(0, bootloader.Magic.ToString().Length - 1).EndsWith("S"))
                if (((Bootloader2BL)bootloader).CPUKey != null)
                    name += "_B";
                else
                    name += "_A";
            return name;
        }
        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog {ShowNewFolderButton = true};
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                
                
                foreach (ListViewItem lvi in lvLoaders.SelectedItems)
                {
                    bool use_cpu_key = true;
                    //for (int i = 0; i < 16; i++) if (Image.CPUKey[i] != 0) use_cpu_key = false;
                    if (lvi.Tag.GetType().BaseType == typeof(Bootloader))
                    {
                        Bootloader bl = (Bootloader)lvi.Tag;
                        File.WriteAllBytes(Path.Combine(fbd.SelectedPath, getBootloaderName(bl) + "." + bl.Build + ".bin"), bl.GetData(use_cpu_key));
                    }
                    else if (lvi.Tag.GetType() == typeof(KeyVault))
                    {
                        KeyVault kv = (KeyVault)lvi.Tag;
                        File.WriteAllBytes(Path.Combine(fbd.SelectedPath, "KV_dec.bin"), kv.GetData(true));
                        File.WriteAllBytes(Path.Combine(fbd.SelectedPath, "KV.bin"), kv.GetData(false));
                    }
                    else if (lvi.Tag.GetType() == typeof(SMC))
                    {
                        SMC smc = (SMC)lvi.Tag;
                        File.WriteAllBytes(Path.Combine(fbd.SelectedPath, "SMC_dec.bin"), smc.GetData(true));
                        File.WriteAllBytes(Path.Combine(fbd.SelectedPath, "SMC.bin"), smc.GetData(false));
                    }
                    else if (lvi.Tag.GetType() == typeof(MobileXFile))
                    {
                        MobileXFile file = (MobileXFile) lvi.Tag;
                        File.WriteAllBytes(Path.Combine(fbd.SelectedPath, file.Type.ToString() + ".bin"), file.GetData());
                    }
                    else if (lvi.Tag.GetType() == typeof(byte[]))
                    {
                        // config block
                        File.WriteAllBytes(Path.Combine(fbd.SelectedPath, "smc_config.bin"), ((byte[])lvi.Tag));
                    }
                }
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvLoaders.SelectedItems.Count > 1)
            {
                MessageBox.Show("Only one loader can be replaced at a time.");
                return;
            }
            ListViewItem lvi = lvLoaders.SelectedItems[0];
            if (lvi.Tag.GetType().BaseType == typeof(Bootloader))
            {
                Bootloader bl = (Bootloader)lvi.Tag;
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byte[] filedata = File.ReadAllBytes(ofd.FileName);
                    X360IO peekio = new X360IO(filedata, true);
                    NANDBootloaderMagic magic = (NANDBootloaderMagic)peekio.Reader.ReadUInt16();
                    if (magic != bl.Magic)
                        if (MessageBox.Show(
                            "You are attempting to replace with a different stage loader. Are you sure you want to do this?",
                            "Replace?", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    peekio.Close();
                    bl.SetData(filedata);
                    refreshBootloaders();
                }
            }
            if(lvi.Tag.GetType() == typeof(MobileXFile))
            {
                MobileXFile file = (MobileXFile) lvi.Tag;
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byte[] filedata = File.ReadAllBytes(ofd.FileName);
                    file.SetData(filedata);
                    refreshBootloaders();
                }
            }
            if (lvi.Tag.GetType() == typeof(SMC))
            {
                SMC smc = (SMC) lvi.Tag;
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byte[] filedata = File.ReadAllBytes(ofd.FileName);
                    if (filedata.Length != smc.GetData(true).Length)
                    {
                        MessageBox.Show("Invalid SMC");
                        return;
                    }
                    smc.SetData(filedata);
                    refreshBootloaders();
                }
            }
            if (lvi.Tag.GetType() == typeof(KeyVault))
            {
                KeyVault kv = (KeyVault) lvi.Tag;
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byte[] filedata = File.ReadAllBytes(ofd.FileName);
                    //byte[] data = new byte[0x3ff0];
                    //Array.Copy(filedata, filedata.Length - 0x3ff0, data, 0x0, 0x3ff0);
                    //kv.SetData(data);
                    kv.SetData(filedata);
                    refreshBootloaders();
                }
            }
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.SaveImage();
            Image.Close();
            if(((NANDImageStream)Image.IO.Stream).TempReturnData != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Please choose where to save this image";
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, ((NANDImageStream)Image.IO.Stream).TempReturnData);
                }
                ((NANDImageStream)Image.IO.Stream).TempReturnData = null;
            }
            Image = null;
            // ui stuff
            lvLoaders.Items.Clear();
            scContainer.Panel2.Controls.Clear();
        }
        
        private void dumpBootloaderInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder str = new StringBuilder();
            if(Image.Header != null)
            {
                str.AppendLine("---------------------------------------------------------------------------------");
                str.AppendLine("Xbox 360 flash image, copyright" + Image.Header.Copyright);
                str.AppendLine("Build: " + Image.Header.Build + ", qfe: 0x" +
                               Image.Header.Qfe.ToString("X") + ", flags: 0x" + Image.Header.Flags.ToString("X") +
                               ", entrypoint: 0x" +
                               Image.Header.Entrypoint.ToString("X") + ", size: 0x" + Image.Header.Size.ToString("X"));
                str.AppendLine("---------------------------------------------------------------------------------");
                str.AppendLine("CPU key:");
                str.AppendLine("\t" + Shared.BytesToHexString(Image.CPUKey, " "));
                str.AppendLine();
                str.AppendLine("Reserved:");
                str.AppendLine("\t" + Shared.BytesToHexString(Image.Header.Reserved, " "));
                str.AppendLine();
                str.AppendLine("KeyVault size: 0x" + Image.Header.KeyVaultSize.ToString("X"));
                str.AppendLine();
                str.AppendLine("SysUpdate addr: 0x" + Image.Header.SysUpdateAddress.ToString("X"));
                str.AppendLine();
                str.AppendLine("SysUpdate count: " + Image.Header.SysUpdateCount);
                str.AppendLine();
                str.AppendLine("KeyVault version: 0x" + Image.Header.KeyVaultVersion.ToString("X"));
                str.AppendLine();
                str.AppendLine("KeyVault addr: 0x" + Image.Header.KeyVaultAddress.ToString("X"));
                str.AppendLine();
                str.AppendLine("FileSystem addr: 0x" + Image.Header.FileSystemAddress.ToString("X"));
                str.AppendLine();
                str.AppendLine("SMC config addr: 0x" + Image.Header.SmcConfigAddress.ToString("X"));
                str.AppendLine();
                str.AppendLine("SMC size: 0x" + Image.Header.SmcSize.ToString("X"));
                str.AppendLine();
                str.AppendLine("SMC address: 0x" + Image.Header.SmcAddress.ToString("X"));
                str.AppendLine();
            }
            foreach (Bootloader bl in Image.Bootloaders)
            {
                str.AppendLine("---------------------------------------------------------------------------------");
                str.AppendLine(getBootloaderName(bl) + " bootloader");
                str.AppendLine("Build: " + bl.Build + ", qfe: 0x" +
                               bl.Qfe.ToString("X") + ", flags: 0x" + bl.Flags.ToString("X") + ", entrypoint: 0x" +
                               bl.Entrypoint.ToString("X") + ", size: 0x" + bl.Size.ToString("X"));
                str.AppendLine("---------------------------------------------------------------------------------");
                switch (bl.Magic)
                {
                    /*
                         * 
    public byte[] Signature = new byte[0x100]; // 0x100
    public byte[] AesInvData = new byte[0x110]; // 0x110
    public byte[] RsaPublicKey = new byte[0x110]; // 0x110
                         * */
                    case NANDBootloaderMagic.CB:
                    case NANDBootloaderMagic.SB:
                    case NANDBootloaderMagic.S2:
                    case NANDBootloaderMagic.C2:
                    case NANDBootloaderMagic._S2:
                        Bootloader2BL bl2 = (Bootloader2BL)bl;
                        str.AppendLine("POST output addr: 0x" + bl2.POSTOutputAddress.ToString("X") +
                                       ", Southbridge flash addr: 0x" + bl2.SbFlashAddress.ToString("X") +
                                       ", Soc MMIO addr: 0x" + bl2.SocMmioAddress.ToString("X"));
                        str.AppendLine();
                        str.AppendLine("3BL nonce:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl2.Nonce3BL, " "));
                        str.AppendLine();
                        str.AppendLine("3BL salt:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl2.Salt3BL, " "));
                        str.AppendLine();
                        str.AppendLine("4BL salt:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl2.Salt4BL, " "));
                        str.AppendLine();
                        str.AppendLine("4BL digest:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl2.Digest4BL, " "));
                        str.AppendLine();
                        str.AppendLine("Padding: 0x" + bl2.Padding.ToString("X8"));
                        str.AppendLine();
                        str.AppendLine("Console type and sequence allow data:");
                        str.AppendLine("\t" + "Console type: 0x" +
                                       bl2.ConsoleTypeSeqAllowData.ConsoleType.ToString("X2"));
                        str.AppendLine();
                        str.AppendLine("\t" + "Console sequence: 0x" +
                                       bl2.ConsoleTypeSeqAllowData.ConsoleSequence.ToString("X2"));
                        str.AppendLine();
                        str.AppendLine("\t" + "Console sequence allow: 0x" +
                                       bl2.ConsoleTypeSeqAllowData.ConsoleSequenceAllow.ToString("X4"));
                        str.AppendLine();
                        str.AppendLine("Per-box data:");
                        str.AppendLine("\t" + "Pairing data: 0x" +
                                       Shared.BytesToHexString(bl2.PerBoxData.PairingData, ""));
                        str.AppendLine();
                        str.AppendLine("\t" + "Lockdown value: 0x" +
                                       bl2.PerBoxData.LockDownValue.ToString("X"));
                        str.AppendLine();
                        str.AppendLine("\t" + "Reserved:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl2.PerBoxData.Reserved, " "));
                        str.AppendLine();
                        str.AppendLine("\t" + "Per-box digest:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl2.PerBoxData.PerBoxDigest, " "));
                        str.AppendLine();
                        break;
                    /*
                         *         public byte[] Signature = new byte[0x100]; // 0x100
    public byte[] RsaPublicKey = new byte[0x110]; // 0x110
                         * */
                    case NANDBootloaderMagic.CD:
                    case NANDBootloaderMagic.SD:
                    case NANDBootloaderMagic.S4:
                    case NANDBootloaderMagic.C4:
                    case NANDBootloaderMagic._S4:
                        Bootloader4BL bl4 = (Bootloader4BL)bl;
                        str.AppendLine("6BL nonce:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl4.Nonce6BL, " "));
                        str.AppendLine();
                        str.AppendLine("6BL salt:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl4.Salt6BL, " "));
                        str.AppendLine();
                        str.AppendLine("Padding: 0x" + bl4.Padding.ToString("X8"));
                        str.AppendLine();
                        str.AppendLine("5BL digest:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl4.Digest5BL, " "));
                        str.AppendLine();
                        break;
                    case NANDBootloaderMagic.CE:
                    case NANDBootloaderMagic.SE:
                    case NANDBootloaderMagic.S5:
                    case NANDBootloaderMagic.C5:
                        Bootloader5BL bl5 = (Bootloader5BL)bl;
                        str.AppendLine("Image address: 0x" + bl5.ImageAddress.ToString("X16"));
                        str.AppendLine();
                        str.AppendLine("Image size: 0x" + bl5.ImageSize.ToString("X8"));
                        str.AppendLine();
                        str.AppendLine("Padding: 0x" + bl5.Padding.ToString("X8"));
                        str.AppendLine();
                        break;
                    /*
                         *         
    public byte[] Signature = new byte[0x100]; // 0x100
                         * */
                    case NANDBootloaderMagic.CF:
                    case NANDBootloaderMagic.SF:
                        Bootloader6BL bl6 = (Bootloader6BL)bl;
                        str.AppendLine("Build QFE source: " + bl6.BuildQfeSource);
                        str.AppendLine();
                        str.AppendLine("Build QFE target: " + bl6.BuildQfeTarget);
                        str.AppendLine();
                        str.AppendLine("Reserved: " + bl6.Reserved);
                        str.AppendLine();
                        str.AppendLine("7BL size: " + bl6.Size7BL);
                        str.AppendLine();
                        str.AppendLine("7BL nonce:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl6.Nonce7BL, " "));
                        str.AppendLine();
                        str.AppendLine("7BL digest:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl6.Digest7BL, " "));
                        str.AppendLine();
                        str.AppendLine("Padding: 0x" + bl6.Padding.ToString("X8"));
                        str.AppendLine();
                        str.AppendLine("7BL per-box data:");
                        str.AppendLine("\tUsed block count: " +
                                       bl6.PerBoxData7BL.UsedBlocksCount);
                        str.AppendLine();
                        str.AppendLine("\tUsed blocks:");
                        string data = "\t";
                        for (int i = 0; i < bl6.PerBoxData7BL.UsedBlocksCount; i++)
                        {
                            data += "0x" + bl6.PerBoxData7BL.BlockNumbers[i] + "  ";
                            if (i != 0 && i % 10 == 0)
                                data += "\r\n\t";
                        }
                        str.Append(data + "\r\n");
                        str.AppendLine();
                        str.AppendLine("6BL per-box data:");
                        str.AppendLine("\tReserved:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl6.PerBoxData.Reserved, " "));
                        str.AppendLine();
                        str.AppendLine("\tUpdate slot number: 0x" +
                                       bl6.PerBoxData.UpdateSlotNumber.ToString("X"));
                        str.AppendLine();
                        str.AppendLine("\tPairing data: 0x" + Shared.BytesToHexString(bl6.PerBoxData.PairingData, ""));
                        str.AppendLine();
                        str.AppendLine("\tLockdown value: 0x" + bl6.PerBoxData.LockDownValue.ToString("X"));
                        str.AppendLine();
                        str.AppendLine("\tPer-box digest:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl6.PerBoxData.PerBoxDigest, " "));
                        str.AppendLine();
                        break;
                    case NANDBootloaderMagic.CG:
                    case NANDBootloaderMagic.SG:
                        Bootloader7BL bl7 = (Bootloader7BL)bl;
                        str.AppendLine("Source image size: 0x" + bl7.SourceImageSize.ToString("X"));
                        str.AppendLine();
                        str.AppendLine("Source digest:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl7.SourceDigest, " "));
                        str.AppendLine();
                        str.AppendLine("Target image size: 0x" + bl7.TargetImageSize.ToString("X"));
                        str.AppendLine();
                        str.AppendLine("Target digest:");
                        str.AppendLine("\t" + Shared.BytesToHexString(bl7.TargetDigest, " "));
                        str.AppendLine();
                        break;
                }
            }
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, str.ToString());
            }
        }

        private void createImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateImageDialog dialog = new CreateImageDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Image = new NANDImage();
                Image.CPUKey = dialog.CPUKey;
                Image._1BLKey = dialog.BLKey;
                NANDImageStream stream = new NANDImageStream(new MemoryStream(new byte[dialog.ImgSize]), 0x200);
                //stream.SwapBlockIdx = dialog.SwapBlockIdx;
                Image.IO = new X360IO(stream, true);
                Image.CreateHeader();
                Image.Header.Entrypoint = (uint) dialog.BL2Addr;
                Image.Header.Size = (uint) dialog.SU0Addr;
                Image.Header.SysUpdateAddress = (uint) dialog.SU0Addr;
                Image.Header.Copyright = Image.Header.Copyright.Replace("2011", dialog.MfgYear);
                refreshBootloaders();
            }
        }

        
        private void addBootloaderToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image.AddBootloaderFromPath(ofd.FileName);
                refreshBootloaders();
            }
        }

        private void lvLoaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvLoaders.SelectedItems.Count != 1)
                return;
            ListViewItem item = lvLoaders.SelectedItems[0];
            scContainer.Panel2.Controls.Clear();
            try
            {
                if (item.Tag.GetType().BaseType == typeof(Bootloader))
                {
                    if (item.Tag.GetType() == typeof(Bootloader2BL))
                        scContainer.Panel2.Controls.Add(new Bootloader2BLControl((Bootloader2BL)item.Tag));
                    else if (item.Tag.GetType() == typeof(Bootloader6BL))
                        scContainer.Panel2.Controls.Add(new Bootloader6BLControl((Bootloader6BL)item.Tag));
                }
                else
                {
                    if (item.Tag.GetType() == typeof(FileSystemRoot))
                    {
                        scContainer.Panel2.Controls.Add(new FileSystemControl((FileSystemRoot)item.Tag));

                    }else if(item.Tag.GetType() == typeof(KeyVault)){
                        scContainer.Panel2.Controls.Add(new KeyVaultControl(Image.KeyVault));
                    }
                }
            }
            catch { }
        }

        private void lvLoaders_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == sorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (sorter.Order == SortOrder.Ascending)
                {
                    sorter.Order = SortOrder.Descending;
                }
                else
                {
                    sorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lvLoaders.Sort();
        }

        private void loadFromIniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            string ret = Program.LoadFromIni(ref Image, ofd.FileName);
            if (ret != null)
                MessageBox.Show(ret);
            refreshBootloaders();
        }

        private void addFileSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.CreateFileSystem();
            refreshBootloaders();
        }

        private void addSMCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(ofd.FileName);
                if(data.Length < 0x3000)
                {
                    MessageBox.Show("SMC is incorrect size.");
                    return;
                }
                if(Image.SMC == null)
                    Image.SMC = new SMC(Image.IO);
                Image.SMC.SetData(data);

                Image.Header.SmcSize = (uint)(Image.SMC.GetData(true).Length);
                Image.Header.SmcAddress = (uint)(0x4000 - Image.Header.SmcSize);
                refreshBootloaders();
            }
        }

        private void addKeyVaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(ofd.FileName);
                if (data.Length != 0x4000 && data.Length != 0x3ff0)
                {
                    MessageBox.Show("KeyVault is incorrect size.");
                    return;
                }
                byte[] data2 = new byte[0x3ff0];
                Array.Copy(data, data.Length == 0x4000 ? 0x10 : 0x0, data2, 0, 0x3ff0);
                if(Image.KeyVault == null)
                    Image.KeyVault = new KeyVault(Image.IO, Image.CPUKey);

                Image.KeyVault.SetData(data);
                refreshBootloaders();
            }
        }
        
        private void addMobileFile(FileSystemExEntries type)
        {
            if(Image.CurrentFileSystem == null)
            {
                MessageBox.Show("You need a file system first...");
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(ofd.FileName);
                Image.AddMobileFile(data, type);
                
                refreshBootloaders();
            }
        }
        private void addMobileBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileB);
        }

        private void addMobileCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileC);
        }

        private void addMobileDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileD);
        }

        private void addMobileEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileE);
        }

        private void addMobileFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileF);
        }

        private void addMobileGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileG);
        }

        private void addMobileHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileH);
        }

        private void addMobileIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileI);
        }

        private void addMobileJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMobileFile(FileSystemExEntries.MobileJ);
        }

        private void addSMCConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Image.CurrentFileSystem == null)
            {
                MessageBox.Show("You need a file system first...");
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(ofd.FileName);
                if (data.Length != 0x10000)
                {
                    MessageBox.Show("This SMC config is invalid.");
                    return;
                }
                Image.ConfigBlock = data;
                refreshBootloaders();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void closeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Image == null)
                return; // already closed

            ((NANDImageStream)Image.IO.Stream).Close(false);
            Image = null;
            lvLoaders.Items.Clear();
            scContainer.Panel2.Controls.Clear();

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("RGBuild "+Program.Version+"\nby stoker25, tydye81 and #RGLoader@EFnet");
        }

        private void addPayloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPayloadDialog dia = new AddPayloadDialog();
            if (dia.ShowDialog() != DialogResult.OK)
                return;
            Image.AddPayload(dia.Description, dia.Address, File.ReadAllBytes(dia.FilePath));
            refreshBootloaders();
        }

        private void decompressPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select base kernel 1888 (CE, NOT DECOMPRESSED)";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(ofd.FileName);
                data = XCompress.DecompressInChunks(data, 0x30);
                ofd.Title = "Select CG patch";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byte[] patchdata = File.ReadAllBytes(ofd.FileName);
                    byte[] trgtsize = new byte[] { patchdata[0x3B], patchdata[0x3A], patchdata[0x39], patchdata[0x38] };
                    uint size = BitConverter.ToUInt32(trgtsize, 0);
                    byte[] patcheddata = XCompress.DecompressPatchInChunks(data, patchdata, size);
                    SaveFileDialog sfd = new SaveFileDialog();
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(sfd.FileName, patcheddata);
                    }
                }
            }

        }

        private void extractBaseKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bootloader5BL bl5 = (Bootloader5BL)Image.Bootloaders.Find(f => f.GetType() == typeof(Bootloader5BL));
            if (bl5 == null)
            {
                MessageBox.Show("Not enough bootloaders for kernel");
                return;
            }
            byte[] kerneldata = XCompress.DecompressInChunks(bl5.GetData(), 0x30);
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            File.WriteAllBytes(sfd.FileName, kerneldata);
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK) {
                byte[] data = File.ReadAllBytes(ofd.FileName);
                Array.Copy(XeCrypt.XeCryptBnQwNeRsaPrvCrypt(XeCrypt.XeCryptBnQwBeSigCreate(XeCrypt.XeCryptRotSumSha(data, 0, 0x10, data, 0x120, data.Length - 0x120, 0x14), new byte[] { 0x58, 0x42, 0x4F, 0x58, 0x5F, 0x52, 0x4F, 0x4D, 0x5F, 0x34 }, prvKey), prvKey), 0, data, 0x20, 0x100);
                File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName) + ".signed" + Path.GetExtension(ofd.FileName)), data);
            }
        }

        private void scContainer_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Text = "RGBuild "+Program.Version;
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


            byte[][] keys = new byte[Program.StoredKeys.Count][];
            for (int index = 0; index < Program.StoredKeys.Count; ++index)
                keys[index] = Shared.HexStringToBytes(Program.StoredKeys[index].Split(new string[1] { "|-|" }, StringSplitOptions.None)[1]);
            byte[] numArray = NANDImage.CheckKeysAgainstImage(files[0], keys);
            this.Image = new NANDImage();

            // check W1 nand
            FileStream fs = File.OpenRead(files[0]);
            byte[] data = new byte[0x2];
            fs.Read(data, 0, data.Length);
            fs.Close();
            if (data[1] == 0x3F)
            {
                Image.CPUKey = new byte[0x10];
                Image._1BLKey = new byte[0x10];
            }
            else if (numArray == null || numArray.Length != 16)
            {
                OpenImageDialog openImageDialog = new OpenImageDialog(files[0]);
                if (openImageDialog.ShowDialog() != DialogResult.OK)
                    return;
                this.Image.CPUKey = openImageDialog.CPUKey;
                this.Image._1BLKey = openImageDialog.BLKey;
            }
            else
                this.Image.CPUKey = numArray;
            this.Image.OpenImage(files[0], 512);
            this.refreshBootloaders();
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void decryptChallengeFileToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK) {
                byte[] challenge_enc = File.ReadAllBytes(ofd.FileName);

                byte[] key_nonce = new byte[0x10];
                Array.Copy(challenge_enc, 0x10, key_nonce, 0, 0x10);

                byte[] challenge_payload = new byte[challenge_enc.Length - 0x20];
                Array.Copy(challenge_enc, 0x20, challenge_payload, 0, challenge_enc.Length - 0x20);

                Tuple<byte[], byte[]> decrypted = Shared.HmacRc4(bl1Key, key_nonce, challenge_payload);

                Console.WriteLine("Computed RC4 Key: " + Shared.BytesToHexString(decrypted.Item2, ""));

                Array.Copy(decrypted.Item1, 0, challenge_enc, 0x20, decrypted.Item1.Length);

                File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName) + ".dec" + Path.GetExtension(ofd.FileName)), challenge_enc);
            }
        }

        private void chOffsetWithSpare_CheckedChanged(object sender, EventArgs e) {
            refreshBootloaders();
        }

        public static bool isOffsetWithSpare() {
            return main.chOffsetWithSpare.Checked;
        }

        private void compressHVKernelToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Title = "Select uncompressed HV/Kernel binary";
            byte[] hvKernel = null; bool isDev = false; string seFilename = "";
            if (ofd1.ShowDialog() == DialogResult.OK) {
                hvKernel = File.ReadAllBytes(ofd1.FileName);
                seFilename = ofd1.FileName;

                isDev = hvKernel[0] == 0x5E;
                bool isRetail = hvKernel[0] == 0x4E;
                bool mzMagic = hvKernel[0x40000] == 0x4D && hvKernel[0x40001] == 0x5A;

                if (!isDev && !isRetail) {
                    MessageBox.Show("File does not start with valid HV magic.");
                    return;
                }

                if (!mzMagic) {
                    MessageBox.Show("File does not contain kernel pe header starting at 0x40000");
                    return;
                }
            }

            if (hvKernel == null) {
                return;
            }

            OpenFileDialog ofd2 = new OpenFileDialog();
            ofd2.Title = "Select decrypted SD";
            byte[] sd = null; string sdFilename = "";
            if (ofd2.ShowDialog() == DialogResult.OK) {
                sd = File.ReadAllBytes(ofd2.FileName);
                sdFilename = ofd2.FileName;

                bool isSd = sd[0] == 0x53 && sd[1] == 0x44;
                bool sdDecrypted = sd[0x240] == 0x58 && sd[0x24B] == 0x00;

                if (!isSd) {
                    MessageBox.Show("File does not start with SD");
                    return;
                }

                if (!sdDecrypted) {
                    MessageBox.Show("SD is not decrypted");
                    return;
                }
            }

            if(sd == null) {
                return;
            }

            OpenFileDialog ofd3 = new OpenFileDialog();
            ofd3.Title = "Select shadowboot";
            byte[] shadowboot = null; string shadowbootFilename = "";
            if (ofd3.ShowDialog() == DialogResult.OK) {
                shadowboot = File.ReadAllBytes(ofd3.FileName);
                shadowbootFilename = ofd3.FileName;

                bool isShadowboot = shadowboot[0] == 0xFF && shadowboot[1] == 0x4F;
                if (!isShadowboot) {
                    MessageBox.Show("Not a valid shadowboot magic");
                    return;
                }
            }

            if (shadowboot == null) {
                return;
            }

            byte[] compressed = XCompress.CompressInChunks(hvKernel);
            int padding = (16 - (compressed.Length % 16)) % 16;

            byte[] output = new byte[compressed.Length + padding + 0x30];
            output[0] = 0x53;
            output[1] = 0x45;
            Array.Copy(hvKernel, 2, output, 2, 2);
            if (isDev) {
                output[4] = 0x80;
            }

            byte[] shaNonce = new byte[0x10];
            new Random().NextBytes(shaNonce);
            Array.Copy(shaNonce, 0, output, 0x10, 0x10);
            output[0x29] = 0x20;
            byte[] size = BitConverter.GetBytes(compressed.Length + 0x30);
            Array.Reverse(size);
            Array.Copy(size, 0, output, 0xC, 4);
            Array.Copy(compressed, 0, output, 0x30, compressed.Length);

            byte[] digest = XeCrypt.XeCryptRotSumSha(output, 0, 0x10, output, 0x20, output.Length - 0x20, 0x14);

            Array.Copy(digest, 0, sd, 0x24C, 0x14);

            Array.Copy(XeCrypt.XeCryptBnQwNeRsaPrvCrypt(XeCrypt.XeCryptBnQwBeSigCreate(XeCrypt.XeCryptRotSumSha(sd, 0, 0x10, sd, 0x120, sd.Length - 0x120, 0x14), new byte[] { 0x58, 0x42, 0x4F, 0x58, 0x5F, 0x52, 0x4F, 0x4D, 0x5F, 0x34 }, prvKey), prvKey), 0, sd, 0x20, 0x100);

            byte[] blOffsetBytes = new byte[4];
            Array.Copy(shadowboot, 0x8, blOffsetBytes, 0, 0x4);
            Array.Reverse(blOffsetBytes);
            int blOffset = BitConverter.ToInt32(blOffsetBytes, 0);

            byte[] romSizeBytes = new byte[4];
            Array.Copy(shadowboot, 0xC, romSizeBytes, 0, 0x4);
            Array.Reverse(romSizeBytes);
            int romSize = BitConverter.ToInt32(romSizeBytes, 0);

            byte[] scRc4Key = null;
            byte[] zeroKey = new byte[0x10];

            // find SC Key and SD
            int currentOffset = blOffset;
            while(BitConverter.ToInt16(shadowboot, currentOffset) != 0x4453 && currentOffset < romSize) {
                if (BitConverter.ToInt16(shadowboot, currentOffset) == 0x4353) { // SC
                    byte[] scShaNonce = new byte[0x10];
                    Array.Copy(shadowboot, currentOffset + 0x10, scShaNonce, 0, 0x10);
                    scRc4Key = new byte[0x10];
                    Array.Copy(new HMACSHA1(zeroKey).ComputeHash(scShaNonce, 0, scShaNonce.Length), 0, scRc4Key, 0, 0x10);
                }

                blOffsetBytes = new byte[4];
                Array.Copy(shadowboot, currentOffset + 0xC, blOffsetBytes, 0, 0x4);
                Array.Reverse(blOffsetBytes);
                blOffset = BitConverter.ToInt32(blOffsetBytes, 0);
                currentOffset += blOffset;
            }

            if (scRc4Key == null) {
                MessageBox.Show("Failed to find SC in shadowboot");
                return;
            }

            if (currentOffset >= romSize) {
                MessageBox.Show("Failed to find SD in shadowboot");
                return;
            }

            // encrypt SD
            byte[] sdShaNonce = new byte[0x10];
            byte[] sdData = new byte[sd.Length - 0x20];
            Array.Copy(sd, 0x10, sdShaNonce, 0, 0x10);
            Array.Copy(sd, 0x20, sdData, 0, sdData.Length);
            Tuple<byte[], byte[]> sdEnc = Shared.HmacRc4(scRc4Key, sdShaNonce, sdData);
            Array.Copy(sdEnc.Item1, 0, sd, 0x20, sdData.Length);

            // encrypt SE
            byte[] seShaNonce = new byte[0x10];
            byte[] seData = new byte[output.Length - 0x20];
            Array.Copy(output, 0x10, seShaNonce, 0, 0x10);
            Array.Copy(output, 0x20, seData, 0, seData.Length);
            Tuple<byte[], byte[]> seEnc = Shared.HmacRc4(sdEnc.Item2, seShaNonce, seData);
            Array.Copy(seEnc.Item1, 0, output, 0x20, seData.Length);

            // copy compressed/encrypted bootloaders to shadowboot
            Array.Copy(sd, 0, shadowboot, currentOffset, sd.Length);
            Array.Copy(output, 0, shadowboot, currentOffset + sd.Length, output.Length);

            // write files
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(seFilename), Path.GetFileNameWithoutExtension(seFilename) + ".comp.enc" + Path.GetExtension(seFilename)), output); 
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(sdFilename), Path.GetFileNameWithoutExtension(sdFilename) + ".signed.enc" + Path.GetExtension(sdFilename)), sd);
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(shadowbootFilename), Path.GetFileNameWithoutExtension(shadowbootFilename) + ".patched" + Path.GetExtension(shadowbootFilename)), shadowboot);
        }

        private void decompressCeSeLToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK) {
                byte[] data = File.ReadAllBytes(ofd.FileName);
                File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName) + ".dec" + Path.GetExtension(ofd.FileName)), XCompress.DecompressInChunks(data, 0x30));
            }
        }
    }
}