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

namespace SayıTahmin
{
    public partial class Form3 : Form
    {
        string kayıttarihi = DateTime.Now.ToString("dd.MM.yyyy");
        int tahminsayisi;
        int dogrusayı;
        int toplamzaman ;
        int puan = 100;
        int puan2;
        private string ad;
        Random rnd= new Random();
        public Form3(string kullaniciadi)
        {
            InitializeComponent();
            ad = kullaniciadi;
            this.AcceptButton = btnBaşlat;
            this.AcceptButton = btnTahminEt;
        }

        //rastgele sayı üretme ve rakamlarının farkli olup olmadıgına bakmak
        bool Farklirakamlar(int sayi)
        {
            string rakam = sayi.ToString();
            for (int i = 0; i < rakam.Length; i++)
            {
                for (int j = i + 1; j < rakam.Length; j++)
                {
                    if (rakam[i] == rakam[j])
                    {
                        return false;
                    }
                }


            }
            return true;

        }
        void Skorkaydet()
        {
            string baglanti = "Data Source=DESKTOP-FMV47LT\\SQLEXPRESS;Initial Catalog=sayitahmin;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection conn = new SqlConnection(baglanti))
            {
                try
                {
                    string sql = "insert into skorlar(ad,skor,süre,tarih)values(@ad2,@puan2,@toplamzaman,@kayıttarihi)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ad2", ad);
                    cmd.Parameters.AddWithValue("@puan2", puan2);
                    cmd.Parameters.AddWithValue("toplamzaman", toplamzaman);
                    cmd.Parameters.AddWithValue("@kayıttarihi", kayıttarihi);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException hata)
                {
                    MessageBox.Show("Veritabanı hatası: " + hata.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Beklenmeyen hata: " + ex.Message);
                }
            }
        }
        void enyuksekskor()
        {
            string baglanti = "Data Source=DESKTOP-FMV47LT\\SQLEXPRESS;Initial Catalog=sayitahmin;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection conn = new SqlConnection(baglanti))
            {

                string sql = "select top 5 ad,skor,süre,tarih from skorlar order by cast(skor as int)desc,cast(süre as int)asc";//en yüksek 5 skoru gösterir
                SqlDataAdapter dat = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                dat.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'sayitahminDataSet1.kullanicilar' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.kullanicilarTableAdapter.Fill(this.sayitahminDataSet1.kullanicilar);
            txtTahmin.Enabled = false;//başlat butonuna basmadan text bozsa girişe izin vermez
            lblSonuc.Text = "";//sonuc kismina yazı eklemez bos
            lblYazı.Text = "";// label gözukmesın dıye bunuda boş birakir
            lblKalanHak.Text = "";
            lblSüre.Text = "";
            listBox1.Visible = false;//gizleme yapar
            lblPuan.Text = "";
            panel2.Visible = false;
            enyuksekskor();

        }

        private void button14_Click(object sender, EventArgs e)
        {
            // başlata basilmadiysa yazma
            if (!txtTahmin.Enabled)
            {
                return;
            }

            // combobaxdan seçilen sayı kadar rakam girilebilir
            if (comboBox1.SelectedItem == null)
            {
                return;
            }
            int maxuzunluk = int.Parse(comboBox1.SelectedItem.ToString());
            if (txtTahmin.Text.Length >= maxuzunluk)
                return;
            // butonlarda yazılan sayıları yazdırma
            Button btn = sender as Button;
            if (btn != null)
            {
                txtTahmin.Text += btn.Text;
            }
        }

        private void btnBaşlat_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;//başlata basinca görünür yapar
            lblSonuc.Text = "Tahmininizi Giriniz!";//sonuc kismina yazıyı yazar
            lblKalanHak.Enabled = true;
            tahminsayisi = 10;
            lblKalanHak.Text = "kalan Hak:" + tahminsayisi;//kalan hak sayısını yazar


            txtTahmin.Enabled = true;//tahmin girdigimiz kismi açar ki sayıyı girebilelim 
            txtTahmin.Focus();


            txtTahmin.Clear();//başlat yapinca siler
            listBox1.Items.Clear();// başlat yapinca temizler
            lblYazı.Text = "";// yazi yazdigimiz kismi temizler

            //combobaxdan hane sayısı almak icin
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("lütfen hane sayısı giriniz");
                return;

            }
            int secilenhane = Convert.ToInt32(comboBox1.SelectedItem);

            int min = 0; int max = 0;
            if (comboBox1.SelectedIndex == 0)//3 haneli oyun için
            {
                min = 100;
                max = 999;
            }
            if (comboBox1.SelectedIndex == 1)//4 haneli oyun için
            {
                min = 1000;
                max = 9999;
            }
            if (comboBox1.SelectedIndex == 2)//5 haneli oyun için
            {
                min = 10000;
                max = 99999;
            }
            //rakamlar farli olana kadar üretmek
            do
            {
                dogrusayı = rnd.Next(min, max);
            }
            while (!Farklirakamlar(dogrusayı));
            toplamzaman = 0;
            lblSüre.Text = "00.00";
            timer1.Start();
            puan = 100;
            lblPuan.Text = "puan:" + puan;
        }

