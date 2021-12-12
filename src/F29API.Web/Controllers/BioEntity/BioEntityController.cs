using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using F29API.Data;
using F29API.Services;
using F29API.Web.Models;

namespace F29API.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class BioEntityController : ControllerBase
    {
        private BioEntityService _bioEntityService = null;

        public BioEntityController(BioEntityService bioEntityService)
        {
            _bioEntityService = bioEntityService;
        }

        [HttpGet("[action]/{id}")]
        public IActionResult Alias(string id)
        {
            var alias = _bioEntityService.GetAlias(id);
            if (alias != null)
            {
                return Ok(alias);
            }
            return NotFound();
        }

        [HttpGet("[action]/{id}")]
        public IActionResult Children(string id)
        {
            var children = _bioEntityService.GetChildren(id);
            return Ok(children.ToArray());
        }

        [HttpPost("Disease/Genes")]
        public IActionResult DiseaseGenes([FromBody] string[] ids, int rows = 100, bool extended = false)
        {
            var results = new Dictionary<string, EntityAssociations>();
            foreach (var id in ids)
            {
                results[id] = GetGeneAssociations(id, rows, extended);
            }
            return Ok(results);
        }

        [HttpPost("Disease/Phenotypes")]
        public IActionResult DiseasePhenotypes([FromBody] string[] ids, int rows = 100, bool extended = false)
        {
            var results = new Dictionary<string, EntityAssociations>();
            foreach (var id in ids)
            {
                results[id] = GetPhenotypeAssociations(id, rows, extended);
            }
            return Ok(results);
        }

        private EntityAssociations GetGeneAssociations(string id, int rows, bool extended)
        {
            id = _bioEntityService.GetAlias(id);
            if (id != null)
            {
                var items = _bioEntityService.GetGenes(id);
                var children = _bioEntityService.GetDescendants(id);
                var childrenItems = _bioEntityService.GetGenes(children.Select(r => r.Id).ToArray());
                var allItems = items.Concat(childrenItems).ToArray();
                var assocs = EntityAssociations.FromAssociations(allItems.Take(rows), extended);
                assocs.NumFound = allItems.Length;
                return assocs;
            }
            return EntityAssociations.Empty;
        }

        private EntityAssociations GetPhenotypeAssociations(string id, int rows, bool extended)
        {
            id = _bioEntityService.GetAlias(id);
            if (id != null)
            {
                var items = _bioEntityService.GetPhenotypes(id);
                var children = _bioEntityService.GetDescendants(id);
                var childrenItems = _bioEntityService.GetPhenotypes(children.Select(r => r.Id).ToArray());
                var allItems = items.Concat(childrenItems).ToArray();
                var assocs = EntityAssociations.FromAssociations(allItems.Take(rows), extended);
                assocs.NumFound = allItems.Length;
                return assocs;
            }
            return EntityAssociations.Empty;
        }
    }
}
