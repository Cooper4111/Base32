using System;
using System.IO;
using System.Text;

namespace Base32
{
    class Program
    {
        static void Main(string[] args)
        {
            string logPath = "C:\\_DEBUG_LOGS\\Base32.txt";
            string initial = "hello χγύ!!";
            (string, string) text = run(initial);
            string encoded = text.Item1;
            string decoded = text.Item2;

            Console.WriteLine($"initial len: {initial.Length}");
            printChars(initial);
            //Console.WriteLine($"encoded len: {encoded.Length}");
            Console.WriteLine($"decoded len: {decoded.Length}");
            printChars(decoded);
            using (FileStream FS = new FileStream(logPath, FileMode.OpenOrCreate))
            {
                FS.Write(Encoding.UTF8.GetBytes(encoded + "\n" + decoded));
            }
        }

        static void printChars(string S)
        {
            foreach (char ch in S)
                Console.WriteLine($"{(int)ch} - {Convert.ToString(ch, 2)}");
        }

        static (string, string) run(string S)
        {
            Console.WriteLine("initial: " + S + " EOF\n");
            var endoced = Base32.EncodeBase32(S);
            Console.WriteLine("encoded: " + endoced + " EOF\n");
            var decoded = Base32.DecodeBase32(endoced);
            Console.WriteLine("decoded: " + decoded + " EOF\n");
            return (endoced, decoded);
            //Base32.StrToBase32("Helloe Ϡ");
        }
        public static byte[] Concat(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            a.CopyTo(c, 0);
            b.CopyTo(c, a.Length);
            return c;
        }
    }
}
