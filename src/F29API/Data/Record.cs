using System;
using System.Linq;
using System.Collections.Generic;

namespace F29API.Data
{
    public class Record
    {
        public string Subject { get; set; }
        public string SubjectLabel { get; set; }
        public string SubjectTaxon { get; set; }
        public string SubjectTaxonLabel { get; set; }
        public string Object { get; set; }
        public string ObjectLabel { get; set; }
        public string Relation { get; set; }
        public string RelationLabel { get; set; }
        public string Evidence { get; set; }
        public string EvidenceLabel { get; set; }
        public string Source { get; set; }
        public string IsDefinedBy { get; set; }
        public string Qualifier { get; set; }

        #region Serialize
        static public string Header()
        {
            return String.Join("\t", GetPropertyNames().Select(r => r?.ToString().Replace("\t", "")));
        }

        public string Serialize()
        {
            return String.Join("\t", GetPropertyValues().Select(r => r?.ToString().Replace("\t", "")));
        }

        private IEnumerable<object> GetPropertyValues()
        {
            yield return Subject;
            yield return SubjectLabel;
            yield return SubjectTaxon;
            yield return SubjectTaxonLabel;
            yield return Object;
            yield return ObjectLabel;
            yield return Relation;
            yield return RelationLabel;
            yield return Evidence;
            yield return EvidenceLabel;
            yield return Source;
            yield return IsDefinedBy;
            yield return Qualifier;
        }

        static private IEnumerable<object> GetPropertyNames()
        {
            yield return "Subject";
            yield return "SubjectLabel";
            yield return "SubjectTaxon";
            yield return "SubjectTaxonLabel";
            yield return "Object";
            yield return "ObjectLabel";
            yield return "Relation";
            yield return "RelationLabel";
            yield return "Evidence";
            yield return "EvidenceLabel";
            yield return "Source";
            yield return "IsDefinedBy";
            yield return "Qualifier";
        }

        public override string ToString()
        {
            return Serialize();
        }
        #endregion

        static public Record Deserialize(string line)
        {
            var fields = line.Split('\t');
            return new Record()
            {
                Subject = fields[0],
                SubjectLabel = fields[1],
                SubjectTaxon = fields[2],
                SubjectTaxonLabel = fields[3],
                Object = fields[4],
                ObjectLabel = fields[5],
                Relation = fields[6],
                RelationLabel = fields[7],
                Evidence = fields[8],
                EvidenceLabel = fields[9],
                Source = fields[10],
                IsDefinedBy = fields[11],
                Qualifier = fields[12],
            };
        }
    }
}
