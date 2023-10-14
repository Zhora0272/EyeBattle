public static class HashManager
{
    private static IHashableData hashableData;

    static HashManager()
    {
        hashableData = new BinaryHash();
    }

    public static string HashData(string data)
    {
        return hashableData.HashData(data);
    }

    public static string DeHashData(string data)
    {
        return hashableData.DeHashData(data);
    }
}