using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace LogParsing.LogParsers
{
    public class ThreadLogParser: ILogParser
    {
        private readonly FileInfo file;
        private readonly Func<string, string> tryGetIdFromLine;
        private readonly ConcurrentQueue<string> ids;
        private const int ThreadsCount = 5;

        public ThreadLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
        {
            this.file = file;
            this.tryGetIdFromLine = tryGetIdFromLine;
            ids = new ConcurrentQueue<string>();
        }
        
        public string[] GetRequestedIdsFromLogFile()
        {
            var lines = File.ReadLines(file.FullName);
            RunThreads(lines.ToList());

            return ids.ToArray();
        }

        private void RunThreads(List<string> lines)
        {
            // не уверен, нужно ли лочить lines и ids
            var slicesCount = lines.Count < ThreadsCount ? lines.Count : ThreadsCount;
            var sliceLength = lines.Count < ThreadsCount ? 1 : lines.Count / ThreadsCount;
            var lastSliceLength = lines.Count / ThreadsCount + 1;
            var threads = new List<Thread>();
            for (var i = 0; i < slicesCount - 1; i++)
            {
                var j = i;
                var thread = new Thread(
                    () => AddIdsFromSlice(lines, j * sliceLength, j * sliceLength + sliceLength));
                threads.Add(thread);
                thread.Start();
            }
            var lastThread = new Thread(
                () => AddIdsFromSlice(lines, lines.Count - lastSliceLength, lines.Count));
            lastThread.Start();
            foreach (var thread in threads)
                thread.Join();
            lastThread.Join();
        }

        private void AddIdsFromSlice(List<string> lines, int start, int end)
        {
            for (var i = start; i < end; i++)
            {
                AddId(lines[i]);
            }
        }
        
        private void AddId(string line)
        {
            var id = tryGetIdFromLine(line);
            if (id != null)
                ids.Enqueue(id);
        }
    }
}