namespace HFantasy.Script.Entity
{
    public abstract class BaseEntity
    {
        public BaseEntity() { }
        public int Cid { get; protected set; }

        public int NetId { get; protected set; }

    }
}