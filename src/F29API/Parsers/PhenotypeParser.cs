using System;
using System.IO;

using F29API.Data;

namespace F29API
{
    static public class PhenotypeParser
    {
        static public void Parse(string filename, Conditions conditions)
        {
            using (var reader = new StreamReader(filename))
            {
                reader.ReadLine();
                string line = reader.ReadLine();
                while (line != null)
                {
                    var record = Record.Deserialize(line);
                    var id = record.Subject;
                    var condition = conditions.GetOrAddCondition(id, () => new Condition(id, record.SubjectLabel));
                    var association = new Association
                    {
                        Subject = condition,
                        Object = new Phenotype(record.Object, record.ObjectLabel),
                        Relation = new Relation(record.Relation, record.RelationLabel)
                    };
                    condition.Phenotypes.Add(association);
                    line = reader.ReadLine();
                }
            }
        }
    }
}
