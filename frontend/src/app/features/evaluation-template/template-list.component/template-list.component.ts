import { Component, signal } from '@angular/core';
import { TemplateService } from '../../../core/services/template.service';
import { EvaluationTemplateList } from '../../../core/models/evaluation-template-list';
import { OnInit } from '@angular/core';
import { Signal } from '@angular/core';
@Component({
  selector: 'app-template-list',
  imports: [],
  standalone: true,
  templateUrl: './template-list.component.html',
  styleUrls: ['./template-list.component.scss'],
})
export class TemplateListComponent implements OnInit {
public templates = signal<EvaluationTemplateList[]>([]);
public pageNumber = signal<number>(1);
public constructor(private TemplateService:TemplateService){}
ngOnInit()
{
   this.TemplateService.GetTemplates(this.pageNumber(),20,"").subscribe(res=>
   {
    this.templates.set(res);
    console.log(res)
   }
   )
}

}
