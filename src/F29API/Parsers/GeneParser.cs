using System;
using System.IO;

using F29API.Data;

namespace F29API
{
    static public class GeneParser
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
                    var id = record.Object;
                    var condition = conditions.GetOrAddCondition(id, () => new Condition(id, record.ObjectLabel));
                    var association = new Association
                    {
                        Subject = condition,
                        Object = new Gene(record.Subject, record.SubjectLabel),
                        Relation = new Relation(record.Relation, record.RelationLabel)
                    };
                    condition.Genes.Add(association);
                    line = reader.ReadLine();
                }
            }
        }
    }
}
