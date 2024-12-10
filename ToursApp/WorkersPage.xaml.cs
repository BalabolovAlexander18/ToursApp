 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToursApp;

namespace ToursApp
{
    /// <summary>
    /// Логика взаимодействия для WorkersPage.xaml
    /// </summary>
    public partial class WorkersPage : Page
    {
        public WorkersPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var AdminUser = ToursBaseEntities2.GetContext().Users.FirstOrDefault(p => p.Type == "Administrator");
            var ManagerUser = ToursBaseEntities2.GetContext().Users.FirstOrDefault(p => p.Type == "Manager");
            if (textBoxLogin.Text == AdminUser.Login && PasBox.Password == Encryption.DecryptString(AdminUser.Password, AdminUser.KeyPassword))
            {
                Manager.MainFrame.Navigate(new HotelsPage());
            }
            else if (textBoxLogin.Text == ManagerUser.Login && PasBox.Password == Encryption.DecryptString(ManagerUser.Password, ManagerUser.KeyPassword))
            {
                Manager.MainFrame.Navigate(new HotelsPage());
            }
            else
            {
                MessageBox.Show("Не удалось войти. Проверьте логин и пароль!");
            }
        }
    }

    public static class Encryption
    {
        public static string EncryptString(string plainText, string keyString)
        {
            byte[] key = Convert.FromBase64String(keyString);
            byte[] iv = new byte[16]; // Vector initialization, usually 16 bytes for AES
            Array.Clear(iv, 0, iv.Length); // Инициализируем IV нулями (для примера; в реальности лучше генерировать случайный)

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string DecryptString(string cipherText, string keyString)
        {
            byte[] key = Convert.FromBase64String(keyString);
            byte[] iv = new byte[16];
            Array.Clear(iv, 0, iv.Length);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
