namespace Chroma.ColorSystem
{
    public struct HashIndexPair
    {
        public int Hash { get; }
        public int Index { get; }

        public HashIndexPair(int hash, int index)
        {
            Hash = hash;
            Index = index;
        }
    }
}
