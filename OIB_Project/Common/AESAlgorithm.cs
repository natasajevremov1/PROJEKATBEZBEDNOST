using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class AESAlgorithm
    {
        //  private readonly string key = "thisisaverysecretkey12345"; // 32 byte key for AES-256
        private readonly string iv = "thisisaverysecre"; // 16 byte IV

        public string Encrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateValidKey(key);
                aesAlg.Mode = CipherMode.CBC;

                // Generiši slučajni IV
                aesAlg.GenerateIV();
                byte[] iv = aesAlg.IV;  // Dobijanje generisanog IV-a
                string ivHex = BitConverter.ToString(iv).Replace("-", " ");
                Console.WriteLine("Generisani IV (hex): " + ivHex);

                // Opcionalno: Prikaz IV-a kao decimalnih vrednosti
                Console.WriteLine("Generisani IV (decimal): " + string.Join(", ", iv));

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

                    // Vraćamo enkriptovane podatke zajedno sa IV-om
                    byte[] encrypted = msEncrypt.ToArray();
                    byte[] ivWithEncryptedData = new byte[iv.Length + encrypted.Length];

                    // Kopiraj IV na početak
                    Array.Copy(iv, 0, ivWithEncryptedData, 0, iv.Length);
                    // Kopiraj enkriptovane podatke nakon IV-a
                    Array.Copy(encrypted, 0, ivWithEncryptedData, iv.Length, encrypted.Length);

                    return Convert.ToBase64String(ivWithEncryptedData); // Vraćamo u Base64 formatu
                }
            }
        }

        public string Decrypt(string cipherText, string key)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // Prvo odvajamo IV (prvih 16 bajtova)
            byte[] iv = new byte[16];
            Array.Copy(cipherBytes, 0, iv, 0, iv.Length);

            // Ostatak su enkriptovani podaci
            byte[] encryptedData = new byte[cipherBytes.Length - iv.Length];
            Array.Copy(cipherBytes, iv.Length, encryptedData, 0, encryptedData.Length);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateValidKey(key);
                aesAlg.IV = iv; // Koristi IV koji je poslat sa podacima
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
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



        // Funkcija za proveru i generisanje validnog IV-a
        private byte[] GetValidIV(string iv)
        {
            // Ako je IV kraći od 16 bajtova, proširućemo ga do tačno 16 bajtova
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            if (ivBytes.Length < 16)
            {
                Array.Resize(ref ivBytes, 16);
            }
            else if (ivBytes.Length > 16)
            {
                Array.Resize(ref ivBytes, 16);
            }

            return ivBytes;
        }

        // Funkcija koja generiše ključ tačne veličine (256-bitni ključ = 32 bajta)
        private byte[] GenerateValidKey(string key)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }
    }
}