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
    public partial class Frmhome : Form
    {
       public DirectoryInfo nodeDirInfo;
       private recordhistory[] Tableau_ipo  ;
        private int index;
        public Frmhome()
        {
            InitializeComponent();
            index = 0;
            Tableau_ipo = new recordhistory[0];
            loadFromSerial();
        
           
            if (File.Exists("enregistreFolders.bin") == true)
            {
                loadFoldersFromSerial(treeView1, "enregistreFolders.bin");
            }
            else
            { PopulateTreeView();
                SAveFoldersSerial(treeView1, "enregistreFolders.bin");
            }
            


          
            



        }
        private void PopulateTreeView()
        {
            TreeNode rootNode;

            //DirectoryInfo info = new DirectoryInfo(@"../../../../../..");
            DirectoryInfo info = new DirectoryInfo(@"//192.168.1.124/private/doc/Pluralsight/");
            DirectoryInfo info2 = new DirectoryInfo(@"//192.168.1.124/private/doc/udemy/");
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
            if (info2.Exists)
            {
                rootNode = new TreeNode(info2.Name);
                rootNode.Tag = info2;
                GetDirectories(info2.GetDirectories(), rootNode);
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
                catch (Exception ex) { System.Console.WriteLine(ex); }

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
                Array.Resize(ref Tableau_ipo, Tableau_ipo.Length + 1);
                
                Tableau_ipo[index]=elt;
                index = index + 1;
                listBox1.Items.Add(elt.path + elt.filename);

                // System.Console.WriteLine (Tableau_ipo.Length);

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
                index = Tableau_ipo.Length;
                System.Console.WriteLine(Tableau_ipo.Length);
            }
            foreach (recordhistory elt in Tableau_ipo)
            {
                listBox1.Items.Add(elt.path + elt.filename);
            }
        }
        private void SAve_Click() {
            FileStream f = File.Create("enregistre.bin");

            BinaryFormatter s = new BinaryFormatter();
            s.Serialize(f, Tableau_ipo);
            f.Close();
                }

      
       public static int loadFoldersFromSerial(TreeView tree, string filename)
        {
            if (File.Exists(filename))
            {
                // Datei öffnen
                Stream file = File.Open(filename, FileMode.Open);
                // Binär-Formatierer init.
                BinaryFormatter bf = new BinaryFormatter();
                // Object var. init.
                object obj = null;
                try
                {
                    // Daten aus der Datei deserialisieren
                    obj = bf.Deserialize(file);
                }
                catch (System.Runtime.Serialization.SerializationException e)
                {
                    MessageBox.Show("De-Serialization failed : {0}", e.Message);
                    return -1;
                }
                // Datei schliessen
                file.Close();

                // Neues Array erstellen
                ArrayList nodeList = obj as ArrayList;

                // load Root-Nodes
                foreach (TreeNode node in nodeList)
                {
                    tree.Nodes.Add(node);
                }
                return 0;

            }
            else return -2; // File existiert nicht
        }

        public static int SAveFoldersSerial(TreeView tree, string filename)
        {
            // Neues Array anlegen
            ArrayList al = new ArrayList();
            foreach (TreeNode tn in tree.Nodes)
            {
                // jede RootNode im TreeView sichern ...
                al.Add(tn);
            }

            // Datei anlegen
            Stream file = File.Open(filename, FileMode.Create);
            // Binär-Formatierer init.
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                // Serialisieren des Arrays
                bf.Serialize(file, al);
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                MessageBox.Show("Serialization failed : {0}", e.Message);
                return -1; // ERROR
            }

            // Datei schliessen
            file.Close();

            return 0; // OKAY
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PopulateTreeView();
            SAveFoldersSerial(treeView1, "enregistreFolders.bin");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                string filename = listBox1.GetItemText(listBox1.SelectedItem);
               
                string strCmdText;
                //strCmdText = "\"\\\\192.168.1.124\\private\\doc\\Pluralsight\\Architecting Azure Solutions (70-534)- Infrastructure and Networking\\0. Introduction to the Infrastructure and Networking Objective Domain\\" + listView1.SelectedItems[0].Text + "\"";
                strCmdText = "\"" +  filename + "\""; ;
                System.Diagnostics.Process.Start("C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe", strCmdText);
              

                SAve_Click();
            }
        }
    }

}
