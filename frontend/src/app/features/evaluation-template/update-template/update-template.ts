import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TemplateService } from '../../../core/services/template.service';
import { UpdateTemplateDto } from '../../../core/models/update-template-dto';

@Component({
  selector: 'app-update-template',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './update-template.html',
  styleUrl: './update-template.scss'
})
export class UpdateTemplate implements OnInit {

  form!: FormGroup;

  isLoading = signal(false);
  backendError = signal('');

  private templateId!: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private templateService: TemplateService
  ) {}

  ngOnInit(): void {

    this.templateId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]]
    });

    this.loadTemplate();
  }

  loadTemplate() {

    this.isLoading.set(true);

    this.templateService
      .GetTemplateById(this.templateId)
      .subscribe({
        next: (template) => {

          this.form.patchValue({
            title: template.title,
            description: template.description
          });

          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
  }

  submit() {

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const dto: UpdateTemplateDto = {
      title: this.form.value.title,
      description: this.form.value.description,
      createdById: 1
    };

    this.templateService
      .UpdateTemplate(this.templateId, dto)
      .subscribe({
        next: () => {

          this.router.navigate(['/templates']);

        },
        error: err => {

          this.backendError.set(
            err?.error?.message ?? 'Something went wrong'
          );

        }
      });
  }
}