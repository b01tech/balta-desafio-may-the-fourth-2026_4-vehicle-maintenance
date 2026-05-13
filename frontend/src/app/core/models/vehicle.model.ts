export interface Vehicle {
  id: string;
  brand: string;
  model: string;
  year: number;
  currentMileage: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateVehicle {
  brand: string;
  model: string;
  year: number;
  currentMileage: number;
}

export interface UpdateVehicle {
  brand: string;
  model: string;
  year: number;
  currentMileage: number;
}

export interface Part {
  name: string;
  quantity: number;
  estimatedPrice: number;
  partNumber?: string;
}

export interface MaintenanceRecommendation {
  serviceType: string;
  description: string;
  urgencyLevel: string;
  estimatedMileage: number;
  milesUntilDue: number;
  recommendedParts: Part[];
  estimatedTotalPrice: number;
}

export interface MaintenanceAnalysis {
  vehicleId: string;
  brand: string;
  model: string;
  year: number;
  currentMileage: number;
  recommendations: MaintenanceRecommendation[];
  analyzedAt: string;
}

export interface MaintenanceAnalysisRequest {
  vehicleId: string;
  additionalContext?: string;
}