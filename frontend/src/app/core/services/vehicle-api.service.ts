import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Vehicle, 
  CreateVehicle, 
  UpdateVehicle, 
  MaintenanceAnalysis,
  MaintenanceAnalysisRequest
} from '../models/vehicle.model';

@Injectable({
  providedIn: 'root'
})
export class VehicleApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api/vehicle';

  getAll(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(this.baseUrl);
  }

  getById(id: string): Observable<Vehicle> {
    return this.http.get<Vehicle>(`${this.baseUrl}/${id}`);
  }

  create(vehicle: CreateVehicle): Observable<Vehicle> {
    return this.http.post<Vehicle>(this.baseUrl, vehicle);
  }

  update(id: string, vehicle: UpdateVehicle): Observable<Vehicle> {
    return this.http.put<Vehicle>(`${this.baseUrl}/${id}`, vehicle);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  uploadCsv(id: string, file: File): Observable<{ message: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ message: string }>(`${this.baseUrl}/${id}/upload-csv`, formData);
  }

  analyze(request: MaintenanceAnalysisRequest): Observable<MaintenanceAnalysis> {
    return this.http.post<MaintenanceAnalysis>(`${this.baseUrl}/analyze`, request);
  }

  analyzeWithAi(vehicleId: string, additionalContext?: string): Observable<{ analysis: string }> {
    return this.http.post<{ analysis: string }>(`${this.baseUrl}/analyze-ai`, {
      vehicleId,
      additionalContext
    });
  }
}