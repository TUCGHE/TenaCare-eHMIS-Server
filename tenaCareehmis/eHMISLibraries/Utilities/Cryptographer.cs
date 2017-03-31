/*
 * 
 * Copyright © 2006-2017 TenaCareeHMIS  software, by The Administrators of the Tulane Educational Fund, 
 * dba Tulane University, Center for Global Health Equity is distributed under the GNU General Public License(GPL).
 * All rights reserved.

 * This file is part of TenaCareeHMIS
 * TenaCareeHMIS is free software: 
 * 
 * you can redistribute it and/or modify it under the terms of the 
 * GNU General Public License as published by the Free Software Foundation, 
 * version 3 of the License, or any later version.
 * TenaCareeHMIS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
 * FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License along with TenaCareeHMIS.  
 * If not, see http://www.gnu.org/licenses/.    
 * 
 * 
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;

namespace General.Util
{

	/// <summary>
	/// Utility class which provides encryption and decryption facilities.
	/// </summary>
	public class Cryptographer
	{
		/// <summary>
		/// The cryptographic method which will be used for encryption and decryption.
		/// </summary>
		private SymmetricAlgorithm cryptoMethod = null;

		/// <summary>
		/// The public key needed by the cryptographic method.
		/// </summary>
		private byte[] cryptoKey = null;

		/// <summary>
		/// The initialization vector needed by the cryptographic method.
		/// </summary>
		private byte[] cryptoIV = null;

		/// <summary>
		/// The number of bytes per chunk of encoding.
		/// </summary>
		private int chunkSize = 100;

		public SymmetricAlgorithm Method
		{
			get
			{
				return cryptoMethod;
			}
			set
			{
				cryptoMethod = value;
			}
		}

		public byte[] Key
		{
			get
			{
				return cryptoKey;
			}
			set
			{
				cryptoKey = value;
			}
		}

		public byte[] IV
		{
			get
			{
				return cryptoIV;
			}
			set
			{
				cryptoIV = value;
			}
		}

		public int ChunkSize
		{
			get
			{
				return chunkSize;
			}
			set
			{
				chunkSize = value;
			}
		}

		/// <summary>
		/// Constructs a Cryptographer with a specific cryptographer method.
		/// </summary>
		/// <param name="Method">the cryptographic method to use for encryption/decryption</param>
		/// <param name="Key">the public key</param>
		/// <param name="IV">the initialization vector</param>
		public Cryptographer(SymmetricAlgorithm Method, byte[] Key, byte[] IV)
		{
			this.Method = Method;
			this.Key = Key;
			this.IV = IV;
		}

		/// <summary>
		/// Encrypts data from a source stream using the current cryptographic method, 
		/// writing the result to a destination stream. 
		/// </summary>
		/// <param name="source">data source</param>
		/// <param name="destination">destination for the encrypted data</param>
		public void Encrypt(Stream source, Stream destination)
		{
			CryptoStream encStream = new CryptoStream(destination, 
				cryptoMethod.CreateEncryptor(cryptoKey, cryptoIV), 
				CryptoStreamMode.Write);

			byte[] buf = new byte[ChunkSize];
			long totlen = source.Length; // total length of the input
			long rdlen = 0; // total number of bytes written
			int len; // number of bytes per chunk of data transferred

			while (rdlen < totlen)
			{
				len = source.Read(buf, 0, ChunkSize);
				encStream.Write(buf, 0, len);
				rdlen += len;
            }

			encStream.FlushFinalBlock();
		}

		/// <summary>
		/// Convenience method for encrypting a file.
		/// </summary>
		/// <param name="sourceFilename">filename of source</param>
		/// <param name="destinationFilename">filename of encrypted destination</param>
		public void Encrypt(string sourceFilename, string destinationFilename)
		{
			FileStream sourceStream = new FileStream(sourceFilename,
				FileMode.Open,
				FileAccess.Read);
			FileStream destinationStream = new FileStream(destinationFilename,
				FileMode.Create,
				FileAccess.ReadWrite);
			Encrypt(sourceStream, destinationStream);
			sourceStream.Close();
			destinationStream.Close();
		}

		/// <summary>
		/// Decrypts data from a source stream using the current cryptographic method, 
		/// writing the result to a destination stream. 
		/// </summary>
		/// <param name="source">data source</param>
		/// <param name="destination">destination for the decrypted data</param>
		public void Decrypt(Stream source, Stream destination)
		{
			CryptoStream decStream = new CryptoStream(source, 
				cryptoMethod.CreateDecryptor(cryptoKey, cryptoIV), 
				CryptoStreamMode.Read);
			byte[] buf = new byte[ChunkSize];
			int len = -1; // number of bytes per chunk of data transferred
			
			while (len != 0)
			{
				len = decStream.Read(buf, 0, ChunkSize);
				destination.Write(buf, 0, len);
			}

			destination.Flush();
		}

		/// <summary>
		/// Convenience method for decrypting a file.
		/// </summary>
		/// <param name="sourceFilename">filename of encrypted source</param>
		/// <param name="destinationFilename">filename of decrypted destination</param>
		public void Decrypt(string sourceFilename, string destinationFilename)
		{
			FileStream sourceStream = new FileStream(sourceFilename,
				FileMode.Open,
				FileAccess.Read);
			FileStream destinationStream = new FileStream(destinationFilename,
				FileMode.Create,
				FileAccess.ReadWrite);
			Decrypt(sourceStream, destinationStream);
			sourceStream.Close();
			destinationStream.Close();
		}
	}
}
