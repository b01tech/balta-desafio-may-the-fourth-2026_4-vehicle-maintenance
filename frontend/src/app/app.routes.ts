import { Routes } from '@angular/router';
import { VehicleListComponent } from './features/vehicles/vehicle-list.component';
import { VehicleFormComponent } from './features/vehicles/vehicle-form.component';
import { VehicleAnalysisComponent } from './features/analysis/vehicle-analysis.component';

export const routes: Routes = [
  { path: '', redirectTo: '/vehicles', pathMatch: 'full' },
  { path: 'vehicles', component: VehicleListComponent },
  { path: 'vehicles/new', component: VehicleFormComponent },
  { path: 'vehicles/:id', component: VehicleFormComponent },
  { path: 'vehicles/:id/analysis', component: VehicleAnalysisComponent },
  { path: '**', redirectTo: '/vehicles' }
];