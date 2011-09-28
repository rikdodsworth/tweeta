using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using Tweeta.Common.Logging;

namespace Tweeta
{
    public class SerializationManager
    {
        private static object lockObj = new object();
        private static List<string> inprogress = new List<string>();

        internal static void SerializeData<T>(string name, T data)
        {
            Log.Info("Save Data " + name + " - " + Thread.CurrentThread.ManagedThreadId);

            lock (lockObj)
            {
                if (inprogress.Contains(name))
                    return;

                inprogress.Add(name);
            }
        
            ThreadPool.QueueUserWorkItem((obj) => SerializeData_Internal<T>(name, data));
        }

        internal static void DeSerializeData<T>(string name, Action<T> callback)
        {
            ThreadPool.QueueUserWorkItem((obj) => DeserializeData_Internal<T>(name, callback));
        }

        private static void SerializeData_Internal<T>(string name, T data)
        {
            Log.Info("Saving Data " + name + " - " + Thread.CurrentThread.ManagedThreadId);

            using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (var stream = store.OpenFile(name, System.IO.FileMode.Create))
                    {
                        var ser = new XmlSerializer(typeof(T));
                        ser.Serialize(stream, data);
                    }
                }
                catch (Exception err)
                {
                    Log.Warning("failed to serialize data \n" + err.Message);
                }
                finally
                {
                    lock (lockObj)
                    {
                        inprogress.Remove(name);
                    }
                }
            }
        }

        private static void DeserializeData_Internal<T>(string name, Action<T> callback)
        {
            Log.Info("Load File " + name + " - " + Thread.CurrentThread.ManagedThreadId);

            using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.FileExists(name))
                {
                    callback(default(T));
                    return;
                }

                T data = default(T);

                using (var stream = store.OpenFile(name, System.IO.FileMode.Open))
                {
                    try
                    {
                        var ser = new XmlSerializer(typeof(T));
                        data = (T)ser.Deserialize(stream);
                    }
                    catch (Exception err)
                    {
                        Log.Warning("failed to serialize data \n" + err.Message);
                    }
                }

                callback(data);
            }
        }
    }
}
