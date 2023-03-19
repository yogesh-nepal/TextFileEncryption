using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFileEncryption
{
    public static class CreateFile
    {
        public static void CreateNewTextFile(int fileSizeInMB, int numThreads, int chunkSizeInMB, string fileName)
        {
            // Create a list to store the threads
            List<Thread> threads = new List<Thread>();

            // Start the stopwatch
            Stopwatch stopwatch = Stopwatch.StartNew();
            object fileLock = new object();
            long totalBytesWritten = 0;


            for (int i = 0; i < numThreads; i++)
            {
                // Determine the start and end positions for this thread's chunk of the file
                int startPosition = i * chunkSizeInMB * 1024 * 1024;
                int endPosition = (i + 1) * chunkSizeInMB * 1024 * 1024;

                // Create a new thread to generate and write random text
                Thread thread = new Thread(() =>
                {
                    lock (fileLock)
                    {
                        using (FileStream stream = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                for (int j = startPosition; j < endPosition; j += 1024 * 1024)
                                {
                                    string text = GenerateRandomText(1024 * 1024); // 1MB

                                    writer.BaseStream.Seek(j, SeekOrigin.End);
                                    writer.Write(text);
                                    Interlocked.Add(ref totalBytesWritten, text.Length);
                                    if (totalBytesWritten >= Convert.ToInt64(fileSizeInMB * 1024L * 1024L))
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                });

                // Add the thread to the list and start it
                threads.Add(thread);
                thread.Start();
            }
            // Wait for all threads to finish
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Stop the stopwatch
            stopwatch.Stop();
            Console.WriteLine("File created successfully!");
            Console.WriteLine($"Time taken to create a file of {fileSizeInMB} MB with {numThreads} threads: {stopwatch.ElapsedMilliseconds} ms");

            Console.ReadKey();
        }
        private static string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
