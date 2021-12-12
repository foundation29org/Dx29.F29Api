using System;

namespace F29API.Data
{
    public class Association
    {
        public Entity Subject { get; set; }
        public Entity Object { get; set; }

        public Relation Relation { get; set; }
    }
}
