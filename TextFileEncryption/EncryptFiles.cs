using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace TextFileEncryption
{
    internal static class EncryptFiles
    {
        public static void Encrypt(string inputFilePath, string outputDir, string password, int threadCount, int chunkSize)
        {
            try
            {
                List<string> encryptedFiles = new();
                List<byte[]> chunksEncrypted = new();

                object locker = new();
                int totalchunks = 0;
                using (FileStream fs = new(inputFilePath, FileMode.Open, FileAccess.Read))
                {
                    totalchunks = (int)Math.Ceiling(Convert.ToDecimal(fs.Length) / Convert.ToDecimal(chunkSize));
                }

                // Create a new instance of the encryption algorithm
                Aes aes = Aes.Create();

                // Set the password as the key and IV
                byte[] key = Encoding.UTF8.GetBytes(password);
                byte[] iv = Encoding.UTF8.GetBytes(password);
                aes.Key = key;
                aes.IV = iv;

                // Divide the input files into chunks and add them to a thread-safe queue
                ConcurrentQueue<byte[]> chunksToProcess = DivideFileIntoChunks(inputFilePath, chunkSize);

                int chunksProcessed = 0;

                // Create the worker threads
                // Create a list to hold the threads
                List<Thread> workerThreads = new();
                for (int i = 0; i < threadCount; i++)
                {
                    workerThreads.Add(new Thread(() =>
                    {
                        while (chunksToProcess.TryDequeue(out byte[] chunk))
                        {
                            byte[] encryptedChunk = EncryptChunk(chunk, key);
                            chunksEncrypted.Add(encryptedChunk);
                            Interlocked.Increment(ref chunksProcessed);
                        }
                    }));
                }
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Start the worker threads
                foreach (Thread thread in workerThreads)
                {
                    thread.Start();
                }

                // Wait for all worker threads to complete
                foreach (Thread thread in workerThreads)
                {
                    thread.Join();
                }
                // Stop the stopwatch
                stopwatch.Stop();

                if (chunksProcessed == totalchunks)
                {
                    // Combine the encrypted chunks into a single encrypted file
                    CombineChunksToFile(chunksEncrypted, outputDir);
                    // Output the time taken to encrypt all chunks
                    Console.WriteLine($"Time taken to encrypt all chunks: {stopwatch.ElapsedMilliseconds} ms");

                }
                else
                {
                    Console.WriteLine("All chunks were not processed!!!");
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        // Combines a list of encrypted chunks into a single encrypted file
        public static void CombineChunksToFile(List<byte[]> chunks, string outputPath)
        {
            try
            {
                using FileStream fs = new(outputPath, FileMode.Create, FileAccess.Write);
                foreach (byte[] chunk in chunks)
                {
                    fs.Write(chunk, 0, chunk.Length);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured while combining files!!!");
            }
        }


        // Encrypts a single chunk using the specified key
        public static byte[] EncryptChunk(byte[] chunk, byte[] key)
        {
            using MemoryStream ms = new();
            try
            {
                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.GenerateIV();

                ms.Write(aes.IV, 0, aes.IV.Length);
                using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(chunk, 0, chunk.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured while encrypting!!!!");
            }
            return ms.ToArray();
        }
        public static ConcurrentQueue<byte[]> DivideFileIntoChunks(string filePath, int chunkSize)
        {
            Console.WriteLine($"Divided the file..........");
            ConcurrentQueue<byte[]> chunks = new();
            try
            {
                using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
                {
                    long fileSize = fs.Length;
                    int bytesRead = 0;
                    long totalBytesRead = 0;
                    byte[] buffer = new byte[chunkSize];

                    while ((bytesRead = fs.Read(buffer, 0, chunkSize)) > 0)
                    {
                        byte[] chunk = new byte[bytesRead];
                        Array.Copy(buffer, chunk, bytesRead);
                        chunks.Enqueue(chunk);
                        totalBytesRead += bytesRead;
                    }
                }
                Console.WriteLine($"Divided the file into {chunks.Count} chunks");
                return chunks;
            }
            catch (Exception)
            {
                Console.WriteLine("Error while dividing file into small chunks!!!");
            }
            return chunks;
        }
    }
}
