using System;
using System.Collections.Generic;

namespace F29API.Data
{
    public class Entity
    {
        public Entity(string id, string label = null)
        {
            Id = id;
            Label = label;
        }

        public string Id { get; set; }
        public string Label { get; set; }

        public override string ToString()
        {
            return $"{Id}\t{Label}";
        }
    }

    public class Gene : Entity
    {
        public Gene(string id, string label = null) : base(id, label)
        {
        }
    }

    public class Phenotype : Entity
    {
        public Phenotype(string id, string label = null) : base(id, label)
        {
        }
    }

    public class Relation : Entity
    {
        public Relation(string id, string label = null) : base(id, label)
        {
        }
    }
}
