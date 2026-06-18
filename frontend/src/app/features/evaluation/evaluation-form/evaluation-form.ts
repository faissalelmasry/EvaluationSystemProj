import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';

import { TemplateService } from '../../../core/services/template.service';
import { EvaluationService } from '../../../core/services/evaluation';

@Component({
  selector: 'app-evaluation-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './evaluation-form.html',
  styleUrls: ['./evaluation-form.scss'],
})
export class EvaluationFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private evaluationService = inject(EvaluationService);
  private templateService = inject(TemplateService);
  private router = inject(Router);
  evaluationForm: FormGroup = this.fb.group({
    responses: this.fb.array([]),
  });

  assignmentId!: number;
  templateId!: number;
  templateData = signal<any>(null);

  get responsesArray(): FormArray {
    return this.evaluationForm.get('responses') as FormArray;
  }

  ngOnInit(): void {
    this.assignmentId = Number(this.route.snapshot.paramMap.get('assignmentId'));
    
    const urlTemplateId = this.route.snapshot.queryParamMap.get('templateId');
    
    if (urlTemplateId) {
      this.templateId = Number(urlTemplateId);
      this.loadTemplate();
    } else {
      console.error('Missing Template ID in URL! The Dashboard routing must pass templateId.');
      alert('Missing Template ID! Please go back to your dashboard and try clicking Start again.');
    }
  }

  private loadTemplate(): void {
    this.templateService.GetTemplateById(this.templateId).subscribe({
      next: (data) => {
        
        this.templateData.set(data);
        console.log('📦 Fetched Template Data:', data);
        this.buildFormArray(data);
      },
      error: () => alert(`Could not find Template ID ${this.templateId} in the database.`),
    });
  }
  private buildFormArray(template: any): void {
    template?.sections?.forEach((section: any) => {
      section.criteria?.forEach((criterion: any) => {
        
        const qType = criterion.questionType?.toString().toLowerCase() || 'default';

        const isText = qType === '3' || qType === 'text';
        const isChoice = qType === '1' || qType === 'singlechoice';
        const isBool = qType === '5' || qType === 'boolean';
        const isRatingOrDefault = !isText && !isChoice && !isBool;

        this.responsesArray.push(
          this.fb.group({
            criterionId: [criterion.id],
            questionType: [criterion.questionType],
            maxScore: [criterion.maxScore || 5],
            
            // Dynamically apply Validators.required ONLY to the correct input type
            score: [null, isRatingOrDefault ? [Validators.required, Validators.min(0), Validators.max(criterion.maxScore || 5)] : []],
            selectedOption: ['', isChoice || isBool ? Validators.required : []],
            textAnswer: ['', isText ? Validators.required : []],
          })
        );
      });
    });
  }

  getQuestionGroup(criterionId: number): FormGroup {
    const group = this.responsesArray.controls.find(
      (control) => control.value.criterionId === criterionId
    ) as FormGroup;

    return group ?? this.fb.group({
      criterionId: [criterionId],
      questionType: [null],
      maxScore: [5],
      score: [null],
      selectedOption: [''],
      textAnswer: [''],
    });
  }
onSubmit(): void {
    if (this.evaluationForm.invalid) return;

    const responses = this.evaluationForm.value.responses.map((res: any) => {
      let score = Number(res.score) || 0;
      const qType = String(res.questionType).toLowerCase();

      if (qType === '5' || qType === 'boolean') {
        const isYes = String(res.selectedOption).toLowerCase() === 'yes';
        score = isYes ? Number(res.maxScore) : 0;
      }

      const responseObj: any = { 
        criterionId: res.criterionId, 
        score: score 
      };
      
      if (res.textAnswer?.trim()) {
        responseObj.textAnswer = res.textAnswer;
      }
      
      if (res.selectedOption?.trim()) {
        responseObj.selectedOption = res.selectedOption;
      }

      return responseObj;
    });

    this.evaluationService.submitEvaluation(this.assignmentId, { responses }).subscribe({
      next: () => {
        alert('Evaluation submitted successfully!');
        this.router.navigate(['/pending']);
      },
      error: (err) => {
        console.error('Submit Error:', err);
        alert('Backend rejected the save! Check the browser console for details.');
      }
    });
  }
  // --- Pagination State ---
  currentSectionIndex = 0;

  get isFirstSection(): boolean {
    return this.currentSectionIndex === 0;
  }

  get isLastSection(): boolean {
    const sections = this.templateData()?.sections || [];
    return this.currentSectionIndex === sections.length - 1;
  }

  get progressPercentage(): number {
    const sections = this.templateData()?.sections || [];
    if (sections.length === 0) return 0;
    return ((this.currentSectionIndex + 1) / sections.length) * 100;
  }

  nextSection(): void {
    if (!this.isLastSection) {
      this.currentSectionIndex++;
      window.scrollTo({ top: 0, behavior: 'smooth' }); // Scroll to top of next section
    }
  }

  prevSection(): void {
    if (!this.isFirstSection) {
      this.currentSectionIndex--;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}