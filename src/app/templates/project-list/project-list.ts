import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Project, ProjectService } from './project.service';
import {Observable} from 'rxjs';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './project-list.html',
})
export class ProjectListComponent implements OnInit {
  private service = inject(ProjectService);

  projects = signal<Project[]>([]);
  error = signal('');

  // formulár: projectId je null, keď pridávame novy
  projectId: string | null = null;
  form = { name: '', abbreviation: '', customer: '' };

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({
      next: list => this.projects.set(list),
      error: () => this.error.set('Zoznam sa nepodarilo načítať.'),
    });
  }

  edit(p: Project) {
    this.projectId = p.id;
    this.form = {name: p.name, abbreviation: p.abbreviation, customer: p.customer};
  }

  cancel() {
    this.projectId = null;
    this.form = { name: '', abbreviation: '', customer: '' };
  }

  save() {
    const request: Observable<unknown> = this.projectId === null
      ? this.service.create(this.form)
      : this.service.update(this.projectId, this.form);

    request.subscribe({
      next: () => { this.cancel(); this.load(); },
      error: () => this.error.set('Uloženie zlyhalo.'),
    });
  }

  remove(p: Project) {
    if (!confirm(`Zmazať ${p.name}?`)) return;
    this.service.delete(p.id).subscribe(() => this.load());
  }
}
