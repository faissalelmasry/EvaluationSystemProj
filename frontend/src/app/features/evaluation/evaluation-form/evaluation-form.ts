import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { EvaluationService } from '../../../core/services/evaluation';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

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


  evaluationForm!: FormGroup; 
  assignmentId!: number;
  templateData: any; 

  ngOnInit(): void {
    this.assignmentId = Number(this.route.snapshot.paramMap.get('assignmentId'));

    this.evaluationForm = this.fb.group({
      responses: this.fb.array([]) 
    });

    this.loadMockTemplate();
  }

loadMockTemplate() {
    this.templateData = {
      id: 1,
      title: "Mid-Year Developer Evaluation",
      sections: [
        {
          id: 101,
          name: "Technical Skills",
          criteria: [
            // Type 4: RatingScale
            { id: 1001, text: "Rate your C# Clean Architecture knowledge.", type: 4 }, 
            // Type 5: Boolean
            { id: 1002, text: "Are you comfortable deploying to production?", type: 5 },
            // Type 3: Text
            { id: 1003, text: "What is your primary technical goal for next year?", type: 3 }
          ]
        }
      ]
    };

    this.buildFormArray();
  }

  get responsesArray(): FormArray {
    return this.evaluationForm.get('responses') as FormArray;
  }

buildFormArray() {
    this.templateData.sections.forEach((section: any) => {
      section.criteria.forEach((criterion: any) => {
        
        // We initialize all possible answers. 
        // We will only use the ones that match the question type!
        const questionGroup = this.fb.group({
          criterionId: [criterion.id],
          type: [criterion.type], // <--- Save the type so HTML knows what to draw
          score: [null],          // For Type 4 (Rating)
          selectedOption: [''],   // For Type 5 & 1 & 2 (Yes/No, Multiple Choice)
          textAnswer: ['']        // For Type 3 (Text)
        });

        this.responsesArray.push(questionGroup);
      });
    });
  }

  getQuestionGroup(criterionId: number): FormGroup {
    return this.responsesArray.controls.find(
      control => control.value.criterionId === criterionId
    ) as FormGroup;
  }

  onSubmit() {
    if (this.evaluationForm.valid) {
      const payload = {
        responses: this.evaluationForm.value.responses
      };

      this.evaluationService.submitEvaluation(this.assignmentId, payload).subscribe({
        next: () => alert('Evaluation submitted successfully!'),
        error: (err) => console.error('Submission failed', err)
      });
    }
  }
}