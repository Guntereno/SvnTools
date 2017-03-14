using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnTools
{
    class Program
    {
        const string kRepository = "https://svn.mediatonic.co.uk:8443/svn/060_project_fortune/";
        const string kBranchesRoot = "branches/";
        const string kTrunk = "trunk/";


        static void Main(string[] args)
        {
            try
            {
                string[] branches = Svn.List(kRepository + kBranchesRoot);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
