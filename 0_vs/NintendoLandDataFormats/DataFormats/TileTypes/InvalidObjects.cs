namespace NintendoLand.TileTypes
{
    public class Empty : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return '0';
            }
        }

        public override bool IsValid()
        {
            return false;
        }
    }

    public class Nothing : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return ' ';
            }
        }

        public override bool IsValid()
        {
            return false;
        }
    }

    public class Null : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return '\0';
            }
        }

        public override bool IsValid()
        {
            return false;
        }
    }

    class HeaderUnknown : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return '#';
            }
        }

        public override bool IsValid()
        {
            return false;
        }
    }

    public class Wall : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return '*';
            }
        }

        public override bool IsValid()
        {
            return false;
        }
    }
}
