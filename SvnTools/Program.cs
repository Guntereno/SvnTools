using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SvnTools
{
    class Program
    {
        private const string kRepository = "https://svn.mediatonic.co.uk:8443/svn/060_project_fortune/";
        private const string kBranchesRoot = "branches/";
        private const string kTrunk = "trunk/";
        private const string kSvnMergeInfo = "svn:mergeinfo";

        static void Main(string[] args)
        {
            try
            {
                string target = (args.Length > 0) ? args[0] : System.IO.Directory.GetCurrentDirectory();

                CleanMergeInfo(target);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        private static void CleanMergeInfo(string target)
        {
            SvnPropertyValue[] properties = null;
            Threading.DoThread(() =>
            {
                properties = GetMergeInfoProperties(target);
            });

            if (properties != null)
            {
                foreach (SvnPropertyValue property in properties)
                {
                    Console.WriteLine(property.Target);

                    string before = property.StringValue;

                    var mergeInfo = new MergeInfo(before);

                    StripNonExistantSources(mergeInfo);

                    // Warning: Project specific code. We need to strip merge info before
                    // this specific revision. This should really be an argument.
                    const int kLastValidRevision = 10616;
                    StripCheckinsBeforeRevision(mergeInfo, kLastValidRevision);

                    if (mergeInfo.Entries.Count > 0)
                    {
                        string after = mergeInfo.ToString();
                        Console.WriteLine(after);
                        Svn.SetProperty(property.Target.TargetName, kSvnMergeInfo, after);
                    }
                    else
                    {
                        Console.WriteLine("DELETED");
                        Svn.DeleteProperty(property.Target.TargetName, kSvnMergeInfo);
                    }

                    Console.WriteLine();
                }
            }
            else
            {
                throw new Exception("Unable to find mergeinfo properties for target '" + target + "'");
            }
        }

        private static SvnPropertyValue[] GetMergeInfoProperties(string target)
        {
            using (var client = new SvnClient())
            {
                var args = new SvnGetPropertyArgs()
                {
                    Depth = SvnDepth.Infinity
                };

                SvnTargetPropertyCollection properties;

                bool gotList = client.GetProperty(target, kSvnMergeInfo, args, out properties);
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

        private static void StripCheckinsBeforeRevision(MergeInfo mergeInfo, int revision)
        {
            var newEntries = new List<MergeInfoEntry>();
            foreach (MergeInfoEntry entry in mergeInfo.Entries)
            {
                var newRanges = new List<MergeInfoRange>();
                foreach (MergeInfoRange range in entry.Ranges)
                {
                    if (range.From >= revision)
                    {
                        newRanges.Add(range);
                    }
                    else if (range.To >= revision)
                    {
                        // Range spans boundary, split it
                        MergeInfoRange newRange = new MergeInfoRange()
                        {
                            From = revision,
                            To = range.To
                        };
                        newRanges.Add(newRange);
                    }
                }

                if (newRanges.Count > 0)
                {
                    newEntries.Add(new MergeInfoEntry()
                    {
                        Source = entry.Source,
                        Ranges = newRanges
                    });
                }
            }

            mergeInfo.Entries = newEntries;
        }

        private static void StripNonExistantSources(MergeInfo mergeInfo)
        {
            List<MergeInfoEntry> newMergeInfos = new List<MergeInfoEntry>(mergeInfo.Entries.Count);
            foreach (MergeInfoEntry entry in mergeInfo.Entries)
            {
                string repoPath = kRepository + entry.Source;
                if (Svn.TargetExists(repoPath))
                {
                    newMergeInfos.Add(entry);
                }
            }
            mergeInfo.Entries = newMergeInfos;
        }
    }
}
