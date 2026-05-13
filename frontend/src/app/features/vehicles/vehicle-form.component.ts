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
    .container { padding: 24px; max-width: 600px; margin: 0 auto; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    h1 { margin: 0; }
    .btn { padding: 10px 20px; border-radius: 4px; text-decoration: none; font-size: 14px; cursor: pointer; border: none; }
    .btn-secondary { background: #757575; color: white; }
    .btn-primary { background: #1976d2; color: white; }
    .btn-primary:disabled { background: #b0b0b0; }
    .form { display: flex; flex-direction: column; gap: 16px; }
    .form-group { display: flex; flex-direction: column; gap: 6px; }
    .form-group label { font-weight: 500; }
    .form-group input { padding: 10px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; }
    .error { color: #d32f2f; font-size: 12px; }
    .form-actions { margin-top: 16px; }
    .upload-section { margin-top: 32px; padding-top: 24px; border-top: 1px solid #e0e0e0; }
    .upload-section h3 { margin-top: 0; }
    .format { font-size: 12px; color: #666; }
    .success { color: #2e7d32; margin-left: 8px; }
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