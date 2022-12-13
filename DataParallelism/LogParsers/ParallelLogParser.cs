using System;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LogParsing.LogParsers
{
    public class ParallelLogParser: ILogParser
    {
        private readonly FileInfo file;
        private readonly Func<string, string> tryGetIdFromLine;
        private readonly ConcurrentQueue<string> ids;

        public ParallelLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
        {
            this.file = file;
            this.tryGetIdFromLine = tryGetIdFromLine;
            ids = new ConcurrentQueue<string>();
        }
        
        public string[] GetRequestedIdsFromLogFile()
        {
            var lines = File.ReadLines(file.FullName);
            Parallel.ForEach(lines, AddId);
            return ids.ToArray();
        }

        private void AddId(string line)
        {
            var id = tryGetIdFromLine(line);
            if (id != null)
                ids.Enqueue(id);
        }
    }
}