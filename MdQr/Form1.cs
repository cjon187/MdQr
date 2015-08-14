using Npgsql;
using OnBarcode.Barcode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Cassandra;
namespace MdQr
{
    
    public partial class Form1 : Form
    {
        string fileName;
        string dd;

        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        

        public Form1()
        {
            InitializeComponent();
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            String fileName2;
            String camName = textBox1.Text.ToString();
            //fileName = String.Format(@"{0}\images\"+camName+ DateTime.Now.ToString("yyyy-MM-ddTHH'-'mm'-'")+".png", Application.StartupPath);
            //fileName2 = String.Format(@"{0}\images\" + camName + DateTime.Now.ToString("yyyy-MM-ddTHH'-'mm'-'") + "XX.png", Application.StartupPath);
            fileName = String.Format(@"{0}\images\"+camName+".png", Application.StartupPath);
            fileName2 = String.Format(@"{0}\images\" + camName + "XX.png", Application.StartupPath);


            //validation for ip and mac
            IPAddress address;

            if (!String.IsNullOrEmpty(camName))
            {

                string rot = camName;
                rot = Encrypt(rot);
                Console.WriteLine(rot);
                QRCode qrcode = new QRCode();
                qrcode.Data = rot;
                qrcode.X = 4;
                qrcode.drawBarcode(fileName);
                qrcode.drawBarcode(fileName2);
                pictureBox1.ImageLocation = fileName;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                //test
                rot = Decrypt(rot);
                Console.WriteLine(rot);
                //test


                string inputImage = String.Format(@"{0}\blank.png", Application.StartupPath);
                string outputImageFilePath = String.Format(@"{0}\temp.png", Application.StartupPath);
                string textToDisplayOnImage = camName;
                Bitmap imageInBitMap = new Bitmap(inputImage);
                Graphics imageGraphics = Graphics.FromImage(imageInBitMap);

                //Set the alignment based on the coordinates 
                StringFormat formatAssignment = new StringFormat();
                formatAssignment.Alignment = StringAlignment.Near;

                //Here we are going to assign the font color
                Color assignColorToString = System.Drawing.ColorTranslator.FromHtml("#000000");

                //Assigning font size, font family, position of the text to display and others.
                imageGraphics.DrawString(textToDisplayOnImage, new Font("Times new Roman", 8, FontStyle.Bold), new SolidBrush(assignColorToString), new Point(0, 0), formatAssignment);

                //saving in the computer with label
                imageInBitMap.Save(outputImageFilePath);
                imageInBitMap.Dispose();

                //join images
                String[] a = { fileName2, outputImageFilePath };
                Bitmap com = CombineBitmap(a);
                com.Save(fileName2);
                com.Dispose();
                imageGraphics.Dispose();
                imageGraphics.Dispose();

            }
            else
            {
                MessageBox.Show("Enter a Camera Name");
            }
        }




        public static System.Drawing.Bitmap CombineBitmap(string[] files)
        {
            //read all images into memory
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (string image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image);

                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image,
                          new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                throw ex;
            }
            finally
            {
                //clean up memory
                foreach (System.Drawing.Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fileName = String.Format(@"{0}\images\url.png", Application.StartupPath);
            string rot = "07-01-01-F";
            rot = Encrypt(rot);
            Console.WriteLine(rot);
            QRCode qrcode = new QRCode();
            qrcode.Data = rot;
            qrcode.X = 4;
            qrcode.drawBarcode(fileName);
            pictureBox1.ImageLocation = fileName;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;




        }

        private static string Encrypt(string raw)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                ICryptoTransform e = GetCryptoTransform(csp, true);
                byte[] inputBuffer = Encoding.UTF8.GetBytes(raw);
                byte[] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

                string encrypted = Convert.ToBase64String(output);

                return encrypted;
            }
        }

        public static string Decrypt(string encrypted)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                var d = GetCryptoTransform(csp, false);
                byte[] output = Convert.FromBase64String(encrypted);
                byte[] decryptedOutput = d.TransformFinalBlock(output, 0, output.Length);

                string decypted = Encoding.UTF8.GetString(decryptedOutput);
                return decypted;
            }
        }

        private static ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            var passWord = "Pass@word1";
            var salt = "S@1tS@lt";

            //a random Init. Vector. just for testing
            String iv = "e675f725e675f725";

            var spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(passWord), Encoding.UTF8.GetBytes(salt), 65536);
            byte[] key = spec.GetBytes(16);


            csp.IV = Encoding.UTF8.GetBytes(iv);
            csp.Key = key;
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPage1)
            {
                refresh();
            }
            if (e.TabPage == tabPage2)
            {
                
            }
        }

        //pg connect on load
        public void refresh()
        {
            
            NpgsqlConnection conn = new NpgsqlConnection(global.connstring);
            conn.Open();
            try
            {
                // PostgeSQL-style connection string
                
                // Making connection with Npgsql provider
               
                // quite complex sql statement
                string sql = "SELECT label as \"Label\",type as \"Type\" FROM part where type !='cable' ";
                // data adapter making request from our connection
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                // i always reset DataSet before i do
                // something with it.... i don't know why :-)
                ds.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                da.Fill(ds);
                // since it C# DataSet can handle multiple tables, we will select first
                dt = ds.Tables[0];
                // connect grid to DataTable
                dataGridView1.DataSource = dt;
                // since we only showing the result we don't need connection anymore
                //conn.Close();
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                MessageBox.Show(msg.ToString());
                throw;
            }

            ///polulate combobox
            // Define a query
           // NpgsqlCommand command = new NpgsqlCommand("SELECT label FROM part",conn);

            // Execute the query and obtain a result set
            //NpgsqlDataReader dr = command.ExecuteReader();
            // Output rows
            //DataTable dt1 = new DataTable();
            //dt1.Columns.Add("label", typeof(string));
            //dt1.Load(dr);

            //comboBox1.ValueMember = "label";
           // comboBox1.DisplayMember = "label";
            //comboBox1.DataSource = dt1;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            refresh();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                pictureBox1.Image = null;
                string value1 = row.Cells[0].Value.ToString();
                string value2 = row.Cells[1].Value.ToString();
                textBox1.Text=value1+"~"+value2;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dd = comboBox1.SelectedValue.ToString();
            textBox1.Text = dd;

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("10.0.31.31").Build();
            ISession session = cluster.Connect("demo");
            RowSet rows = session.Execute("select * from camera");

            //pouplate the ddlist
            var pizzaChoices = new Dictionary<string,string>();
            pizzaChoices.Add("Small", "small");


            foreach (Row row in rows)
            {
                Console.WriteLine("{0} {1}", row["id"], row["lastuser"]);
                pizzaChoices.Add(row["label"].ToString(), row["label"].ToString());
            }

            comboBox1.DataSource = new BindingSource(pizzaChoices, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";


        }
    }




}//namespace





