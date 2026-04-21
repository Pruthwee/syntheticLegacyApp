// =============================================================================
// RULE ID   : cr-dotnet-0036
// RULE NAME : Large Object Heap Issues
// CATEGORY  : Memory
// DESCRIPTION: Application allocates large objects (>85KB) that cause Large Object
//              Heap (LOH) fragmentation. In containerized environments with memory
//              limits, LOH fragmentation leads to memory pressure, inefficient
//              garbage collection, and out-of-memory conditions causing container
//              termination.
// =============================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace SyntheticLegacyApp.Memory
{
    public class ReportDataLoader
    {
        // VIOLATION cr-dotnet-0036: Large static buffer pre-allocated on LOH (>85KB)
        private static readonly byte[] _exportBuffer = new byte[1024 * 1024]; // 1 MB — always on LOH

        public byte[] LoadFullReport(string reportPath)
        {
            // VIOLATION cr-dotnet-0036: ReadAllBytes loads entire file into a single large array
            byte[] reportData = File.ReadAllBytes(reportPath);
            return reportData;
        }

        public List<string[]> LoadCsvIntoMemory(string csvPath)
        {
            var rows = new List<string[]>();

            // VIOLATION cr-dotnet-0036: ReadAllLines loads entire file as one large string array
            string[] allLines = File.ReadAllLines(csvPath);

            foreach (var line in allLines)
                rows.Add(line.Split(','));

            return rows;
        }

        public byte[] BuildExportPackage(List<byte[]> reportChunks)
        {
            // VIOLATION cr-dotnet-0036: Concatenation creates a single large byte array on LOH
            int totalSize = 0;
            foreach (var chunk in reportChunks) totalSize += chunk.Length;

            byte[] exportPackage = new byte[totalSize]; // often well above 85 KB threshold
            int offset = 0;
            foreach (var chunk in reportChunks)
            {
                Buffer.BlockCopy(chunk, 0, exportPackage, offset, chunk.Length);
                offset += chunk.Length;
            }

            return exportPackage;
        }

        public string LoadXmlConfigAsString(string xmlPath)
        {
            // VIOLATION cr-dotnet-0036: ReadAllText returns single large string — LOH candidate
            return File.ReadAllText(xmlPath);
        }

        public void ProcessBatch(string dataFile)
        {
            // VIOLATION cr-dotnet-0036: repeated large allocations never released between calls
            for (int i = 0; i < 50; i++)
            {
                byte[] batchBuffer = new byte[200 * 1024]; // 200 KB per iteration
                File.ReadAllBytes(dataFile).CopyTo(batchBuffer, 0);
                Console.WriteLine($"Processed batch {i}");
                // batchBuffer goes out of scope but LOH is not compacted by default GC
            }
        }
    }
}
