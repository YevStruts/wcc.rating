using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.Infrastructure;

namespace wcc.rating.data
{
    public interface IDataRepository
    {
        List<Rating> GetRating();

        bool SaveGame();

        bool UpdateGame();
    }
}
