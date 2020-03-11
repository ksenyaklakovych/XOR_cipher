using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;

namespace cryptology_wpf_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentFileName;
        const string uaAlphabet = "абвгґдеєжзиіїклмнопрстуфхцчшщьюя";
        //const string uaAlphabet = "абвгґдеєжзиіїклмнопрстуфхцчшщьюяАБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
        const string enAlphabet = "abcdefghijklmnopqrstuvwxyz ";
        //const string enAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string keyVern = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        //encryption
        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            string firstText = rawTextBox.Text.ToString();
            string result = "";
            if ((bool)gammaCheckBox.IsChecked)
            {
                string key = keyFieldSlogan.Text;
                result = Encrypt(firstText, key);
            }
            else
            { 
                keyVern = GenerateKey(enAlphabet, firstText.Length);
                result = EncryptVerman(enAlphabet, firstText, keyVern);
            }

            finalTextBox.Text = result;
        }
        public string GenerateKey(string alphabet, int length)
        {
            char[] keyLetters = new char[length];
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                keyLetters[i] = alphabet[random.Next(0,length-1)];
            }
            return new string(keyLetters);
        }
        public string EncryptVerman(string alph, string text, string key)
        {
            string encrypt = "";
            for (int i = 0; i < text.Length; i++)
            {
                encrypt += (char)(text[i] ^ key[i]);
            }
            return encrypt;
        }
       
        //шифрування тексту
        public string Encrypt(string plainText, string password)
            => Cipher(plainText, password);

        //розшифрування тексту
        public string Decrypt(string encryptedText, string password)
            => Cipher(encryptedText, password);

        //метод де-/шифрування
        private string Cipher(string text, string secretKey)
        {
            var currentKey = GetRepeatKey(secretKey, text.Length);
            var res = string.Empty;
            for (var i = 0; i < text.Length; i++)
            {
                res += ((char)(text[i] ^ currentKey[i])).ToString();
            }

            return res;
        }

        //генератор повторюваної послідовності
        private string GetRepeatKey(string text, int length)
        {
            var gamma = text;
            while (gamma.Length < length)
            {
                gamma += gamma;
            }

            return gamma.Substring(0, length);
        }
        public void WriteToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(finalTextBox.Text);
            }
        }

        //decryption       
        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string firstText = rawTextBox.Text.ToString();
            string result="";
            if ((bool)gammaCheckBox.IsChecked)
            {
                string key = keyFieldSlogan.Text;
                result = Decrypt(firstText, key);
            }
            else
            {
                result = EncryptVerman(enAlphabet, firstText, keyVern);
            }
            finalTextBox.Text = result;
        }



        //other buttons
        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            currentFileName = null;
            rawTextBox.Text = String.Empty;
            finalTextBox.Text = String.Empty;
        }
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentFileName == null)
            {
                currentFileName = "../../resultFile.txt";
            }
            WriteToFile(currentFileName);
            MessageBox.Show($"{currentFileName}\nSuccessfully saved!");
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                rawTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            currentFileName = openFileDialog.FileName;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This text Caesar Cipher encryption application was created by Ksenia Klakovych.");
        }

        private void PrintFileButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.ShowDialog();
        }

        private void SwitchTextButton_Click(object sender, RoutedEventArgs e)
        {
            string first = rawTextBox.Text;
            rawTextBox.Text = finalTextBox.Text;
            finalTextBox.Text = first;
        }
    }
    class OnTimePad
    {
        Dictionary<char, int> alph = new Dictionary<char, int>();
        Dictionary<int, char> alph_ua = new Dictionary<int, char>();
        public OnTimePad(IEnumerable<char> Alphabet)
        {
            int i = 0;
            foreach (var c in Alphabet)
            {
                alph.Add(c, i);
                alph_ua.Add(i++, c);
            }
        }
        public string CryptVernama(string Text, string Key)
        {
            char[] key = Key.ToCharArray();
            char[] text = Text.ToCharArray();
            var sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                int ind;
                if (alph.TryGetValue(text[i], out ind))
                {
                    sb.Append(alph_ua[(ind ^ alph[key[i % key.Length]]) % alph.Count]);
                }
            }
            return sb.ToString();
        }
    }
}
