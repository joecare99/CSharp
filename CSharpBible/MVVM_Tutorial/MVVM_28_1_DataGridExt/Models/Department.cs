﻿using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_28_1_DataGridExt.Models
{
    public class Department : NotificationObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}