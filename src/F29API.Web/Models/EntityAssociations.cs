using System;
using System.Linq;
using System.Collections.Generic;

using F29API.Data;

namespace F29API.Web.Models
{
    public class EntityAssociations
    {
        static public readonly EntityAssociations Empty = new EntityAssociations();

        public EntityAssociations()
        {
            Associations = new List<EntityAssociation>();
        }

        static public EntityAssociations FromAssociations(IEnumerable<Association> associations, bool extended)
        {
            var model = new EntityAssociations();

            var subjects = new HashSet<string>();
            var objects = new HashSet<string>();
            var relations = new HashSet<string>();

            foreach (var assoc in associations)
            {
                var assocModel = new EntityAssociation
                {
                    Object = EntityModel.FromEntity(assoc.Object)
                };
                if (extended)
                {
                    assocModel.Subject = EntityModel.FromEntity(assoc.Subject);
                    assocModel.Relation = EntityModel.FromEntity(assoc.Relation);
                }
                model.Associations.Add(assocModel);
                subjects.Add(assoc.Subject.Id);
                objects.Add(assoc.Object.Id);
                relations.Add(assoc.Relation.Id);
            }

            if (extended)
            {
                model.Subjects = subjects.ToArray();
                model.Objects = objects.ToArray();
                model.Relations = relations.ToArray();
            }

            return model;
        }

        public List<EntityAssociation> Associations { get; private set; }
        public int NumFound { get; set; }
        public string[] Subjects { get; private set; }
        public string[] Objects { get; private set; }
        public string[] Relations { get; private set; }
    }

    public class EntityAssociation
    {
        public EntityModel Subject { get; set; }
        public EntityModel Object { get; set; }
        public EntityModel Relation { get; set; }
    }

    public class EntityModel
    {
        public string Id { get; set; }
        public string Label { get; set; }

        static public EntityModel FromEntity(Entity entity)
        {
            return new EntityModel
            {
                Id = entity.Id,
                Label = entity.Label
            };
        }
    }
}
