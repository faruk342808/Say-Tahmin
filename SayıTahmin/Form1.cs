using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;//Sql bağlantısı için
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SayıTahmin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string Sifreleme(string parola)
        {
            using (SHA256 sha256 = SHA256.Create())//şifreyi kırılmaz bir karışıma çeviren sistem
            {
                byte[] bytes = Encoding.UTF8.GetBytes(parola);//kullanıcının girdigi şifreyi kayitlara dönüştürür
                byte[] hash = sha256.ComputeHash(bytes);// şifrenin byte halini alıp sha256 ile karşilaştirir
                StringBuilder sb = new StringBuilder();// bu kayıtları okunabilir hale getiren araç
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));


                }
                return sb.ToString();
            }
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = txtKullaniciAdi.Text;
            string parola = Sifreleme(txtSifre.Text);
            string conn = "Data Source=DESKTOP-FMV47LT\\SQLEXPRESS;Initial Catalog=SayiTahmin;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(conn))
            {
                string sql = "select count(*) from kullanicilar where ad=@kullanıcı and sifre=@sifre";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@kullanıcı", kullaniciAdi);
                cmd.Parameters.AddWithValue("@sifre", parola);
                connection.Open();
                int sonuc = (int)cmd.ExecuteScalar();
                if (sonuc > 0)
                {
                    MessageBox.Show("GİRİŞ BAŞARILI");
                    Form3 frm = new Form3(kullaniciAdi);


                    frm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Eposta veya Şifre hatali!!");
                }
            }
        }

        private void btnKayit_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 frm = new Form2();
            frm.Show();
        }
    }
}
