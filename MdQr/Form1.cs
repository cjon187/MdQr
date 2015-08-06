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

namespace MdQr
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ///////////




        /// ////////////


        public static string Transform(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 17;
                    }
                    else
                    {
                        number += 17;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 17;
                    }
                    else
                    {
                        number += 17;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }

        public static string Transform2(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 18;
                    }
                    else
                    {
                        number += 18;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 18;
                    }
                    else
                    {
                        number += 18;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }




        ///
        private void button1_Click(object sender, EventArgs e)
        {
            String camName = textBox1.Text.ToString();
            String ipAddress = textBox2.Text.ToString();
            String macAddress = textBox3.Text.ToString();

            //validation for ip and mac
            IPAddress address;
            if (IPAddress.TryParse(ipAddress, out address))
            {
                String m = macAddress;
                m = macAddress.Replace(" ", "").Replace(":", "").Replace("-", "");
                Regex r = new Regex("^([0-9a-fA-F]{2}(?:[:-]?[0-9a-fA-F]{2}){5})$");
                if (r.IsMatch(m))
                {
                    if (camName.Equals("") || camName.Equals(null))
                    {
                        MessageBox.Show("Need a Camera Name");
                    }
                    else
                    {
                        string rot = "Name: " + camName + "\r\n" + "IP: " + ipAddress + "\r\n" + "MAC: " + macAddress;
                        rot = Encrypt(rot);
                        Console.WriteLine(rot);
                        QRCode qrcode = new QRCode();
                        qrcode.Data = rot;
                        qrcode.X = 4;
                        qrcode.drawBarcode("c:\\qr_code.png");
                        qr1.Data = rot;
                        qr1.Refresh();

                        rot = Decrypt(rot);
                        Console.WriteLine(rot);

                        /*add text to image
                        System.Drawing.Image img = System.Drawing.Image.FromFile(@"c:\qr_code.png");
                        MessageBox.Show("Width: " + img.Width + ", Height: " + img.Height);
                        img.Dispose();

                        FileStream fs = new FileStream(@"c:\qr_code.png", FileMode.Open, FileAccess.Read);
                        Image image = Image.FromStream(fs);
                        fs.Close();

                        Bitmap b = new Bitmap(image);
                        Graphics graphics = Graphics.FromImage(b);
                        graphics.DrawString("Hello", this.Font, Brushes.Black, 80, 130);

                        b.Save(@"c:\qr_code.png", image.RawFormat);

                        image.Dispose();
                        b.Dispose();*/

                        string inputImage = @"C:\qr_code.png";
                        string outputImageFilePath = @"C:\qr_code1.png";
                        string textToDisplayOnImage = "www.aspnettutorialonline.blogspot.com/";

                        Bitmap imageInBitMap = new Bitmap(inputImage);
                        Graphics imageGraphics = Graphics.FromImage(imageInBitMap);

                        //Set the alignment based on the coordinates 
                        StringFormat formatAssignment = new StringFormat();
                        formatAssignment.Alignment = StringAlignment.Near;

                        //Here we are going to assign the font color
                        Color assignColorToString = System.Drawing.ColorTranslator.FromHtml("#000000");

                        //Assigning font size, font family, position of the text to display and others.
                        imageGraphics.DrawString(textToDisplayOnImage, new Font("Times new Roman", 8, FontStyle.Bold), new SolidBrush(assignColorToString), new Point(80, 80), formatAssignment);

                        /*saving in the computer with label
                        imageInBitMap.Save(outputImageFilePath);
                        String[] a = { inputImage, inputImage };
                        Bitmap com = CombineBitmap(a);
                        com.Save(@"c:\com.png");
                        com.Dispose();*/

                    }

                }
                else
                {
                    MessageBox.Show("mac is wrong");
                }

            }
            else
            {
                MessageBox.Show("IP WRONG");
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
            /*string value = "guru";
            Console.WriteLine(value);

            value = Transform(value);
            Console.WriteLine("en1 " + value);
            value = Transform2(value);
            Console.WriteLine("en2 " + value);


            value = Transform2(value);
            Console.WriteLine("en4 " + value);
            value = Transform(value);
            Console.WriteLine("en5 " + value);*/

            string encrypted = Encrypt("guru");
            Console.Out.WriteLine(encrypted);

            string decrypted = Decrypt(encrypted);
            Console.Out.WriteLine(decrypted);

            
                       


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
       
    }



}//namespace





