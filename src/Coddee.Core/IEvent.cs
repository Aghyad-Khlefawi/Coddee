using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee
{
    public interface IEvent
    {
        EventRoutingStrategy EventRoutingStrategy { get; }
    } 
}
