using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsEpiphany.Model.Interfaces;

public interface IEntity
{
    int get_position_x();
    void set_id(Entity_Handle handle);
}
