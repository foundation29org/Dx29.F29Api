using System;
using System.IO;
using System.Collections.Generic;

using F29API.Data;

namespace F29API
{
    static public class ConditionParser
    {
        static public Conditions Parse(string filename)
        {
            var conditions = new Conditions();

            using (var reader = new StreamReader(filename))
            {
                string line = reader.GoTo("[Term]");
                while (line != null)
                {
                    ProcessTerm(conditions, reader);
                    line = reader.GoTo("[Term]");
                }
            }

            return conditions;
        }

        static private void ProcessTerm(Conditions conditions, StreamReader reader)
        {
            string id = null;
            string name = null;
            var parents = new List<string>();
            var aliases = new List<string>();

            string line = reader.ReadLine();
            while (!String.IsNullOrEmpty(line))
            {
                (string prop, string vals) = ProcessLine(conditions, line);
                switch (prop)
                {
                    case "id":
                        id = vals;
                        break;
                    case "name":
                        name = vals;
                        break;
                    case "is_a":
                        parents.Add(vals.Split(' ')[0].Trim());
                        break;
                    case "xref":
                        aliases.Add(vals.Split(' ')[0].Trim());
                        break;
                }
                line = reader.ReadLine();
            }
            var condition = conditions.GetOrAddCondition(id, () => new Condition(id));
            condition.Label = name;

            foreach (var parent in parents)
            {
                var pcond = conditions.GetOrAddCondition(parent, () => new Condition(parent));
                condition.Ancestors.Add(pcond);
                pcond.Children.Add(condition);
            }

            foreach (var alias in aliases)
            {
                conditions.Aliases[alias] = id;
            }
        }

        static private (string, string) ProcessLine(Conditions conditions, string line)
        {
            int index = line.IndexOf(':');
            string prop = line.Substring(0, index);
            string vals = line.Substring(index + 1).TrimStart();
            return (prop, vals);
        }
    }
}
