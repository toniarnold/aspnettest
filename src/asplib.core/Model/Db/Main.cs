﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace asplib.Model.Db
{
    public partial class Main
    {
        public long Mainid { get; set; }
        public Guid Session { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public Guid Clsid { get; set; }
        public byte[] Main1 { get; set; } = null!;
    }
}