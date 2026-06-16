import { Component, OnInit, Signal, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { TemplateService } from '../../../core/services/template.service';
import { Router } from '@angular/router';
import { QuestionType } from '../../../core/enums/question-type';

@Component({
  selector: 'app-create-template',
  imports: [ReactiveFormsModule],
  templateUrl: './create-template.html',
  styleUrls: ['./create-template.scss'],
})
export class CreateTemplate implements OnInit {
  questionTypes = [
      { id: QuestionType.SingleChoice, name: 'Single Choice' },
      { id: QuestionType.MultipleChoice, name: 'Multiple Choice' },
      { id: QuestionType.Text, name: 'Text' },
      { id: QuestionType.RatingScale, name: 'Rating Scale' },
      { id: QuestionType.Boolean, name: 'Boolean' },
    ];
  form!:FormGroup;
  isSubmitting:boolean=false;
  errorMessage = signal('');
  public constructor(private fb:FormBuilder,private templateService:TemplateService,private router:Router){}
  ngOnInit(): void {
    this.form=this.fb.group({
      title:['',[Validators.required,Validators.maxLength(30)]],
      description:['',[Validators.required,Validators.maxLength(100)]],
      createdById:[4],
      sections:this.fb.array([this.createSection()])
    })
  }
  get Sections():FormArray
  {
    return this.form.get("sections") as FormArray
  }
  getCriteria(sectionIndex: number): FormArray {
    return this.Sections.at(sectionIndex).get('criteria') as FormArray;
  }

  private createSection(): FormGroup {
    return this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(30)]],
      description: ['', [Validators.required, Validators.maxLength(100)]],
      orderNo: [1, [Validators.required, Validators.min(1)]],
      criteria: this.fb.array([this.createCriterion()])
    });
  }
    removeSection(index: number): void {
    if (this.Sections.length === 0) return; 
    this.Sections.removeAt(index);
  }

  private createCriterion(sectionIndex?: number): FormGroup {
    const criteriaArray = sectionIndex !== undefined ? this.getCriteria(sectionIndex) : null;
    return this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      orderNo: [criteriaArray ? criteriaArray.length + 1 : 1, [Validators.required, Validators.min(1)]],
      questionType: [QuestionType.SingleChoice,[Validators.required]],
      maxScore: [1,Validators.min(1)],
      weight: [1,[Validators.required,Validators.min(1)]],
      isRequired: [true,Validators.required],
    });
  }

  addSection(): void {
    this.Sections.push(this.createSection());
  }


  addCriterion(sectionIndex: number): void {
    this.getCriteria(sectionIndex).push(this.createCriterion(sectionIndex));
  }
  removeCriterion(sectionIndex: number, criterionIndex: number): void {
    const criteria = this.getCriteria(sectionIndex);
    if (criteria.length === 0) return;
    criteria.removeAt(criterionIndex);
  }



  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage.set('');

    this.templateService.AddTemplate(this.form.value).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        this.router.navigateByUrl(`/templates`)
      },
      error: (err) => {
        this.isSubmitting = false;
        console.log(err.error.Message)
        this.errorMessage.set(err.error?.Message || 'Something went wrong');
      }
    });
  }
}

