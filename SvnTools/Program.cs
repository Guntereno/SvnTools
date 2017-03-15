using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
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
            string[] branches = null;
            DoThread(() =>
            {
                branches = List(kRepository + kBranchesRoot);
            });
            if (branches != null)
            {
                Console.WriteLine("Branches:");
                foreach (string branch in branches)
                {
                    Console.WriteLine(branch);
                }
                Console.WriteLine();
            }
            

            SvnPropertyValue[] properties = null;
            DoThread(() =>
            {
                properties = GetMergeInfoProperties(kRepository + kTrunk);
            });

            if (properties != null)
            {
                Console.WriteLine("MergeInfo:");
                foreach (SvnPropertyValue property in properties)
                {
                    Console.WriteLine(property.Target);
                }
                Console.WriteLine();
            }
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

        static SvnPropertyValue[] GetMergeInfoProperties(string target)
        {
            using (var client = new SvnClient())
            {
                var args = new SvnGetPropertyArgs()
                {
                    Depth = SvnDepth.Infinity
                };

                SvnTargetPropertyCollection properties;

                bool gotList = client.GetProperty(target, "svn:mergeinfo", args, out properties);
                if (gotList)
                {
                    return new List<SvnPropertyValue>(properties).ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        private static void DoThread(Action action)
        {
            Task task = new Task(action);
            task.Start();

            if (Console.CursorLeft > 0)
            {
                Console.WriteLine();
            }

            Console.Write("Working");

            // Animate some 10 dots appearing in order
            int dotsStart = Console.CursorLeft;
            const int kDotCount = 3;
            int currentDot = 0;
            while (!task.IsCompleted)
            {
                if (currentDot >= kDotCount)
                {
                    Console.SetCursorPosition(dotsStart, Console.CursorTop);
                    Console.Write("          ");
                    Console.SetCursorPosition(dotsStart, Console.CursorTop);
                    currentDot = 0;
                }

                Console.Write(".");
                ++currentDot;

                Thread.Sleep(1000);
            }

            // Clear the line and return cursor to the start
            int numChars = Console.CursorLeft;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new String(' ', numChars));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
