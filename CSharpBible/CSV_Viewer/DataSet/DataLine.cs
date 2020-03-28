using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpBible.CSV_Viewer.DataSet
{
    public partial class DataLine : UserControl
    {
        public TFieldDefs FieldDefs { get; set; }

        public DataLine()
        {
            InitializeComponent();
        }
    }
}
