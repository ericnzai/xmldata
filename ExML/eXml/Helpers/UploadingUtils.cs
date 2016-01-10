using System;
using System.IO;
using System.Web;
using System.Web.Caching;


namespace eXml.Helpers
{
    public class UploadingUtils
    {
        const string RemoveTaskKeyPrefix = "DXRemoveTask_";
        public static void RemoveFileWithDelay(string key, string fullPath, int delay)
        {
            RemoveFileWithDelayInternal(key, fullPath, delay, FileSystemRemoveAction);
        }
        static void RemoveFileWithDelayInternal(string fileKey, object fileData, int delay, CacheItemRemovedCallback removeAction)
        {
            string key = RemoveTaskKeyPrefix + fileKey;
            if (HttpRuntime.Cache[key] == null)
            {
                DateTime absoluteExpiration = DateTime.UtcNow.Add(new TimeSpan(0, delay, 0));
                HttpRuntime.Cache.Insert(key, fileData, null, absoluteExpiration,
                    Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, removeAction);
            }
        }
        static void FileSystemRemoveAction(string key, object value, CacheItemRemovedReason reason)
        {
            string fileFullPath = value.ToString();
            if (File.Exists(fileFullPath))
                File.Delete(fileFullPath);
        }
    }
}