using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asplib.Components
{
    /// <summary>
    /// Marker interface which reassures OnAfterRenderAsync signals
    /// TestFocus.Event and will expose itself on TestFocus.Component
    /// OnAfterRenderAsync.
    /// </summary>
    public interface IStaticComponent
    { }
}