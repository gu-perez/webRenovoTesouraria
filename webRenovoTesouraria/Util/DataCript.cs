using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace webRenovoTesouraria.Util
{

    public class DataCript
    {
        private static Rijndael CriarInstanciaRijndael()
        {
            Rijndael algoritmo = Rijndael.Create();
            algoritmo.Key = Encoding.ASCII.GetBytes("6W978E4F54G465SWDS897GSOB859F5G8");
            algoritmo.IV = Encoding.ASCII.GetBytes(Startup.StaticConfig.GetSection("CriptoVector").Value);

            return algoritmo;
        }

        public static string Encriptar(string textoNormal)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(textoNormal)) throw new Exception("O conteúdo a ser encriptado não pode ser uma string vazia.");

                using (Rijndael algoritmo = CriarInstanciaRijndael())
                {
                    ICryptoTransform encryptor = algoritmo.CreateEncryptor(algoritmo.Key, algoritmo.IV);

                    using (MemoryStream streamResultado = new MemoryStream())
                    {
                        using (CryptoStream csStream = new CryptoStream(streamResultado, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(csStream))
                            {
                                writer.Write(textoNormal);
                            }
                        }

                        return ArrayBytesToHexString(streamResultado.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static string ArrayBytesToHexString(byte[] conteudo)
        {
            string[] arrayHex = Array.ConvertAll(conteudo, b => b.ToString("X2"));
            return string.Concat(arrayHex);
        }

        public static string Decriptar(string textoEncriptado)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(textoEncriptado)) throw new Exception("O conteúdo a ser decriptado não pode ser uma string vazia.");
                if (textoEncriptado.Length % 2 != 0) throw new Exception("O conteúdo a ser decriptado é inválido.");

                using (Rijndael algoritmo = CriarInstanciaRijndael())
                {
                    ICryptoTransform decryptor = algoritmo.CreateDecryptor(algoritmo.Key, algoritmo.IV);

                    string textoDecriptografado = null;
                    using (MemoryStream streamTextoEncriptado = new MemoryStream(HexStringToArrayBytes(textoEncriptado)))
                    {
                        using (CryptoStream csStream = new CryptoStream(streamTextoEncriptado, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(csStream))
                            {
                                textoDecriptografado = reader.ReadToEnd();
                            }
                        }
                    }

                    return textoDecriptografado;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static byte[] HexStringToArrayBytes(string conteudo)
        {
            int qtdeBytesEncriptados = conteudo.Length / 2;
            byte[] arrayConteudoEncriptado = new byte[qtdeBytesEncriptados];
            for (int i = 0; i < qtdeBytesEncriptados; i++)
            {
                arrayConteudoEncriptado[i] = Convert.ToByte(conteudo.Substring(i * 2, 2), 16);
            }

            return arrayConteudoEncriptado;
        }

    }
}
