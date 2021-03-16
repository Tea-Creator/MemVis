using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemVis
{
    public class Frame
    {
        public int Number { get; set; }
        public int Count { get; set; }

        public void Reset()
        {
            Count = 0;
        }
    }

    public class NRU
    {
        private readonly Frame[] _frames;
        private int _faults;

        public NRU(int framesCount)
        {
            _frames = new Frame[framesCount];
        }

        public void Push(Frame frame)
        {
            foreach (var existingFrame in _frames)
            {
                if (existingFrame is not null && existingFrame.Number == frame.Number)
                {
                    existingFrame.Reset();
                    return;
                }
            }

            int top = Array.IndexOf(_frames, null);

            if (top != -1)
            {
                _frames[top] = frame;
                _faults++;
            }
            else
            {
                int index = Array.IndexOf(_frames, _frames.OrderByDescending(f => f.Count).First());
                _frames[index] = frame;
                _faults++;
            }

            frame.Reset();

        }

        public int Faults => _faults;

        public void IncremenentAll()
        {
            foreach (var frame in _frames)
            {
                if (frame is not null)
                {
                    frame.Count++;
                }
            }
        }
    }
}
