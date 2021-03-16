using System;

namespace MemVis
{
    public interface IBitmapMemoryManagerAlgorithm
    {
        void Start(Process process, MemoryBitmapEntry head);
    }

    public class BestFit : IBitmapMemoryManagerAlgorithm
    {
        public void Start(Process process, MemoryBitmapEntry head)
        {
            MemoryBitmapEntry entry = head;
            MemoryBitmapEntry min = null;

            while (entry != null)
            {
                if (entry.Type is MemoryBitmapEntryType.Hole && entry.Value >= process.Size)
                {
                    if (min is null)
                    {
                        min = entry;
                    }

                    if (entry.Value < min.Value)
                    {
                        min = entry;
                    }

                    if (min.Value == process.Size)
                    {
                        break;
                    }
                }

                entry = entry.Next;
            }

            if (min is null)
            {
                throw new Exception("Can't allocate memory");
            }

            if (min.Value == process.Size)
            {
                min.Type = MemoryBitmapEntryType.Process;
                min.Process = process;
            }
            else
            {
                MemoryBitmapEntry holeEntry = new(MemoryBitmapEntryType.Hole, min.Value - process.Size, min.Next);
                min.Next = holeEntry;
                min.Type = MemoryBitmapEntryType.Process;
                min.Value = process.Size;
                min.Process = process;
            }
        }
    }

    public class FirstFit : IBitmapMemoryManagerAlgorithm
    {
        public void Start(Process process, MemoryBitmapEntry head)
        {
            var entry = head;

            while (entry != null)
            {
                if (entry.Type is MemoryBitmapEntryType.Hole && entry.Value >= process.Size)
                {
                    if (entry.Value == process.Size) // Exact fit
                    {
                        entry.Type = MemoryBitmapEntryType.Process;
                        entry.Process = process;
                    }
                    else
                    {
                        MemoryBitmapEntry holeEntry = new(MemoryBitmapEntryType.Hole, entry.Value - process.Size, entry.Next);
                        entry.Next = holeEntry;
                        entry.Type = MemoryBitmapEntryType.Process;
                        entry.Value = process.Size;
                        entry.Process = process;
                    }

                    return;
                }

                entry = entry.Next;
            }

            throw new Exception("Can't allocate memory");
        }
    }
}