        private void btnTahminEt_Click(object sender, EventArgs e)
        {
            lblPuan.Text = "puan" + puan;

            string dogrusayi2 = dogrusayı.ToString();
            int secilenhane = Convert.ToInt32(comboBox1.SelectedItem);
            if (txtTahmin.Text.Length != secilenhane)
            {
                MessageBox.Show($"{secilenhane}haneli sayi giriniz");
                return;
            }

            string tahmin = txtTahmin.Text;// tahminin textinden tahmin sayısını tahmine atar
            txtTahmin.Clear();// ve tahmin textini temizler
            listBox1.Visible = true;

            panel2.Visible = true;
            if (tahmin.StartsWith("0"))// sayını başına sıfır girilip girilmedigine bakar
            {
                lblSonuc.Text = "sayı sıfır ile başlayamaz";
                lblYazı.Text = "ilk rakam 1-9 arasında olmalı";
                return;
            }
            if (tahmin.Length != tahmin.Distinct().Count())//rakamların farkli olup olmadigina bakar
            {
                lblSonuc.Text = "rakamlar birbirinden farkli olmali";
                lblYazı.Text = "örnek=123,789 gibi (tekrar eden rakam yok)";
                return;
            }


            if (tahminsayisi > 0)//hak düşürme
            {

                tahminsayisi--;
                lblKalanHak.Text = "kalan hak:" + tahminsayisi.ToString();
            }
            else
            {
                lblSonuc.Text = "oyun bitti! dogru sayı:" + dogrusayı;
                lblYazı.Text = "yeniden başlamak için başlat butonuna tıklayın";
                timer1.Stop();
                Skorkaydet();
                enyuksekskor();
                lblSonuc.ForeColor = Color.Red;
                lblYazı.ForeColor = Color.Red;
                return;
            }
            if (tahmin == dogrusayi2)//tahminin dogru sayıya eşit mi diye bakariz
            {
                lblSonuc.Text = "dogru sayıyı buldunuz..:)";
                timer1.Stop();
                Skorkaydet();
                enyuksekskor();
                lblSonuc.ForeColor = Color.Green;
                



            }
            //yazdırma kısmı+
            int dogruyer = 0;
            int yanlisyer = 0;

            bool[] dogrukullanildi = new bool[3];
            bool[] tahminkullanildi = new bool[3];

            for (int i = 0; i < 3; i++)// dogru rakam dogru yerde için yazılır
            {
                if (tahmin[i] == dogrusayi2[i])//tahmindeki ikinci basamakla dogru sayının ikinci basamagi
                {
                    dogruyer++;//dogru yer sayısı artirilir
                    dogrukullanildi[i] = true;
                    tahminkullanildi[i] = true;
                }
            }
            for (int i = 0; i < 3; i++)//dogru rakam yanliş yerde icin yazılır
            {
                if (tahminkullanildi[i]) continue;// bu basamak zaten kullanıldıysa atla
                for (int j = 0; j < 3; j++)
                {
                    if (dogrukullanildi[j]) continue;
                    if (tahmin[i] == dogrusayi2[j])// bu basamak dogru sayının bı yerınde varsa 
                    {
                        yanlisyer++;// yanliş yeri artır
                        dogrukullanildi[j] = true;// bu rakam bir daha kullanılmasın diye işaretlenir
                        break;
                    }
                }

            }

            if (dogruyer == 0 && yanlisyer == 0)
            {
                lblSonuc.Visible = true;
                lblYazı.Text = "hiç bir rakam dogru degil";
                puan -= 10;
                if (puan < 0)

                    puan = 0;
                lblPuan.Text = "puan:" + puan;
                puan2 = puan;
                lblSonuc.ForeColor = Color.Red;
                lblYazı.ForeColor = Color.Red;

            }
            if (dogruyer > 0 && yanlisyer > 0)
            {
                lblSonuc.Visible = true;
                lblYazı.Text = dogruyer + "rakamin yeri dogru," + yanlisyer + "rakam dogru ancak yeri yanlış";
                puan -= 10;
                if (puan < 0)

                    puan = 0;
                lblPuan.Text = "puan:" + puan;
                puan2 = puan;
                lblSonuc.ForeColor = Color.Blue;
                lblYazı.ForeColor = Color.Blue;
            }
            else if (dogruyer > 0)
            {
                lblSonuc.Visible = true;
                lblYazı.Text = dogruyer + "rakamin yeri dogru";
                puan -= 10;
                if (puan < 0)

                    puan = 0;
                lblPuan.Text = "puan:" + puan;
                puan2 = puan;
                lblSonuc.ForeColor = Color.Green;
                lblYazı.ForeColor = Color.Green;
            }
            else if (yanlisyer > 0)
            {
                lblSonuc.Visible = true;
                lblYazı.Text = yanlisyer + "rakam dogru ama yeri yanlış";
                puan -= 10;
                if (puan < 0)

                    puan = 0;
                lblPuan.Text = "puan:" + puan;
                puan2 = puan;
                lblSonuc.ForeColor = Color.FromArgb(255, 193, 7);
                lblYazı.ForeColor = Color.FromArgb(255, 193, 7);
            }



            string sonuc = $"+{dogruyer}| -{yanlisyer}";
            //lblYazi.Text = $"+{dogruyer} rakam dogru yerde,{yanlisyer} rakam dogru ama yeri yanlış";
            lblSonuc.Text = $"sonuç:{sonuc}";
            listBox1.Items.Add($"tahmin{tahmin}->{sonuc}");
        }

        private void txtTahmin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // sadece rakam girilebilmesi için 
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //zamanı dakika ve saniye şeklinde oluşturmak için
            toplamzaman++;
            int dakika = toplamzaman / 60;
            int saniye = toplamzaman % 60;

            lblSüre.Text = dakika.ToString("00") + ":" + saniye.ToString("00");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //girilen sayıları silmek için
            if (txtTahmin.Text.Length > 0)
                txtTahmin.Text = txtTahmin.Text.Substring(0, txtTahmin.Text.Length - 1);
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
