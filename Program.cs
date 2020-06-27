using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Practice
{

    class Program
    {
        static void Main(string[] args)
        {
            string message = "";
            int value;
            Console.Write("Enter a string: ");
            message = Console.ReadLine();

            Console.Write("Select the digest's length ( 2, 4, 8 ): ");
            value = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\nWorking with a string");
            string msgHash = hash(message, value);
            Console.WriteLine("original hash -> {0}", msgHash);

            Console.WriteLine("\nTrying to find a collision:");
            Console.ReadKey();
            int col = searchCollisions(msgHash, msgHash.Length, value);

            if (col > 0)
                Console.WriteLine("\nTotal collisions: {0}", col);
            else
                Console.WriteLine("No collisions found");

            Console.WriteLine("\nWorking with changed string: ");
            message += 'a';
            Console.WriteLine("changed string is \"{0}\"", message);
            Console.WriteLine("changed hash -> {0}", hash(message, value));

            Console.Write("\nWorking with images: ");
            string path = @"1.png";
            Console.WriteLine("hash -> {0}", hash(getBytes(path), value));

            Console.Write("\nWorking with file: ");
            string path2 = @"Lab3.csproj";
            Console.WriteLine("hash -> {0}", hash(getBytes(path2), value));

            Console.Write("\nWorking with doc: ");
            string path3 = @"File.docx";
            Console.WriteLine("hash -> {0}", hash(getBytes(path3), value));
            Console.WriteLine("\nWorking with MD5");
            using (MD5 md5Hash = MD5.Create())
            {
                msgHash = GetMd5Hash(md5Hash, path3);

                Console.WriteLine("The MD5 hash of " + path3 + " is: " + msgHash);

                Console.WriteLine("Verifying the hash...");

                if (VerifyMd5Hash(md5Hash, path3, msgHash))
                {
                    Console.WriteLine("The hashes are the same.");
                }
                else
                {
                    Console.WriteLine("The hashes are not the same.");
                }
            }

            Console.WriteLine("\nTrying to find a collision:");

            col = searchMD5Collisions(path3, msgHash);

            if (col > 0)
                Console.WriteLine("\nTotal collisions: {0}", col);
            else
                Console.WriteLine("No collisions found");
            Console.ReadKey();
        }
        private static string hash(string message, int value)
        {
            string byteMessage = "";
            for (int i = 0; i < message.Length; i++)
            {
                byteMessage += "0" + Convert.ToString(Convert.ToInt64(message[i]), 2);
            }

            int blokSize = byteMessage.Length / 8;

            byte[] bloks = new byte[blokSize];

            for (int i = 0; i < blokSize; ++i)
            {
                bloks[i] = Convert.ToByte(byteMessage.Substring(8 * i, 8), 2);
            }
            byte result_b = 0;

            foreach (byte b in bloks)
                result_b ^= b;

            int res = result_b >> (8 - value);

            return Convert.ToString(res, 2);
        }

        private static string getBytes(string path)
        {
            string bytes = "";
            byte[] bData = File.ReadAllBytes(path);

            for (int i = 0; i < bData.Length; i++)
            {
                bytes += bData[i];
            }
            return bytes;
        }

        private static string getRandomString(int len, int iter)
        {
            Random rnd = new Random(iter);
            byte[] rndBytes = new Byte[len];
            rnd.NextBytes(rndBytes);
            return System.Text.Encoding.UTF8.GetString(rndBytes);
        }
        private static int searchCollisions(string msgHash, int msgLen, int value)
        {
            int total = 0, iter = 0;
            string rndString = "";
            string rndHash = "";

            DateTime timeNext = System.DateTime.Now.AddSeconds(10);
            while (System.DateTime.Now < timeNext)
            {
                rndString = getRandomString(msgLen, iter++);
                rndHash = hash(rndString, value);

                if (rndHash == msgHash)
                {
                    Console.WriteLine($"Collison's found! Message: {rndString} \nhash: {rndHash}");
                    total++;
                }
            }

            return total;
        }
        static string GetMd5Hash(MD5 md5Hash, string path)
        {

            // Convert the input string to a byte array and compute the hash.
            string message = File.ReadAllText(path);
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(message));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        static bool VerifyMd5Hash(MD5 md5Hash, string path, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, path);

            // Create a StringComparer an csompare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static int searchMD5Collisions(string path, string msgHash)
        {
            string message = File.ReadAllText(path);
            int total = 0, iter = 0;
            byte[] rndString;
            string rndHash = "";

            DateTime timeNext = System.DateTime.Now.AddSeconds(10);
            while (System.DateTime.Now < timeNext)
            {
                string _rndString = getRandomString(message.Length, iter++);
                rndString = System.Text.Encoding.UTF8.GetBytes(_rndString);
                StringBuilder sBuilder = new StringBuilder();
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] data = md5Hash.ComputeHash(rndString);

                    // Loop through each byte of the hashed data 
                    // and format each one as a hexadecimal string.
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                }
                
                rndHash = sBuilder.ToString();
                Console.WriteLine($"hash: { rndHash}");
                if (rndHash == msgHash)
                {
                    Console.WriteLine($"Collison's found! Message: {_rndString} \nhash: {rndHash}");
                    total++;
                }
            }

            return total;
        }
    }
}