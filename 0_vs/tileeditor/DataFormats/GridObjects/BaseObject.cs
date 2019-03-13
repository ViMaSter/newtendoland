using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    public abstract class BaseObject
    {
        protected BaseObject() { }
        public abstract string DisplayName { get; }
        public abstract string GetIconFileName { get; }
        public virtual string DisplayData
        {
            get
            {
                return "";
            }
        }

        public abstract bool PopulateFields(ref Grid grid);
        public abstract void ObtainData();
        public virtual bool IsValid()
        {
            return true;
        }
    }
}
