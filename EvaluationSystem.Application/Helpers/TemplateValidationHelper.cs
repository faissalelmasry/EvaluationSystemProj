using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Domain.Exceptions;

namespace EvaluationSystem.Application.Helpers
{
    public static class TemplateValidationHelper
    {
        public static void ValidateNewSections(List<AddTemplateSectionDto> sections)
        {
            var duplicateTitles = sections.GroupBy(s => s.Title).Where(g => g.Count() > 1);
            if (duplicateTitles.Any())
                throw new BadRequestException($"Duplicate section title in request: {duplicateTitles.First().Key}");

            var duplicateOrders = sections.GroupBy(s => s.OrderNo).Where(g => g.Count() > 1);
            if (duplicateOrders.Any())
                throw new BadRequestException($"Duplicate section order in request: {duplicateOrders.First().Key}");
        }

        public static void ValidateNewCriteria(List<AddTemplateSectionDto> sections)
        {
            foreach (var section in sections)
            {
                var duplicateTitles = section.Criteria.GroupBy(c => c.Title).Where(g => g.Count() > 1);
                if (duplicateTitles.Any())
                    throw new BadRequestException($"Duplicate criterion title in section '{section.Title}'");

                var duplicateOrders = section.Criteria.GroupBy(c => c.OrderNo).Where(g => g.Count() > 1);
                if (duplicateOrders.Any())
                    throw new BadRequestException($"Duplicate criterion order in section '{section.Title}'");
            }
        }
    }
}
