using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemVis
{
    public class BitmapMemoryManager : IMemoryManager
    {
        public IBitmapMemoryManagerAlgorithm Algorithm { get; set; }

        public BitmapMemoryManager(long memoryAmount)
        {
            Head = new MemoryBitmapEntry(MemoryBitmapEntryType.Hole, memoryAmount);
        }

        public MemoryBitmapEntry Head { get; }

        public void LoadToMemory(Process process)
        {
            process.Killed += OnProcessKilled;

            Algorithm.Start(process, Head);
        }

        private void OnProcessKilled(object sender, ProcessKilledEventArgs eventArgs)
        {
            MemoryBitmapEntry previous = null, current = Head;

            while (current != null)
            {
                if (current.Type is MemoryBitmapEntryType.Process && current.Process.Pid == eventArgs.Pid)
                {
                    current.Type = MemoryBitmapEntryType.Hole;
                    current.Process.Killed -= OnProcessKilled;
                    current.Process = null;

                    HoleMerger.MergeHoles(previous, current);
                }

                previous = current;
                current = current.Next;
            }
        }
    }

    public static class MemoryBitmapViewBuilder
    {
        public static string BuildView(MemoryBitmapEntry head)
        {
            StringBuilder view = new();

            var current = head;

            List<MemoryBitmapEntry> entries = new();

            while (current != null)
            {
                entries.Add(current);
                current = current.Next;
            }

            view.AppendJoin("=>", entries.Select(entry =>
            {
                var symbol = entry.Type is MemoryBitmapEntryType.Hole ? 'H' : 'P';
                return $"[{symbol}|{entry.Value}]";
            }));

            return view.ToString();
        }
    }

    public static class HoleMerger
    {
        public static void MergeHoles(MemoryBitmapEntry previous, MemoryBitmapEntry current)
        {
            if (previous is not null && previous.Type is MemoryBitmapEntryType.Hole)
            {
                previous.Value += current.Value;
                previous.Next = current.Next;
                MergeHoles(null, previous);
                return;
            }

            if (current.Next is not null && current.Next.Type is MemoryBitmapEntryType.Hole)
            {
                MergeHoles(current, current.Next);
                return;
            }
        }
    }

    public class MemoryBitmapEntry : IEnumerable<MemoryBitmapEntry>
    {
        public MemoryBitmapEntry(
            MemoryBitmapEntryType type,
            long value,
            MemoryBitmapEntry next = null,
            Process process = null)
        {
            Type = type;
            Value = value;
            Next = next;
            Process = process;
        }

        public MemoryBitmapEntryType Type { get; set; }
        public long Value { get; set; }
        public MemoryBitmapEntry Next { get; set; }
        public Process Process { get; set; }

        public IEnumerator<MemoryBitmapEntry> GetEnumerator()
        {
            var current = this;

            while (current is not null)
            {
                yield return current;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public enum MemoryBitmapEntryType
    {
        Process,
        Hole
    }
}
