import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { AssignmentResponseDto } from '../../../core/models/assignmentmodels';
import { Router } from '@angular/router';
import { Assignment } from '../../../core/services/assignment';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Reportservice } from '../../../core/services/reportservice';

@Component({
  selector: 'app-assignmentlist',
  imports: [CommonModule, FormsModule],
  templateUrl: './assignmentlist.html',
  styleUrl: './assignmentlist.css',
})
export class Assignmentlist implements OnInit {
  assignments: AssignmentResponseDto[] = [];
  filteredAssignments: AssignmentResponseDto[] = [];
  searchTerm: string = '';
  isLoading: boolean = false; 
  router = inject(Router); 
  assignmentService= inject(Assignment);
  cdr = inject(ChangeDetectorRef);
  reportService = inject(Reportservice);
  ngOnInit(): void {
    this.loadAssignmentsFromApi();
  }

  loadAssignmentsFromApi() {
    this.isLoading = true;
    this.assignmentService.getAssignments().subscribe({
      next: (data: AssignmentResponseDto[]) => {
       
        if (!data || data.length === 0) {
          this.assignments = [];
          this.filteredAssignments = [];
          this.isLoading = false;
          this.cdr.detectChanges(); 
          return;
        }

        try {
          this.assignments = data.map(dto => ({
            id: dto.id,
            templateId: dto.templateId,
            templateTitle: dto.templateTitle || 'General Template',
            evaluatorId: dto.evaluatorId,
            evaluatorName: dto.evaluatorName || 'Unknown Evaluator',
            evaluateeId: dto.evaluateeId,
            evaluateeName: dto.evaluateeName || 'Unknown Evaluatee',
            status: this.getStatusText(dto.status),
            progress: this.calculateProgress(dto.status),
            dueDate: dto.dueDate,
            createdAt: dto.createdAt,
          }));
          this.filteredAssignments = [...this.assignments];
          this.cdr.detectChanges(); 

        } catch (error) {
          console.error(error);
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching assignments:', err.message || err);
        this.isLoading = false;
      }
    });
  }

  private getStatusText(status: any): string {
    if (status === undefined || status === null) return 'Pending';
    const s = status.toString().toLowerCase().replace(/\s+/g, '');
    switch (s) {
      case 'completed': case '2': return 'Completed';
      case 'inprogress': case '1': return 'In Progress';
      case 'pending': case '0': return 'Pending';
      default: return status.toString();
    }
  }

  private calculateProgress(status: any): number {
    if (status === undefined || status === null) return 0;
    const statusStr = status.toString().toLowerCase().replace(/\s+/g, '');
    switch (statusStr) {
      case 'completed': case '2': return 100;
      case 'inprogress': case '1': return 50;
      case 'pending': case '0': return 0;
      default: return 0;
    }
  }

  onSearch() {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredAssignments = [...this.assignments];
      this.cdr.detectChanges(); 
      return;
    }
    this.filteredAssignments = this.assignments.filter(a => 
      a.templateTitle?.toLowerCase().includes(term) ||
      a.evaluatorName?.toLowerCase().includes(term) ||
      a.evaluateeName?.toLowerCase().includes(term)
    );
    this.cdr.detectChanges(); 
  }
 exportPDF(assignmentId: number) {
    this.reportService.getAssignmentPdf(assignmentId).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `assignment_${assignmentId}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Error downloading PDF:', err.message || err);
      }
    });
  }

 

  onCreateAssignment() {this.router.navigate(['/create-assignment']);}
  onDownloadPdf(id: number) { this.exportPDF(id);} 
  onEditAssignment(id: number) { 
   this.router.navigate(['/create-assignment'], { queryParams: { id } });
   }
}

