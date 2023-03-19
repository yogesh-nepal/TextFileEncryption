using System.Diagnostics;
using System.Security.Cryptography;

namespace TextFileEncryption
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filename = "D:\\[PROJECT]\\TextFileEncryption\\TextFileEncryption\\Data\\test.txt";// replace with your desired path
            string outputDir = "D:\\[PROJECT]\\TextFileEncryption\\TextFileEncryption\\Data\\testOutput.txt";
            int smallFileSize = 10 * 1024 * 1024;//10MB
            int smallFileSizeMB = 10;//10MB

            int numberOfThreads = Environment.ProcessorCount;// use the number of available CPU cores

            CreateFile.CreateNewTextFile(2048, numberOfThreads, smallFileSizeMB, filename);

            EncryptFiles.Encrypt(filename, outputDir, "1fhgjt@3#klgv23g", numberOfThreads, smallFileSize);

            #region CREATING TEXT FILE
            //int fileSizeInMB = 2048;

            //// Start the stopwatch
            //Stopwatch stopwatch = Stopwatch.StartNew();

            //using (StreamWriter writer = new StreamWriter(filename))
            //{
            //    for (int i = 0; i < fileSizeInMB; i++)
            //    {
            //        string text = GenerateRandomText(1024 * 1024); // 1MB
            //        writer.Write(text);
            //    }
            //}

            //// Stop the stopwatch
            //stopwatch.Stop();
            //Console.WriteLine("File created successfully!");
            //Console.WriteLine($"Time taken to create a file of {fileSizeInMB} MB : {stopwatch.ElapsedMilliseconds} ms");

            //Console.ReadKey();
            #endregion
        }

        private static string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}