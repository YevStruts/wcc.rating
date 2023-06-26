using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        //cfg.CreateMap<News, NewsModel>()
                        //    .ForMember(dest => dest.Image_url, act => act.MapFrom(src => src.ImageUrl));
                    });

                    instance = new Mapper(config);
                }
                return instance;
            }
        }
    }
}
