using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.Infrastructure;
using wcc.rating.kernel.Models;
using wcc.rating.kernel.Models.C3;

namespace wcc.rating.kernel.Helpers
{
    public sealed class MapperHelper
    {
        private static IMapper? instance = null;

        private MapperHelper()
        {
        }

        public static IMapper Instance
        {
            get
            {
                if (instance == null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Game, GameModel>().ReverseMap();
                        cfg.CreateMap<Rating, RatingModel>().ReverseMap();
                        cfg.CreateMap<Rank, C3RankItemModel>().ReverseMap();
                        cfg.CreateMap<RatingGame1x1, RatingGame1x1Model>().ReverseMap();
                        cfg.CreateMap<RatingGameSettings1x1, RatingGameSettings1x1Model>().ReverseMap();
                    });

                    instance = new Mapper(config);
                }
                return instance;
            }
        }
    }
}
