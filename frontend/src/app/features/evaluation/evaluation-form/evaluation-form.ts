import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
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
    this.templateId = Number(this.route.snapshot.queryParamMap.get('templateId')) || 1;
    this.loadTemplate();
  }

  private loadTemplate(): void {
    this.templateService.GetTemplateById(this.templateId).subscribe({
      next: (data) => {
        this.templateData.set(data);
        this.buildFormArray(data);
      },
      error: () => alert('Could not find that Template ID in the database.'),
    });
  }

  private buildFormArray(template: any): void {
    template?.sections?.forEach((section: any) => {
      section.criteria?.forEach((criterion: any) => {
        this.responsesArray.push(
          this.fb.group({
            criterionId: [criterion.id],
            questionType: [criterion.questionType],
            maxScore: [criterion.maxScore || 5],
            score: [null, [Validators.min(0), Validators.max(criterion.maxScore || 5)]],
            selectedOption: [''],
            textAnswer: [''],
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
      let score = res.score ?? 0;

      if (res.questionType === 5 || res.questionType === 'Boolean') {
        score = res.selectedOption === 'Yes' ? res.maxScore : 0;
      }

      const responseObj: any = { criterionId: res.criterionId, score };
      if (res.textAnswer?.trim()) responseObj.textAnswer = res.textAnswer;

      return responseObj;
    });

    this.evaluationService.submitEvaluation(this.assignmentId, { responses }).subscribe({
      next: () => alert('Evaluation submitted successfully!'),
      error: () => alert('Backend rejected the save! Check the console.'),
    });
  }
}