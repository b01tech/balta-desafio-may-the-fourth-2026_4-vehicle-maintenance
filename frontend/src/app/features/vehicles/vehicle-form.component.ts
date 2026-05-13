import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { VehicleApiService } from '../../core/services/vehicle-api.service';
import { CreateVehicle } from '../../core/models/vehicle.model';

@Component({
  selector: 'app-vehicle-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <div class="container">
      <div class="header">
        <h1>{{ isEdit() ? 'Editar' : 'Novo' }} Veículo</h1>
        <a routerLink="/vehicles" class="btn btn-secondary">Voltar</a>
      </div>

      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form">
        <div class="form-group">
          <label for="brand">Marca</label>
          <input id="brand" type="text" formControlName="brand" placeholder="Ex: Toyota">
          @if (form.get('brand')?.invalid && form.get('brand')?.touched) {
            <span class="error">Marca é obrigatória</span>
          }
        </div>

        <div class="form-group">
          <label for="model">Modelo</label>
          <input id="model" type="text" formControlName="model" placeholder="Ex: Corolla">
          @if (form.get('model')?.invalid && form.get('model')?.touched) {
            <span class="error">Modelo é obrigatório</span>
          }
        </div>

        <div class="form-group">
          <label for="year">Ano</label>
          <input id="year" type="number" formControlName="year" placeholder="Ex: 2020">
          @if (form.get('year')?.invalid && form.get('year')?.touched) {
            <span class="error">Ano inválido</span>
          }
        </div>

        <div class="form-group">
          <label for="currentMileage">Quilometragem atual</label>
          <input id="currentMileage" type="number" formControlName="currentMileage" placeholder="Ex: 50000">
          @if (form.get('currentMileage')?.invalid && form.get('currentMileage')?.touched) {
            <span class="error">Quilometragem inválida</span>
          }
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary" [disabled]="form.invalid || saving()">
            {{ saving() ? 'Salvando...' : 'Salvar' }}
          </button>
        </div>
      </form>

      @if (isEdit()) {
        <div class="upload-section">
          <h3>Importar Dados de Quilometragem</h3>
          <p>Faça upload de um arquivo CSV com o histórico de quilometragem.</p>
          <p class="format">Formato: data,quilometragem (Ex: 2024-01-15,50000)</p>
          <input type="file" (change)="onFileSelected($event)" accept=".csv">
          @if (uploading()) {
            <span>Enviando...</span>
          }
          @if (uploadSuccess()) {
            <span class="success">CSV importado com sucesso!</span>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .container { 
      padding: 32px; 
      max-width: 600px; 
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
      color: white;
      font-size: 28px;
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
    }
    .btn-secondary { 
      background: white; 
      color: #667eea;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }
    .btn-secondary:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0,0,0,0.2);
    }
    .btn-primary { 
      background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); 
      color: white;
      box-shadow: 0 4px 15px rgba(56, 239, 125, 0.4);
    }
    .btn-primary:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(56, 239, 125, 0.5);
    }
    .btn-primary:disabled { 
      background: #b0b0b0; 
      box-shadow: none;
    }
    .form { 
      display: flex; 
      flex-direction: column; 
      gap: 20px;
      background: rgba(255,255,255,0.98);
      padding: 32px;
      border-radius: 16px;
      box-shadow: 0 8px 32px rgba(0,0,0,0.2);
    }
    .form-group { 
      display: flex; 
      flex-direction: column; 
      gap: 8px; 
    }
    .form-group label { 
      font-weight: 600; 
      color: #333;
      font-size: 14px;
    }
    .form-group input { 
      padding: 14px 16px; 
      border: 2px solid #e0e0e0; 
      border-radius: 8px; 
      font-size: 15px;
      transition: all 0.3s ease;
      background: #fafafa;
    }
    .form-group input:focus {
      outline: none;
      border-color: #667eea;
      background: white;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }
    .form-group input::placeholder {
      color: #aaa;
    }
    .error { 
      color: #eb3349; 
      font-size: 13px; 
      font-weight: 500;
    }
    .form-actions { 
      margin-top: 8px; 
      display: flex;
      gap: 12px;
    }
    .form-actions button {
      flex: 1;
    }
    .upload-section { 
      margin-top: 32px; 
      padding: 24px;
      background: rgba(255,255,255,0.98);
      border-radius: 16px;
      box-shadow: 0 8px 32px rgba(0,0,0,0.2);
    }
    .upload-section h3 { 
      margin: 0 0 12px;
      color: #333;
      font-size: 20px;
    }
    .upload-section p { 
      color: #666;
      font-size: 14px;
      margin: 0 0 8px;
    }
    .format { 
      font-size: 13px; 
      color: #888; 
      font-family: monospace;
      background: #f5f5f5;
      padding: 8px 12px;
      border-radius: 6px;
      display: inline-block;
    }
    .upload-section input[type="file"] {
      margin-top: 16px;
      padding: 12px;
      border: 2px dashed #667eea;
      border-radius: 8px;
      width: 100%;
      box-sizing: border-box;
      cursor: pointer;
      background: #f8f9ff;
    }
    .success { 
      color: #11998e; 
      margin-left: 12px;
      font-weight: 500;
      display: inline-block;
      padding: 8px 16px;
      background: #e8f8f5;
      border-radius: 8px;
      margin-top: 12px;
    }
  `]
})
export class VehicleFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(VehicleApiService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  isEdit = signal(false);
  saving = signal(false);
  uploading = signal(false);
  uploadSuccess = signal(false);
  private vehicleId = '';
  private selectedFile: File | null = null;

  form: FormGroup = this.fb.group({
    brand: ['', Validators.required],
    model: ['', Validators.required],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1900), Validators.max(2100)]],
    currentMileage: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'new') {
      this.vehicleId = id;
      this.isEdit.set(true);
      this.loadVehicle();
    }
  }

  loadVehicle() {
    this.api.getById(this.vehicleId).subscribe({
      next: (vehicle) => {
        this.form.patchValue({
          brand: vehicle.brand,
          model: vehicle.model,
          year: vehicle.year,
          currentMileage: vehicle.currentMileage
        });
      }
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.saving.set(true);
    const data: CreateVehicle = this.form.value;

    const request = this.isEdit() 
      ? this.api.update(this.vehicleId, data)
      : this.api.create(data);

    request.subscribe({
      next: () => this.router.navigate(['/vehicles']),
      error: () => this.saving.set(false)
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
      this.uploadCsv();
    }
  }

  uploadCsv() {
    if (!this.selectedFile) return;

    this.uploading.set(true);
    this.uploadSuccess.set(false);

    this.api.uploadCsv(this.vehicleId, this.selectedFile).subscribe({
      next: () => {
        this.uploadSuccess.set(true);
        this.uploading.set(false);
      },
      error: () => this.uploading.set(false)
    });
  }
}