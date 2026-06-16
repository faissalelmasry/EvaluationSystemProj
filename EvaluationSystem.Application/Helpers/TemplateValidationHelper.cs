using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Domain.Exceptions;

namespace EvaluationSystem.Application.Helpers
{
    public static class TemplateValidationHelper
    {
        public static void ValidateCriteria(
            List<UpdateSectionDto> incomingSections,
            List<CriterionLookupDto> existingCriteria)
        {
            foreach (var sectionDto in incomingSections)
            {
                // 1. Check duplicates WITHIN this section's criteria in the request
                var duplicateTitles = sectionDto.Criteria
                    .GroupBy(c => c.Title.Trim().ToLower())
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicateTitles.Any())
                    throw new BadRequestException(
                        $"Duplicate criterion title in section '{sectionDto.Title}': '{duplicateTitles.First()}'");

                var duplicateOrders = sectionDto.Criteria
                    .GroupBy(c => c.OrderNo)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicateOrders.Any())
                    throw new BadRequestException(
                        $"Duplicate criterion order in section '{sectionDto.Title}': {duplicateOrders.First()}");

                // 2. Check against EXISTING criteria in DB (scoped to this section)
                var relevantExisting = existingCriteria
                    .Where(c => c.SectionId == (sectionDto.Id ?? 0))
                    .ToList();

                foreach (var criterionDto in sectionDto.Criteria)
                {
                    var conflict = relevantExisting.FirstOrDefault(c =>
                        c.Id != (criterionDto.Id ?? 0) &&
                        (c.Title.Trim().ToLower() == criterionDto.Title.Trim().ToLower() ||
                         c.OrderNo == criterionDto.OrderNo));

                    if (conflict != null)
                    {
                        if (conflict.Title.Trim().ToLower() == criterionDto.Title.Trim().ToLower())
                            throw new BadRequestException(
                                $"Criterion title '{criterionDto.Title}' already exists in section '{sectionDto.Title}'");

                        throw new BadRequestException(
                            $"Criterion order {criterionDto.OrderNo} already exists in section '{sectionDto.Title}'");
                    }
                }
            }
        }
        public static void ValidateSections(
            List<UpdateSectionDto> incomingSections,
            List<SectionLookupDto> existingSections)
        {
            // 1. Check duplicates WITHIN the incoming request itself
            var duplicateTitles = incomingSections
                .GroupBy(s => s.Title.Trim().ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (duplicateTitles.Any())
                throw new BadRequestException($"Duplicate section title in request: '{duplicateTitles.First()}'");

            var duplicateOrders = incomingSections
                .GroupBy(s => s.OrderNo)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (duplicateOrders.Any())
                throw new BadRequestException($"Duplicate section order in request: {duplicateOrders.First()}");

            // 2. Check incoming sections against EXISTING ones in DB
            foreach (var sectionDto in incomingSections)
            {
                var conflict = existingSections.FirstOrDefault(s =>
                    s.Id != (sectionDto.Id ?? 0) &&  
                    (s.Title.Trim().ToLower() == sectionDto.Title.Trim().ToLower() ||
                     s.OrderNo == sectionDto.OrderNo));

                if (conflict != null)
                {
                    if (conflict.Title.Trim().ToLower() == sectionDto.Title.Trim().ToLower())
                        throw new BadRequestException($"Section title '{sectionDto.Title}' already exists");

                    throw new BadRequestException($"Section order {sectionDto.OrderNo} already exists");
                }
            }
        }
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
