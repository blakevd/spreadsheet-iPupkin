using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<string>> dependees, dependents;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependees = new Dictionary<string, HashSet<string>>();
            dependents = new Dictionary<string, HashSet<string>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                int s = 0;
                foreach (KeyValuePair<string, HashSet<string>> entry in dependents)
                {
                    foreach (string str in entry.Value)
                    {
                        s++;
                    }
                }

                return s;
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                return GetDependees(s).Count();
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.TryGetValue(s, out HashSet<string> result)) // if it found dependent list for s
                return result.Count > 0;

            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (dependees.TryGetValue(s, out HashSet<string> result)) // if it found dependees list for s
                return result.Count > 0;

            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            HashSet<string> empty = new HashSet<string>();
            if (HasDependents(s))
            {
                dependents.TryGetValue(s, out HashSet<string> result);
                return result;
            }

            return empty;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            HashSet<string> empty = new HashSet<string>();
            if (HasDependees(s))
            {
                dependees.TryGetValue(s, out HashSet<string> result);
                return result;
            }

            return empty;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            // add dependent, s
            if (!dependents.ContainsKey(s)) // dependents doesn't have dependent yet  => create new one
            {
                // add the new dependent and set
                HashSet<string> depSet = new HashSet<string>();
                depSet.Add(t);
                dependents.Add(s, depSet);
            }
            else // already has set of dependents
            {
                if (dependents.TryGetValue(s, out HashSet<string> currDepSet) && !currDepSet.Contains(t))
                {
                    currDepSet.Add(t);
                }
            }
            // add the dependee, t
            if (!dependees.ContainsKey(t)) // doesn't have a dependee yet => create new one
            {
                HashSet<string> deeSet = new HashSet<string>();
                deeSet.Add(s);
                dependees.Add(t, deeSet);
            }
            else // already has a set of dependees
            {
                if (dependees.TryGetValue(t, out HashSet<string> currDeeSet) && !currDeeSet.Contains(s))
                {
                    currDeeSet.Add(s);
                }
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (dependents.TryGetValue(s, out HashSet<string> depSet)) // check if dep exists
            {
                if (depSet.Count == 1) // removes the whole thing it its 1 pair
                    dependents.Remove(s);
                else
                    if (depSet.Contains(t)) // removes just the value from hashSet (it has other stuff in it we need to keep)
                        depSet.Remove(t);
                if (dependees.TryGetValue(t, out HashSet<string> deeSet)) // check if dee exists
                {
                    if (deeSet.Count == 1) // removes whole dependee if its only 1 pair
                        dependees.Remove(t);
                    else
                        if (deeSet.Contains(s)) // otherwise just remove it from hashSet
                            deeSet.Remove(s);
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            HashSet<string> copyOfDep = new HashSet<string>(GetDependents(s));

            foreach (string e in copyOfDep)
            {
                RemoveDependency(s, e);
            }

            foreach (string t in newDependents)
            {
                AddDependency(s, t);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            HashSet<string> copyOfDees = new HashSet<string>(GetDependees(s));

            foreach(string e in copyOfDees)
            {
                RemoveDependency(e, s);
            }

            foreach(string t in newDependees)
            {
                AddDependency(t, s);
            }
        }

    }
}