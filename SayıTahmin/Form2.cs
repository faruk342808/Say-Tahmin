using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace SayıTahmin
{
    public partial class Form2 : Form
    {
        public Form2()
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
        string baglanti = "Data Source=DESKTOP-FMV47LT\\SQLEXPRESS;Initial Catalog=Sayitahmin;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            string ad = txtAd.Text.Trim();
            string soyad = txtSoyad.Text.Trim();
            string şifre = Sifreleme(txtŞifre.Text);
            string tel = txtTelefonNo.Text.Trim();
            string eposta = txtEposta.Text.Trim();
            string kayıttarihi = DateTime.Now.ToString("dd.MM.yyyy");
            //bilgilerin boş olup olmadıgı konrtrolu
            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad) || string.IsNullOrWhiteSpace(şifre) || string.IsNullOrWhiteSpace(tel) || string.IsNullOrWhiteSpace(eposta) || string.IsNullOrWhiteSpace(kayıttarihi))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz");
                return;
            }
            // eposta kontrolu
            if (!eposta.Contains("@") || !eposta.Contains("."))
            {
                MessageBox.Show("Lütfen geçerli eposta giriniz");

                return;
            }
            //telefon da sadece sayı olmalı

            if (!tel.All(char.IsDigit))
            {
                MessageBox.Show("Numara sadece sayı içermelı");
                return;
            }
            // telefon uzunlugu 10 ve ya 11 olmalı
            if (tel.Length < 10 || tel.Length > 11)
            {
                MessageBox.Show("Numara 10 veya 11 haneli olmalı");
                return;
            }
            SqlConnection conn = new SqlConnection(baglanti);

            try
            {
                conn.Open();

                //  E-posta var mı kontrol et
                string kontrolSql = "SELECT COUNT(*) FROM kullanicilar WHERE eposta = @eposta";
                SqlCommand kontrolCmd = new SqlCommand(kontrolSql, conn);
                kontrolCmd.Parameters.AddWithValue("@eposta", eposta);

                int sayi = (int)kontrolCmd.ExecuteScalar();

                if (sayi > 0)
                {
                    MessageBox.Show("Bu e-posta zaten kayıtlı!");
                    return; //  kayıt işlemi DURUR
                }

                // Yoksa kayıt ekle
                string ekleSql = @"INSERT INTO kullanicilar
                   (ad, soyad, sifre, telefon, eposta, kayitTarihi)
                  VALUES
                   (@ad, @soyad, @sifre, @telefon, @eposta, @kayittarihi)";

                SqlCommand ekleCmd = new SqlCommand(ekleSql, conn);
                ekleCmd.Parameters.AddWithValue("@ad", ad);
                ekleCmd.Parameters.AddWithValue("@soyad", soyad);
                ekleCmd.Parameters.AddWithValue("@sifre", şifre);
                ekleCmd.Parameters.AddWithValue("@telefon", tel);
                ekleCmd.Parameters.AddWithValue("@eposta", eposta);
                ekleCmd.Parameters.AddWithValue("@kayittarihi", kayıttarihi);

                ekleCmd.ExecuteNonQuery();

                MessageBox.Show("Kayıt başarıyla eklendi");
                this.Hide();
                Form1 fr = new Form1();
                fr.Show();
            }
            catch (SqlException hata)//sqlde hata verip vermediğini kontrol etmek için
            {
                MessageBox.Show("Veritabanı hatası: " + hata.Message);
            }
            catch (Exception ex)//burada hata verip vermediğini kontrol eder
            {
                MessageBox.Show("Beklenmeyen hata: " + ex.Message);
            }
            finally //bağlantıyı kapatıyoruz
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void geri_Click(object sender, EventArgs e)
        {
            Form1 fr = new Form1();
            fr.Show();
            this.Hide();
        }
    }
}
