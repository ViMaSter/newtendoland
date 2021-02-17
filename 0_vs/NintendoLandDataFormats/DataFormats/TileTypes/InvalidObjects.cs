namespace NintendoLand.TileTypes
{
    public class Empty : BaseType
    {
        public override char MemoryIdentifier => '0';

        public override bool IsValid()
        {
            return false;
        }
    }

    public class Nothing : BaseType
    {
        public override char MemoryIdentifier => ' ';

        public override bool IsValid()
        {
            return false;
        }
    }

    public class Null : BaseType
    {
        public override char MemoryIdentifier => '\0';

        public override bool IsValid()
        {
            return false;
        }
    }

    class HeaderUnknown : BaseType
    {
        public override char MemoryIdentifier => '#';

        public override bool IsValid()
        {
            return false;
        }
    }

    public class Wall : BaseType
    {
        public override char MemoryIdentifier => '*';

        public override bool IsValid()
        {
            return false;
        }
    }
}
