using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Runtime.Serialization;

namespace videobrowsing
{
    public partial class Form1 : Form
    {
       public DirectoryInfo nodeDirInfo;
       private recordhistory[] Tableau_ipo  ;
        public Form1()
        {
            InitializeComponent();
       
            PopulateTreeView();
            loadFromSerial();
            Tableau_ipo = new recordhistory[100];


        }
        private void PopulateTreeView()
        {
            TreeNode rootNode;

            //DirectoryInfo info = new DirectoryInfo(@"../../../../../..");
            DirectoryInfo info = new DirectoryInfo(@"//192.168.1.124/private/doc/Pluralsight/");
        
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs,
            TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                try
                {
                    subSubDirs = subDir.GetDirectories();
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (Exception ex) { }

            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
           
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"),
             new ListViewItem.ListViewSubItem(item,
                dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                String filename1 = file.Name;
                if (filename1.Contains(".mp4"))
                    { 
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
                    new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString())
                    };

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
                }
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string filename = listView1.SelectedItems[0].Text;
                recordhistory elt = new recordhistory();
                string strCmdText;
                //strCmdText = "\"\\\\192.168.1.124\\private\\doc\\Pluralsight\\Architecting Azure Solutions (70-534)- Infrastructure and Networking\\0. Introduction to the Infrastructure and Networking Objective Domain\\" + listView1.SelectedItems[0].Text + "\"";
                strCmdText = "\"" + nodeDirInfo.FullName + "\\" + filename + "\""; ;
                System.Diagnostics.Process.Start("C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe", strCmdText);
                elt.filename = filename;
                elt.path = nodeDirInfo.FullName;
                Tableau_ipo.Append(elt);
                SAve_Click();
            }

        }

        private void loadFromSerial()
        {
            System.Console.WriteLine("ici on loadFromSerial");
            if (File.Exists("enregistre.bin")==true) {
                FileStream f = File.Open("enregistre.bin", FileMode.Open);
                BinaryFormatter s = new BinaryFormatter();
                Tableau_ipo = (recordhistory[]) s.Deserialize(f);
                f.Close();
            }        
        }
        private void SAve_Click() {
            FileStream f = File.Create("enregistre.bin");

            BinaryFormatter s = new BinaryFormatter();
            s.Serialize(f, Tableau_ipo);
            f.Close();
                }

     

    }
    
}
