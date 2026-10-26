import { Routes } from '@angular/router';
import {authGuard} from '../authentication/auth.gard';
import {ProjectListComponent} from './templates/project-list/project-list';
import {Login} from './templates/login/login';
import { Register } from './templates/register/register';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'projects', component: ProjectListComponent, canActivate: [authGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'projects' },
  { path: 'register', component: Register },
];
