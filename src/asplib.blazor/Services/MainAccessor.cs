using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asplib.Services
{
    public static class MainAccessor<T>
    {
        public static T? Instance { get; set; }
    }
}