﻿using System;
using System.Collections.Generic;

namespace apiservice.Model.Db
{
    public partial class Main
    {
        public Main()
        {
            Accesscode = new HashSet<Accesscode>();
        }

        public long Mainid { get; set; }
        public Guid Session { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid Clsid { get; set; }
        public byte[] Main1 { get; set; }

        public virtual ICollection<Accesscode> Accesscode { get; set; }
    }
}