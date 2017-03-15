using SharpSvn;
using System;
using System.Collections.ObjectModel;

namespace SvnTools
{
    public static class Svn
    {
        public static bool TargetExists(string target)
        {
            bool result;
            using (var client = new SvnClient())
            {
                Collection<SvnListEventArgs> list;

                var svnListArgs = new SvnListArgs()
                {
                    Depth = SvnDepth.Empty
                };

                try
                {
                    result = client.GetList(target, svnListArgs, out list);
                }
                catch (SvnFileSystemException)
                {
                    result = false;
                }
            }

            return result;
        }

        public static void SetProperty(string target, string key, string value)
        {
            using (var client = new SvnClient())
            {
                bool propertySet = client.SetProperty(target, key, value);
                if (!propertySet)
                {
                    throw new Exception("Failed to set property '" + key + "' on target '" + target + "'!");
                }
            }
        }

        public static void DeleteProperty(string target, string key)
        {
            using (var client = new SvnClient())
            {
                bool propertyDeleted = client.DeleteProperty(target, key);
                if (!propertyDeleted)
                {
                    throw new Exception("Failed to delete property '" + key + "' on target '" + target + "'!");
                }
            }
        }
    }
}
