using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorGfx2.Models.Interfaces;

public interface IGameObject : ITypedObject
{
    int Idx { get; set; }
}
