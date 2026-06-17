import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TemplateService } from '../../../core/services/template.service';
import { EvaluationTemplateList } from '../../../core/models/evaluation-template-list';
import { Router, RouterLink } from "@angular/router";

@Component({
  selector: 'app-template-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './template-list.component.html',
  styleUrls: ['./template-list.component.scss'],
})
export class TemplateListComponent implements OnInit {

  public templates = signal<EvaluationTemplateList[]>([]);
  public pageNumber = signal<number>(1);
  public isLoading = signal<boolean>(false);
  public errorMessage = signal<string>('');
  public pageSize = signal<number>(9);
  public searchTerm = signal('');

  constructor(private templateService: TemplateService,private router:Router) {}

  ngOnInit(): void {
    this.loadTemplates();
  }

  loadTemplates(): void {
  this.isLoading.set(true);
  this.errorMessage.set('');

  this.templateService
    .GetTemplates(
      this.pageNumber(),
      this.pageSize(),
      this.searchTerm()
    )
    .subscribe({
      next: (res) => {
        this.templates.set(res);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(
          err?.error?.message ?? 'Failed to load templates'
        );
        this.isLoading.set(false);
      }
    });

  }
  nextPage() {
  const nextPage = this.pageNumber() + 1;

  this.templateService.GetTemplates(nextPage, this.pageSize(), this.searchTerm()).subscribe({
    next: (res) => {

      if (res.length > 0) {
        this.pageNumber.set(nextPage);
        this.templates.set(res);
      }

    }
  });
}

previousPage() {
  if (this.pageNumber() > 1) {
    this.pageNumber.update(page => page - 1);
    this.loadTemplates();
  }
}
onSearch(value: string) {
  this.searchTerm.set(value);
  this.pageNumber.set(1);
  this.loadTemplates();
}
DeleteTemplate(id: number): void {
  // 1. Show the confirmation dialog
  const isConfirmed = confirm("Are you sure you want to delete this template?");
  
  // 2. If the user clicks "Cancel", stop the function completely
  if (!isConfirmed) {
    return; 
  }

  // 3. If they clicked "OK", proceed with your existing logic
  this.templateService.GetTemplateById(id).subscribe({
    next: (res) => {
      const sections = res.sections;

      if (!sections || sections.length === 0) {
        this.templateService.DeleteTemplate(id).subscribe({
          next: () => {
            if (this.templates()) {
              this.templates.set(this.templates().filter(t => t.id !== id));
            }
            this.router.navigateByUrl('/templates');
          },
          error: (err) => {
            alert("Server error: Could not complete the deletion request.");
          }
        });
      } else {
        alert("Can't delete this template.\nIts sections must be deleted first.");
      }
    },
    error: (err) => {
      alert("Failed to fetch template details for validation.");
    }
  });
}
}