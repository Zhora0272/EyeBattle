namespace Vengadores.Utility.IdGeneration
{
    public static class IdGenerator
    {
        private static int _id = 0;

        public static int GetNewId()
        {
            _id++;
            return _id;
        }
    }
}