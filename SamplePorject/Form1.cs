using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace SamplePorject
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\sauds\source\repos\SamplePorject\SamplePorject\ImagesDB.mdf;Integrated Security=True");
        SqlCommand command;
        string imgloc = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Id_Click(object sender, EventArgs e)
        {

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            try {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "JPG Files (*.jpg)|*.jpg| GIF Files (*.gif)|*.gif| ALL Files (*.*)|*.*";
                dlg.Title = "Select Employees Pic";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    imgloc = dlg.FileName.ToString();
                    PicEmp.ImageLocation = imgloc;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] img = null;
                FileStream fs = new FileStream(imgloc, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                img = br.ReadBytes((int)fs.Length);
                string sql = "INSERT INTO Employees(Id,Name,Number,Pic) VALUES(" + textBoxId.Text + ",'" + textBoxFName.Text + "','" + textBoxLName.Text + "',@img)";
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("@img", img));
                int x = command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show(x.ToString() + "record(s) saved.");
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);

            }
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            try {
                string sql = "SELECT Name, Number, Pic From Employees WHERE Id=" + textBoxId.Text + "";
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                command = new SqlCommand(sql, conn);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    textBoxFName.Text = reader[0].ToString();
                    textBoxLName.Text = reader[1].ToString();
                    byte[] img = (byte[])(reader[2]);
                    if(img == null)
                    {
                        PicEmp.Image = null;
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(img);
                        PicEmp.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    MessageBox.Show("This id not exist");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try {
                byte[] img = null;
                FileStream fs = new FileStream(imgloc, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                img = br.ReadBytes((int)fs.Length);
                string sql = "UPDATE Employees SET Name = @Name, Number = @Number, Pic = @Pic WHERE  Id=" + textBoxId.Text + "";

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", textBoxId.Text);
                    command.Parameters.AddWithValue("@Name", textBoxFName.Text);
                    command.Parameters.AddWithValue("@Number", textBoxLName.Text);
                    if (img != null)
                    {
                        command.Parameters.Add(new SqlParameter("@Pic", SqlDbType.VarBinary, img.Length)).Value = img;
                    }
                    else
                    {
                       
                        command.Parameters.Add(new SqlParameter("@Pic", SqlDbType.VarBinary)).Value = DBNull.Value;
                    }

                    int x = command.ExecuteNonQuery();
                }
                conn.Close();
                MessageBox.Show("Record updated.");

            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "DELETE FROM Employees WHERE Id = @Id";

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", textBoxId.Text);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record deleted.");
                    }
                    else
                    {
                        MessageBox.Show("Record not found or deleted.");
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);
            }
        }
    }
}
