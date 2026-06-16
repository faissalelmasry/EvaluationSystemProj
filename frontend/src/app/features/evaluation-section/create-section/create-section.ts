import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SectionService } from '../../../core/services/section-service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-create-section',
  imports: [ReactiveFormsModule],
  templateUrl: './create-section.html',
  styleUrl: './create-section.scss',
})
export class CreateSection implements OnInit {
  templateId!:number
  Form!:FormGroup;
  IsSubmitting:boolean=false;
  isEditMode = false;
  sectionId!: number;
  BackEndError=signal('');
  public constructor(private fb:FormBuilder,private service:SectionService,private route:ActivatedRoute){}
  ngOnInit(): void {

  this.Form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(30)]],
    description: ['', [Validators.required, Validators.maxLength(100)]],
    orderNo: [1, Validators.required]
  });

  this.templateId = Number(
    this.route.snapshot.paramMap.get('templateid')
  );

  this.sectionId = Number(
    this.route.snapshot.paramMap.get('sectionid')
  );

  if (this.sectionId) {
    this.isEditMode = true;
  }
}
  onSubmit() {

  if (this.Form.invalid) {
    this.Form.markAllAsTouched();
    return;
  }

  this.IsSubmitting = true;

  if (this.isEditMode) {

    this.service
      .EditSection(this.sectionId, this.Form.value)
      .subscribe({
        next: () => {

          this.IsSubmitting = false;
          this.BackEndError.set('');

        },
        error: err => {

          this.BackEndError.set(err.error.Message);
          this.IsSubmitting = false;

        }
      });

  } else {

    this.service
      .AddSection(this.templateId, this.Form.value)
      .subscribe({
        next: () => {

          this.IsSubmitting = false;
          this.BackEndError.set('');
          this.Form.reset();

        },
        error: err => {

          this.BackEndError.set(err.error.Message);
          this.IsSubmitting = false;

        }
      });

  }
}

  get f() {
    return this.Form.controls;
  }
}
