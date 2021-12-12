using System;
using System.IO;
using System.Collections.Generic;

using F29API.Data;

namespace F29API.Services
{
    public class BioEntityService
    {
        public BioEntityService()
        {
            Conditions = new Conditions();
        }

        public Conditions Conditions { get; private set; }

        public void Initialize(string path)
        {
            Conditions = ConditionParser.Parse(Path.Combine(path, "mondo.obo"));

            GeneParser.Parse(Path.Combine(path, "gene_disease.9606.tsv"), Conditions);
            GeneParser.Parse(Path.Combine(path, "gene_disease.noncausal.tsv"), Conditions);
            GeneParser.Parse(Path.Combine(path, "gene_disease.other.tsv"), Conditions);

            PhenotypeParser.Parse(Path.Combine(path, "disease_phenotype.all.tsv"), Conditions);
        }

        public string GetAlias(string id)
        {
            if (!id.StartsWith("MONDO", StringComparison.OrdinalIgnoreCase))
            {
                if (!Conditions.Aliases.TryGetValue(id, out id)) return null;
            }
            return id;
        }

        public List<Condition> GetChildren(string id)
        {
            if (Conditions.Mondo.TryGetValue(id, out Condition condition))
            {
                return condition.Children;
            }
            return new List<Condition>();
        }

        public IEnumerable<Condition> GetDescendants(string id)
        {
            if (Conditions.Mondo.TryGetValue(id, out Condition condition))
            {
                return condition.GetDescendants();
            }
            return new List<Condition>();
        }

        public IEnumerable<Association> GetGenes(string[] ids)
        {
            foreach (var id in ids)
            {
                var assocs = GetGenes(id);
                if (assocs != null)
                {
                    foreach (var item in GetGenes(id))
                    {
                        if (item != null)
                        {
                            yield return item;
                        }
                    }
                }
            }
        }
        public List<Association> GetGenes(string id)
        {
            if (Conditions.Mondo.TryGetValue(id, out Condition condition))
            {
                return condition.Genes;
            }
            return new List<Association>();
        }

        public IEnumerable<Association> GetPhenotypes(string[] ids)
        {
            foreach (var id in ids)
            {
                var assocs = GetPhenotypes(id);
                if (assocs != null)
                {
                    foreach (var item in GetPhenotypes(id))
                    {
                        if (item != null)
                        {
                            yield return item;
                        }
                    }
                }
            }
        }
        public List<Association> GetPhenotypes(string id)
        {
            if (Conditions.Mondo.TryGetValue(id, out Condition condition))
            {
                return condition.Phenotypes;
            }
            return new List<Association>();
        }
    }
}
