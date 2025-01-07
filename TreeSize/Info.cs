using System;
using System.Windows.Controls;

namespace TreeSize
{
    public partial class Info
    {
         public double Size { get; set; }
         public double Allocated { get; set; }
         public int FilesCount { get; set; }
         public int FoldersCount { get; set; }
         public double Value { get; set; }
         public double Minimum { get; set; }
         public double Maximum { get; set; }
        public string LastModified { get; set; }
    }
}
