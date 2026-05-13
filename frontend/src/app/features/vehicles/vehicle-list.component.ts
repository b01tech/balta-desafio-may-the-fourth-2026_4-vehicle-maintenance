import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { VehicleApiService } from '../../core/services/vehicle-api.service';
import { Vehicle } from '../../core/models/vehicle.model';

@Component({
  selector: 'app-vehicle-list',
  standalone: true,
  imports: [RouterLink, DatePipe, DecimalPipe],
  template: `
    <div class="container">
      <div class="header">
        <h1>Veículos</h1>
        <a routerLink="/vehicles/new" class="btn btn-primary">Novo Veículo</a>
      </div>

      @if (loading()) {
        <div class="loading">Carregando...</div>
      } @else if (vehicles().length === 0) {
        <div class="empty">
          <p>Nenhum veículo cadastrado.</p>
          <a routerLink="/vehicles/new" class="btn btn-secondary">Cadastrar primeiro veículo</a>
        </div>
      } @else {
        <div class="table-container">
          <table class="vehicle-table">
            <thead>
              <tr>
                <th>Marca</th>
                <th>Modelo</th>
                <th>Ano</th>
                <th>Km</th>
                <th>Criado em</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              @for (vehicle of vehicles(); track vehicle.id) {
                <tr>
                  <td>{{ vehicle.brand }}</td>
                  <td>{{ vehicle.model }}</td>
                  <td>{{ vehicle.year }}</td>
                  <td>{{ vehicle.currentMileage | number }} km</td>
                  <td>{{ vehicle.createdAt | date:'dd/MM/yyyy' }}</td>
                  <td class="actions">
                    <a [routerLink]="['/vehicles', vehicle.id]" class="btn btn-small">Editar</a>
                    <a [routerLink]="['/vehicles', vehicle.id, 'analysis']" class="btn btn-small btn-info">Análise</a>
                    <button (click)="deleteVehicle(vehicle.id)" class="btn btn-small btn-danger">Excluir</button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
  styles: [`
    .container { padding: 24px; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    h1 { margin: 0; font-size: 24px; }
    .btn { padding: 10px 20px; border-radius: 4px; text-decoration: none; font-size: 14px; cursor: pointer; border: none; }
    .btn-primary { background: #1976d2; color: white; }
    .btn-secondary { background: #757575; color: white; }
    .btn-small { padding: 6px 12px; font-size: 12px; margin-right: 8px; }
    .btn-info { background: #0288d1; color: white; }
    .btn-danger { background: #d32f2f; color: white; }
    .loading, .empty { text-align: center; padding: 48px; color: #666; }
    .table-container { overflow-x: auto; }
    .vehicle-table { width: 100%; border-collapse: collapse; }
    .vehicle-table th, .vehicle-table td { padding: 12px; text-align: left; border-bottom: 1px solid #e0e0e0; }
    .vehicle-table th { background: #f5f5f5; font-weight: 600; }
    .actions { display: flex; gap: 4px; }
  `]
})
export class VehicleListComponent implements OnInit {
  private readonly api = inject(VehicleApiService);
  
  vehicles = signal<Vehicle[]>([]);
  loading = signal(true);

  ngOnInit() {
    this.loadVehicles();
  }

  loadVehicles() {
    this.api.getAll().subscribe({
      next: (data) => {
        this.vehicles.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  deleteVehicle(id: string) {
    if (confirm('Tem certeza que deseja excluir este veículo?')) {
      this.api.delete(id).subscribe({
        next: () => this.loadVehicles()
      });
    }
  }
}