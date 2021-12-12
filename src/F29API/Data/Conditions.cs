using System;
using System.Collections.Generic;

namespace F29API.Data
{
    public class Conditions
    {
        public Conditions()
        {
            Mondo = new Dictionary<string, Condition>(StringComparer.OrdinalIgnoreCase);
            Aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<string, Condition> Mondo { get; }
        public Dictionary<string, string> Aliases { get; }

        public Condition GetCondition(string id)
        {
            Mondo.TryGetValue(id, out Condition condition);
            return condition;
        }

        public Condition GetOrAddCondition(string id, Func<Condition> newCondition)
        {
            if (!Mondo.TryGetValue(id, out Condition condition))
            {
                condition = newCondition();
                Mondo[id] = condition;
            }
            return condition;
        }
    }
}
