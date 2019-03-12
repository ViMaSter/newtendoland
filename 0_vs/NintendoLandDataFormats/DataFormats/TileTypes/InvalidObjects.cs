namespace NintendoLand.TileTypes
{
    class Empty : BaseType
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

    class Nothing : BaseType
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

    class Null : BaseType
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

    class Wall : BaseType
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
