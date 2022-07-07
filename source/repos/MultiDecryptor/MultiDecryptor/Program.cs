using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

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

			Console.Write("(1). Itroublve Webhook Decryptor\n(2). Drag And Drop Itroublve (Only Unobfuscated Files)\n(3). Drag and Drop Itroublve Obfuscated File\n(4). Blitzed Webhook Decryptor\n(5). Drag and drop Blitzed\n>>: ");
			string option = Console.ReadLine();
			if (option == "1")
			{
				Console.Clear();
				Console.Write("Enter webhook: ");
				string encryptedWebhook = Console.ReadLine();
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine($"[+] Decrypting Webhook");
				byte[] bytes = ExtensionMethods.AES128(Convert.FromBase64String(encryptedWebhook));
				encryptedWebhook = new Uri(Encoding.ASCII.GetString(bytes)).AbsoluteUri;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"\nWebhook: {encryptedWebhook.Replace("%00", "")}");
				Console.ReadLine();
			}
			else if (option == "2")
			{
				Console.Clear();
				Console.Write("Enter file path: ");
				string path = Console.ReadLine().Replace("\"", null);
				Console.Clear();

				ModuleDefMD Module = ModuleDefMD.Load(path);
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
									Console.ForegroundColor = ConsoleColor.Blue;
									Console.WriteLine($"[+] Decrypting Webhook");
									string encryptedWebhook = method.Body.Instructions[i = 78].Operand.ToString();
									Console.WriteLine(encryptedWebhook);
                                    byte[] bytes = ExtensionMethods.AES128(Convert.FromBase64String(encryptedWebhook));
                                    encryptedWebhook = new Uri(Encoding.ASCII.GetString(bytes)).AbsoluteUri;
									Console.ForegroundColor = ConsoleColor.Red;
									Console.WriteLine($"\nWebhook: {encryptedWebhook.Replace(" %00", "")}");
                                    Console.ReadLine();
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
			else if (option == "3")
            {
				Console.Clear();
				Console.Write("Enter file path: ");

				string path = Console.ReadLine();
				string confexe = Directory.GetCurrentDirectory() + "\\Conf\\ConfuserEx-Unpacker.exe";
				string confpath = Directory.GetCurrentDirectory() + "\\Conf";
				string filename = Path.GetFileName(path);

				Process process = new Process();
				ProcessStartInfo startInfo = new ProcessStartInfo();
				startInfo.WindowStyle = ProcessWindowStyle.Hidden;
				startInfo.FileName = "cmd.exe";
				startInfo.Arguments = $"/C {confexe} {path}";
				process.StartInfo = startInfo;
				process.Start();
				Console.ForegroundColor = ConsoleColor.Blue;

				string filenameNoExtension = filename.Replace(".exe", null);
				Console.Clear();
				Console.WriteLine($"[+] Unconfusing {filenameNoExtension}");
				Thread.Sleep(5000);
				string path_noextension = path.Replace(".exe", null);

				ModuleDefMD Module = ModuleDefMD.Load($"{path_noextension}-Cleaned.exe");
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
									Console.WriteLine($"[+] Decrypting Webhook");
									string encryptedWebhook = method.Body.Instructions[91].Operand.ToString();
                                    byte[] bytes = ExtensionMethods.AES128(Convert.FromBase64String(encryptedWebhook));
                                    encryptedWebhook = new Uri(Encoding.ASCII.GetString(bytes)).AbsoluteUri;
									Console.ForegroundColor = ConsoleColor.Red;
									Console.WriteLine($"\nWebhook: {encryptedWebhook.Replace("%00", "")}");
                                    Console.ReadLine();
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
			else if (option == "4")
			{
				Console.Clear();
				Console.Write("Enter Encrypted Webhook: ");
				Console.Clear();
				string encrypted_webhook = Console.ReadLine();
				Console.Write("Enter Key: ");
				string key = Console.ReadLine();
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine($"[+] Decrypting Webhook");
				string webhook = ExtensionMethods.Unniggerify(encrypted_webhook, key);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Webhook: {webhook}");
			}
			else if (option == "5")
			{
				Console.Clear();
				Console.Write("Enter file path: ");
				string path = Console.ReadLine().Replace("\"", null); ;
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
									Console.ForegroundColor = ConsoleColor.Blue;
									Console.WriteLine($"[+] Decrypting Webhook");
									string encryptedWebhook = method.Body.Instructions[i - 2].Operand.ToString();
									string passPhrase = method.Body.Instructions[i - 1].Operand.ToString();
									string decryptedWebhook = ExtensionMethods.Unniggerify(encryptedWebhook, passPhrase);
									Console.ForegroundColor = ConsoleColor.Red;
									Console.WriteLine($"Webhook: {decryptedWebhook}");
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
				Console.WriteLine("Only 5 options, SO PUT ONE OF THEM");
				Console.ReadLine();
			}
		}
	}
    
}
