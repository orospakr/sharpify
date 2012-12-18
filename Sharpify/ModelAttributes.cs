using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify
{
    /// <summary>
    /// Whether or not this resource on the service must be
    /// accessed with the pagination behaviour with the ?page=
    /// query parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class Paginated : System.Attribute
    {
    }
}
