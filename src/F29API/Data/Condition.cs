using System;
using System.Collections.Generic;

namespace F29API.Data
{
    public class Condition : Entity
    {
        public Condition(string id, string label = null) : base(id, label)
        {
            Ancestors = new List<Condition>();
            Children = new List<Condition>();
            Genes = new List<Association>();
            Phenotypes = new List<Association>();
        }

        public List<Condition> Ancestors { get; }
        public List<Condition> Children { get; }

        public List<Association> Genes { get; }
        public List<Association> Phenotypes { get; }

        public IEnumerable<Condition> GetDescendants()
        {
            foreach (var child in Children)
            {
                yield return child;
            }
            foreach (var child in Children)
            {
                foreach (var descendant in child.GetDescendants())
                {
                    yield return descendant;
                }
            }
        }
    }
}
