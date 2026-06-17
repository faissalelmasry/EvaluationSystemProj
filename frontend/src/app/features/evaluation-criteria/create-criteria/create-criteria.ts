import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CriteriaService } from '../../../core/services/criteria-service';
import { QuestionType } from '../../../core/enums/question-type';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-create-criteria',
  imports: [ReactiveFormsModule],
  templateUrl: './create-criteria.html',
  styleUrl: './create-criteria.scss',
})
export class CreateCriteria implements OnInit {

  questionTypes = [
    { id: QuestionType.SingleChoice, name: 'Single Choice' },
    { id: QuestionType.MultipleChoice, name: 'Multiple Choice' },
    { id: QuestionType.Text, name: 'Text' },
    { id: QuestionType.RatingScale, name: 'Rating Scale' },
    { id: QuestionType.Boolean, name: 'Boolean' },
  ];

  Form!: FormGroup;
  BackEndError= signal('');
  IsSubmitting=signal(false);
  sectionId!: number;
  criteriaId!: number;
  isEditMode = false;

  public constructor(
  private fb: FormBuilder,
  private service: CriteriaService,
  private route: ActivatedRoute,
  private router:Router
) {}
  ngOnInit(): void {

  this.Form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', [Validators.required, Validators.maxLength(200)]],
    questionType: [QuestionType.Text, Validators.required],
    maxScore: [0, [Validators.required, Validators.min(1)]],
    weight: [0, [Validators.required, Validators.min(0)]],
    isRequired: [false],
    orderNo: [1, Validators.required],
  });

  this.sectionId = Number(
    this.route.snapshot.paramMap.get('sectionid')
  );

  this.criteriaId = Number(
    this.route.snapshot.paramMap.get('criteriaid')
  );

  if (this.criteriaId) {
    this.isEditMode = true;
  }
}

  onSubmit() {

  if (this.Form.invalid) {
    this.Form.markAllAsTouched();
    return;
  }

  this.IsSubmitting.set(true);

  if (this.isEditMode) {

    this.service
      .EditCriterion(this.criteriaId, this.Form.value)
      .subscribe({
        next: (res) => {

          this.IsSubmitting.set(false);
          this.BackEndError.set('');
          this.router.navigateByUrl(`/templates`)

        },
        error: err => {

          this.BackEndError.set(err.error.Message);
          this.IsSubmitting.set(false);

        }
      });

  }
  else {

    this.service
      .AddCriterion(this.sectionId, this.Form.value)
      .subscribe({
        next: () => {

          this.IsSubmitting.set(false);
          this.BackEndError.set('');
          this.router.navigateByUrl(`/templates`)


        },
        error: err => {

          this.BackEndError.set(err.error.Message);
          this.IsSubmitting.set(false);

        }
      });

  }
}

  get f() {
    return this.Form.controls;
  }
}
