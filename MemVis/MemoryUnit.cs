namespace MemVis
{
    public static class MemoryUnit
    {
        public static long Gb(long count)
        {
            return Mb(count) * 1024L;
        }

        public static long Mb(long count)
        {
            return Kb(count) * 1024L;
        }

        public static long Kb(long count)
        {
            return count * 1024L;
        }
    }
}
