using System;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;

namespace io.newgrounds
{
	/// <summary>
	/// This class handles all cryptography used by Newgrounds.io.
	/// </summary>
	public class ngCrypto
	{

		public const uint CIPHER_AES128 = 1;
		public const uint CIPHER_RC4 = 2;

		public const uint ENCODE_BASE64 = 1;
		public const uint ENCODE_HEX = 2;

		protected byte[] key;
		protected uint cipher;
		protected uint encoder;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key">The encoded encryption key you want to use.</param>
		/// <param name="cipher">The encryption cypher being used.</param>
		/// <param name="encoder">The encoding used for both the key and any encrypted output.</param>
		public ngCrypto(string key, uint cipher = CIPHER_AES128, uint encoder = ENCODE_BASE64)
		{
			this.key = encoder == ENCODE_BASE64 ? Convert.FromBase64String(key) : hex2bin(key); // convert key string to binary
			this.cipher = cipher;
			this.encoder = encoder;
		}

		/// <summary>
		/// Encrypts and encodes a string value.
		/// </summary>
		/// <param name="data">The string you wish to encrypt.</param>
		/// <returns>The encrypted and encoded string.</returns>
		public string encrypt(string data)
		{
			string encrypted_text = "";
			byte[] encrypted_bytes = new byte[0];

			switch (this.cipher)
			{
				case CIPHER_AES128:
					RijndaelManaged myRijndael = new RijndaelManaged();

					// setting the key here lets Rijndael know how many bits to use for the IV
					myRijndael.Key = this.key;
					// create a random IV
					myRijndael.GenerateIV();

					// encrypt our data string
					byte[] aes = encryptAES128(data, myRijndael.Key, myRijndael.IV);

					// concatenate the IV and the encrypted bytes so PHP can decrypt them later
					encrypted_bytes = new byte[myRijndael.IV.Length + aes.Length];
					Buffer.BlockCopy(myRijndael.IV, 0, encrypted_bytes, 0, myRijndael.IV.Length);
					Buffer.BlockCopy(aes, 0, encrypted_bytes, myRijndael.IV.Length, aes.Length);
					break;

				case CIPHER_RC4:
					encrypted_bytes = encryptRC4(data, this.key);
					break;
			}

			// encode our concatenated byte array to the apropriate string format
			switch (this.encoder)
			{
				case ENCODE_HEX:
					encrypted_text = bin2hex(encrypted_bytes);
					break;
				case ENCODE_BASE64:
					encrypted_text = Convert.ToBase64String(encrypted_bytes);
					break;
			}

			// return the string
			return encrypted_text;
		}

		internal static byte[] encryptRC4(string plainText, byte[] key)
		{
			int a, i, j, k, tmp;
			int[] _key, box;
			byte[] ciphertext;

			byte[] data_bytes = Encoding.UTF8.GetBytes(plainText);

			_key = new int[256];
			box = new int[256];
			ciphertext = new byte[data_bytes.Length];

			for (i = 0; i < 256; i++)
			{
				_key[i] = key[i % key.Length];
				box[i] = i;
			}
			for (j = i = 0; i < 256; i++)
			{
				j = (j + box[i] + _key[i]) % 256;
				tmp = box[i];
				box[i] = box[j];
				box[j] = tmp;
			}
			for (a = j = i = 0; i < data_bytes.Length; i++)
			{
				a++;
				a %= 256;
				j += box[a];
				j %= 256;
				tmp = box[a];
				box[a] = box[j];
				box[j] = tmp;
				k = box[((box[a] + box[j]) % 256)];
				ciphertext[i] = (byte)(data_bytes[i] ^ k);
			}
			return ciphertext;
		}

		// this handles the actual encryption
		internal static byte[] encryptAES128(string plainText, byte[] Key, byte[] IV)
		{
			// Check arguments. 
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");
			byte[] encrypted;
			// Create an RijndaelManaged object 
			// with the specified key and IV. 
			using (RijndaelManaged rijAlg = new RijndaelManaged())
			{
				rijAlg.Key = Key;
				rijAlg.IV = IV;

				// Create a decryptor to perform the stream transform.
				ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

				// Create the streams used for encryption. 
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{

							//Write all data to the stream.
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}


			// Return the encrypted bytes from the memory stream. 
			return encrypted;

		}

		// converts a hex string to a byte array
		internal static byte[] hex2bin(string hex)
		{
			return Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
							.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
							.ToArray();
		}

		// converts a byte array to a hex string
		internal static string bin2hex(byte[] bin)
		{
			return BitConverter.ToString(bin).Replace("-", "");
		}
	}
}