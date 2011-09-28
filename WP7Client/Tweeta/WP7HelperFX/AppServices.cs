using System;
using System.Collections.Generic;

namespace WP7HelperFX
{
    public class AppServices
    {
        private static Dictionary<Type, IAppService> services = new Dictionary<Type, IAppService>();

        public static void AddService<T>() where T: IAppService, new()
        {
            var service = new T();
            services.Add(typeof(T), service);
        }

        public static T GetService<T>() where T: IAppService
        {
            T service = default(T);
            foreach (var item in services.Keys)
            {
                if(item == typeof(T))
                {
                    service = (T)services[item];
                    break;
                }
                var interf = item.GetInterface(typeof(T).FullName, false);
                if(interf!= null)
                {
                    service = (T)services[item];
                    break;
                }
            }
            
            return service;
        }
    }
}
