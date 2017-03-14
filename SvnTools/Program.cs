using SharpSvn;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SvnTools
{
    class Program
    {
        const string kRepository = "https://svn.mediatonic.co.uk:8443/svn/060_project_fortune/";
        const string kBranchesRoot = "branches/";
        const string kTrunk = "trunk/";


        static void Main(string[] args)
        {
            string[] list = List(kRepository + kBranchesRoot);
        }

        static string[] List(string target)
        {
            using (var client = new SvnClient())
            {
                Collection<SvnListEventArgs> list;

                bool gotList = client.GetList(target, out list);
                if (gotList)
                {
                    List<string> files = new List<string>();
                    foreach (SvnListEventArgs item in list)
                    {
                        files.Add(item.Path);
                    }

                    return files.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
