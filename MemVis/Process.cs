using System;

namespace MemVis
{
    public class Process
    {
        public Process(long size)
        {
            Pid = Guid.NewGuid();

            Size = size;
        }

        public event EventHandler<ProcessKilledEventArgs> Killed;

        public Guid Pid { get; }
        public long Size { get; }

        public void Kill()
        {
            Killed?.Invoke(this, new ProcessKilledEventArgs(Pid));
        }
    }

    public class ProcessKilledEventArgs : EventArgs
    {
        public ProcessKilledEventArgs(Guid pid)
        {
            Pid = pid;
        }

        public Guid Pid { get; }
    }
}
