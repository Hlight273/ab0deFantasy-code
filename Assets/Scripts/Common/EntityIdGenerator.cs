namespace HFantasy.Script.Commonpublic
{
    static class EntityIdGenerator
    {
        private static int globalId = 0;

        public static int GetNextId()
        {
            globalId++;
            return globalId;
        }
    }
}