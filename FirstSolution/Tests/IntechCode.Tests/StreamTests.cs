using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntechCode;

namespace IntechCode.Tests
{
    [TestFixture]
    public class StreamTests
    {
        const string demoPath = @"C:\Intech\2017-1\S7-8\2017-1-IL-A4\FirstSolution\Tests\IntechCode.Tests\StreamTests.cs";
        const string outputPath = demoPath + ".bak";

        [Test]
        public void copying_a_stream()
        {
            using (FileStream input = new FileStream(demoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                CopyFromTo(input, output);
            }
            File.ReadAllBytes(demoPath).ShouldBeEquivalentTo(File.ReadAllBytes(outputPath));
        }

        [Test]
        public void krabouille_and_unkrabouille()
        {
            // demoPath => demoPath+".crypt" => demoPath+".clear"
            const string inputPath = demoPath;
            const string cryptPath = demoPath + ".crypt";
            const string clearPath = demoPath + ".clear";

            // 1 - Writing cryptPath file.
            using (Stream input = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream output = new FileStream(cryptPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (Stream outputK = new KrabouilleStream(output, "pwd!", KrabouilleMode.Krabouille))
            {
                //CopyFromTo(input, outputK);
                // (or using the standard method:)
                input.CopyTo(outputK, 4 * 1024);
            }
            File.ReadAllBytes(inputPath).Should().NotEqual(File.ReadAllBytes(cryptPath));
            // 2 - Writing clearPath file (from cryptPath).

            File.ReadAllBytes(inputPath).Should().Equal(File.ReadAllBytes(clearPath));
        }

        static void CopyFromTo( Stream input, Stream output, byte[] buffer = null )
        {
            if (input == null || !input.CanRead) throw new ArgumentException("Must be readable.", nameof(input));
            if (output == null || !output.CanWrite) throw new ArgumentException("Must be writable.", nameof(output));
            if (buffer == null) buffer = new byte[4 * 1024];

            int lenRead;
            do
            {
                lenRead = input.Read(buffer, 0, buffer.Length);
                output.Write(buffer, 0, lenRead);
            }
            while(lenRead == buffer.Length);
        }
    }
}
