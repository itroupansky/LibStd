using System.Collections.Generic;
using AutoMapper;

namespace LibStd.Common
{
    /// <summary>
    /// helper with automap functionality
    /// </summary>
    /// <summary>
    /// helper with automap functionality
    /// </summary>
    public class AutoMapping
    {
        /// <summary>
        /// create mapper for particular types
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <returns></returns>
        public static IMapper GetMapper<T, Q>()
        {

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T, Q>();
            });
            return GetMapper<T, Q>(config);

        }

        /// <summary>
        /// create mapper for particular types
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <param name="cfg">Custom Mapper Configuration</param>
        /// <returns></returns>
        public static IMapper GetMapper<T, Q>(MapperConfiguration cfg)
        {
            IMapper mapper = cfg.CreateMapper();
            return mapper;
        }

        /// <summary>
        /// map lists of given types
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <param name="listin">source list</param>
        /// <returns></returns>
        public static List<Q> MapLists<T, Q>(List<T> listin)
        {
            IMapper mapper = GetMapper<T, Q>();
            List<Q> listout = mapper.Map<List<T>, List<Q>>(listin);
            return listout;
        }
        /// <summary>
        /// map lists of given types
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <param name="listin">source list</param>
        /// <returns></returns>
        public static void MapLists<T, Q>(List<T> listin, List<Q> listout)
        {
            IMapper mapper = GetMapper<T, Q>();
            mapper.Map<List<T>, List<Q>>(listin, listout);

        }
        /// <summary>
        /// map lists of given types
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <param name="listin">source list</param>
        /// <returns></returns>
        public static List<Q> MapLists<T, Q>(List<T> listin, MapperConfiguration cfg)
        {
            IMapper mapper = GetMapper<T, Q>(cfg);
            List<Q> listout = mapper.Map<List<T>, List<Q>>(listin);
            return listout;
        }
        /// <summary>
        /// map 2 objects
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <param name="source">source object</param>
        /// <returns>destination object</returns>
        public static Q Map<T, Q>(T source)
        {
            IMapper mapper = GetMapper<T, Q>();
            return mapper.Map<T, Q>(source);
        }

        /// <summary>
        /// map 2 objects
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <param name="source">source object</param>
        /// <returns>destination object</returns>
        public static Q Map<T, Q>(T source, MapperConfiguration cfg)
        {
            IMapper mapper = GetMapper<T, Q>(cfg);
            return mapper.Map<T, Q>(source);
        }

        // <summary>
        /// map 2 objects -updating existing one
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <param name="source">source object</param>
        /// <returns>destination object</returns>
        public static void Map<T, Q>(T source, Q destination)
        {
            IMapper mapper = GetMapper<T, Q>();
            mapper.Map<T, Q>(source, destination);
        }

        // <summary>
        /// map 2 objects -updating existing one
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <typeparam name="Q">destination type</typeparam>
        /// <param name="source">source object</param>
        /// <returns>destination object</returns>
        public static void Map<T, Q>(T source, Q destination, MapperConfiguration config)
        {
            IMapper mapper = GetMapper<T, Q>(config);
            mapper.Map<T, Q>(source, destination);
        }


    }
}
