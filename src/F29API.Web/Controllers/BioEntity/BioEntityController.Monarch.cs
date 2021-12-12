using System;

using Microsoft.AspNetCore.Mvc;

using F29API.Web.Models;

namespace F29API.Web.Controllers
{
    partial class BioEntityController
    {
        [HttpGet("Disease/{id}/Genes")]
        public IActionResult DiseaseGenes(string id, int rows = 100, bool extended = false)
        {
            return Ok(GetGeneAssociations(id, rows, extended));
        }

        [HttpGet("Disease/{id}/Phenotypes")]
        public IActionResult DiseasePhenotypes(string id, int rows = 100, bool extended = false)
        {
            return Ok(GetPhenotypeAssociations(id, rows, extended));
        }
    }
}
