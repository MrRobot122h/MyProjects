using System.Collections.Generic;

namespace TreeSize
{
    public class CurrentInfo : List<Info>
    {
        public CurrentInfo() { }
        public void AddInfo(Info info)
        {
            this.Add(info);
        }
    }
}
