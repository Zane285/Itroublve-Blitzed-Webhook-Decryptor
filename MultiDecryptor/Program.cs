using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace MultiDecryptor
{
    internal class Program
    {
		public static class ExtensionMethods
		{
			public static string Unniggerify(string cipherText, string passPhrase)
			{
				byte[] array = Convert.FromBase64String(cipherText);
				byte[] salt = array.Take(32).ToArray<byte>();
				byte[] rgbIV = array.Skip(32).Take(32).ToArray<byte>();
				byte[] array2 = array.Skip(64).Take(array.Length - 64).ToArray<byte>();
				string @string;
				using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, salt, 1000))
				{
					byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
					using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
					{
						rijndaelManaged.BlockSize = 256;
						rijndaelManaged.Mode = CipherMode.CBC;
						rijndaelManaged.Padding = PaddingMode.PKCS7;
						using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(bytes, rgbIV))
						{
							using (MemoryStream memoryStream = new MemoryStream(array2))
							{
								using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
								{
									byte[] array3 = new byte[array2.Length];
									int count = cryptoStream.Read(array3, 0, array3.Length);
									memoryStream.Close();
									cryptoStream.Close();
									@string = Encoding.UTF8.GetString(array3, 0, count);
								}
							}
						}
					}
				}
				return @string;
			}
			public static byte[] AES128(byte[] message)
			{
				byte[] result;
				try
				{
					result = new AesManaged
					{
						Key = new byte[]
						{
						88,
						105,
						179,
						95,
						179,
						135,
						116,
						246,
						101,
						235,
						150,
						231,
						111,
						77,
						22,
						131
						},
						IV = new byte[16],
						Mode = CipherMode.CBC,
						Padding = PaddingMode.Zeros
					}.CreateDecryptor().TransformFinalBlock(message, 0, message.Length);
				}
				catch
				{
					result = null;
				}
				return result;
			}
		}
		static void Main(string[] args)
        {
			Console.Title = "Made by Dextority";
			Console.Clear();

			Console.WriteLine("(1). Itroublve Webhook Decryptor\n(2). Drag And Drop Itroublve (Only Unobfuscated Files)\n(3). Blitzed Webhook Decryptor\n(4). Drag and drop Blitzed");
			string option = Console.ReadLine();
			if (option == "1")
			{
				Console.Clear();
				Console.Write("Enter webhook: ");
				string encryptedWebhook = Console.ReadLine();
				byte[] bytes = ExtensionMethods.AES128(Convert.FromBase64String(encryptedWebhook));
				encryptedWebhook = new Uri(Encoding.ASCII.GetString(bytes)).AbsoluteUri;
				Console.Clear();
				Console.WriteLine(encryptedWebhook.Replace("%00", ""));
				Console.ReadLine();
			}
			else if (option == "2")
			{
				Console.Clear();
				Console.Write("Enter file path: ");
				string path = Console.ReadLine();

				ModuleDefMD Module = ModuleDefMD.Load(path);
				Assembly asm = Assembly.LoadFile(path);
				foreach (TypeDef type in Module.Types.Where(t => t.HasMethods))
				{
					foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
					{
						for (int i = 0; i < method.Body.Instructions.Count(); i++)
						{
							try
							{
								if (method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i].Operand.ToString().Contains("Main()"))
								{
									string encryptedWebhook = method.Body.Instructions[i - 78].Operand.ToString();
									byte[] bytes = ExtensionMethods.AES128(Convert.FromBase64String(encryptedWebhook));
									encryptedWebhook = new Uri(Encoding.ASCII.GetString(bytes)).AbsoluteUri;
									Console.Clear();
									Console.WriteLine(encryptedWebhook.Replace("%00", ""));
									Console.ReadLine();
								}
							}
							catch (Exception ex)
							{

							}
						}
					}
				}
			}
			else if (option == "3")
			{
				Console.Clear();
				Console.Write("Enter Encrypted Webhook: ");
				string encrypted_webhook = Console.ReadLine();
				Console.Write("Enter Key: ");
				string key = Console.ReadLine();
				string webhook = ExtensionMethods.Unniggerify(encrypted_webhook, key);
				Console.Clear();
				Console.WriteLine(webhook);
			}
			else if (option == "4")
			{
				Console.Clear();
				Console.Write("Enter file path: ");
				string path = Console.ReadLine();
				Console.Clear();

				ModuleDefMD Module = ModuleDefMD.Load(path);
				Assembly asm = Assembly.LoadFile(path);
				foreach (TypeDef type in Module.Types.Where(t => t.HasMethods))
				{
					foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
					{
						for (int i = 0; i < method.Body.Instructions.Count(); i++)
						{
							try
							{
								//Console.WriteLine(method.Body.Instructions[i].Operand.ToString());

								if (method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i].Operand.ToString().Contains("Unniggerify(System.String,System.String)"))
								{
									string encryptedWebhook = method.Body.Instructions[i - 2].Operand.ToString();
									string passPhrase = method.Body.Instructions[i - 1].Operand.ToString();
									string decryptedWebhook = ExtensionMethods.Unniggerify(encryptedWebhook, passPhrase);
									Console.WriteLine(decryptedWebhook);
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex);
							}
						}
					}
				}
				Console.ReadLine();
			}
			else
			{
				Console.WriteLine("Only 4 options, SO PUT ONE OF THEM");
				Console.ReadLine();
			}
		}
	}
    
}
