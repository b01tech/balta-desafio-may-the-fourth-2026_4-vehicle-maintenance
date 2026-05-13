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
    .container { 
      padding: 32px; 
      max-width: 1000px; 
      margin: 0 auto;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      min-height: 100vh;
    }
    .header { 
      display: flex; 
      justify-content: space-between; 
      align-items: center; 
      margin-bottom: 32px;
    }
    .header h1 {
      margin: 0; 
      font-size: 28px; 
      color: white;
      font-weight: 600;
      text-shadow: 0 2px 4px rgba(0,0,0,0.2);
    }
    .btn { 
      padding: 12px 24px; 
      border-radius: 8px; 
      text-decoration: none; 
      font-size: 14px; 
      cursor: pointer; 
      border: none;
      font-weight: 500;
      transition: all 0.3s ease;
      box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    }
    .btn-primary { 
      background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); 
      color: white; 
    }
    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(56, 239, 125, 0.4);
    }
    .btn-secondary { 
      background: white; 
      color: #667eea;
    }
    .btn-small { 
      padding: 8px 14px; 
      font-size: 13px; 
      margin-right: 8px; 
    }
    .btn-info { 
      background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); 
      color: white; 
    }
    .btn-danger { 
      background: linear-gradient(135deg, #eb3349 0%, #f45c43 100%); 
      color: white; 
    }
    .btn:hover {
      transform: translateY(-2px);
    }
    .loading, .empty { 
      text-align: center; 
      padding: 64px; 
      color: white;
      background: rgba(255,255,255,0.15);
      border-radius: 16px;
      backdrop-filter: blur(10px);
    }
    .table-container { 
      overflow-x: auto; 
      background: rgba(255,255,255,0.95);
      border-radius: 16px;
      box-shadow: 0 8px 32px rgba(0,0,0,0.2);
      padding: 8px;
    }
    .vehicle-table { 
      width: 100%; 
      border-collapse: collapse; 
    }
    .vehicle-table th, .vehicle-table td { 
      padding: 16px; 
      text-align: left; 
      border-bottom: 1px solid #e8e8e8;
    }
    .vehicle-table th { 
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      font-weight: 600;
      text-transform: uppercase;
      font-size: 12px;
      letter-spacing: 0.5px;
    }
    .vehicle-table tr:hover {
      background: #f8f9ff;
    }
    .vehicle-table td {
      color: #333;
      font-size: 14px;
    }
    .actions { display: flex; gap: 6px; }
    .empty p {
      font-size: 18px;
      margin-bottom: 16px;
    }
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