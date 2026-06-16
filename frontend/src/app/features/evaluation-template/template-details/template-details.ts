import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TemplateService } from '../../../core/services/template.service';
import { GetTemplateDto } from '../../../core/models/get-template-dto';
import { SectionService } from '../../../core/services/section-service';
import { CriteriaService } from '../../../core/services/criteria-service';

@Component({
  selector: 'app-template-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './template-details.html',
  styleUrl: './template-details.scss',
})
export class TemplateDetails implements OnInit {

  template = signal<GetTemplateDto | null>(null);
  isLoading = signal<boolean>(false);

  constructor(
    private route: ActivatedRoute,
    private templateService: TemplateService,
    private sectionService:SectionService,
    private criteriaService:CriteriaService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadTemplate(id);
  }

  loadTemplate(id: number) {
    this.isLoading.set(true);
    this.templateService.GetTemplateById(id).subscribe({
      next: (res) => {
        this.template.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  DeleteTemplate(id: any): void {
    const isConfirmed = confirm("Are you sure you want to delete this template?");
    
    if (!isConfirmed) {
      return; 
    }

    const currentTemplate = this.template();
    // Safe check for sections array handling PascalCase or lowercase from API
    const sections = currentTemplate?.sections || (currentTemplate as any)?.Sections;

    if (sections && sections.length === 0) {
      this.templateService.DeleteTemplate(id).subscribe({
        next: () => {
          this.router.navigateByUrl('/templates');
        },
        error: (err) => {
          alert("Server error: Could not complete the deletion request.");
        }
      });
    } else {
      alert("Can't delete this template.\nIts sections must be deleted first.");
    }
  }

  // 🛠️ NEW: Delete Section Method
  DeleteSection(sectionId: number): void {
    const isConfirmed = confirm("Are you sure you want to delete this section and all its criteria?");
    if (!isConfirmed) return;

    this.sectionService.DeleteSection(sectionId).subscribe({
      next: () => {
        const currentTemplate = this.template();
        if (currentTemplate) {
          // Filter out the deleted section from the signal state
          const updatedSections = currentTemplate.sections.filter(s => s.id !== sectionId);
          
          this.template.set({
            ...currentTemplate,
            sections: updatedSections
          });
        }
      },
      error: (err) => {
        alert("Server error: Failed to delete the section.");
      }
    });
  }

  // 🛠️ NEW: Delete Criteria Method
  DeleteCriteria(sectionId: number, criteriaId: number): void {
    const isConfirmed = confirm("Are you sure you want to delete this criteria?");
    if (!isConfirmed) return;

    this.criteriaService.DeleteCriterion(criteriaId).subscribe({
      next: () => {
        const currentTemplate = this.template();
        if (currentTemplate) {
          // Map through sections to find the specific one, then filter out the criteria
          const updatedSections = currentTemplate.sections.map(section => {
            if (section.id === sectionId) {
              return {
                ...section,
                criteria: section.criteria.filter(c => c.id !== criteriaId)
              };
            }
            return section;
          });

          this.template.set({
            ...currentTemplate,
            sections: updatedSections
          });
        }
      },
      error: (err) => {
        alert("Server error: Failed to delete the criteria.");
      }
    });
  }
}