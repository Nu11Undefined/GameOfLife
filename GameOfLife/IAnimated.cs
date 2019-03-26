using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    /// <summary>
    /// Определяет возможность развития и отображения
    /// </summary>
    public interface IAnimated: IDeveloping, IDrawn
    {
    }
}
