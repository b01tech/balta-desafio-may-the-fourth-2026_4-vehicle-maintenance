import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { VehicleApiService } from '../../core/services/vehicle-api.service';
import { MaintenanceAnalysis } from '../../core/models/vehicle.model';

@Component({
  selector: 'app-vehicle-analysis',
  standalone: true,
  imports: [RouterLink, DatePipe, CurrencyPipe],
  template: `
    <div class="container">
      <div class="header">
        <h1>Análise de Manutenção</h1>
        <a routerLink="/vehicles" class="btn btn-secondary">Voltar</a>
      </div>

      @if (loading()) {
        <div class="loading">Carregando análise...</div>
      } @else if (analysis()) {
        <div class="vehicle-info">
          <h2>{{ analysis()!.brand }} {{ analysis()!.model }}</h2>
          <p>Ano: {{ analysis()!.year }}</p>
          <p>Quilometragem atual: {{ analysis()!.currentMileage | number }} km</p>
        </div>

        @if (analysis()!.recommendations.length === 0) {
          <div class="no-recommendations">
            <p>Nenhuma manutenção necessária no momento. Continue mantendo a quilometragem em dia!</p>
          </div>
        } @else {
          <div class="recommendations">
            <h3>Recomendações de Manutenção</h3>
            @for (rec of analysis()!.recommendations; track rec.serviceType) {
              <div class="recommendation-card" [class]="'urgency-' + rec.urgencyLevel.toLowerCase()">
                <div class="rec-header">
                  <h4>{{ rec.description }}</h4>
                  <span class="urgency-badge" [class]="'badge-' + rec.urgencyLevel.toLowerCase()">
                    {{ rec.urgencyLevel }}
                  </span>
                </div>
                
                <div class="rec-details">
                  <p><strong>Próxima revisão:</strong> {{ rec.estimatedMileage | number }} km</p>
                  <p><strong>Falta:</strong> {{ rec.milesUntilDue | number }} km</p>
                </div>

                @if (rec.recommendedParts.length > 0) {
                  <div class="parts-section">
                    <h5>Peças Recomendadas</h5>
                    <table class="parts-table">
                      <thead>
                        <tr>
                          <th>Peça</th>
                          <th>Qtd</th>
                          <th>Preço</th>
                        </tr>
                      </thead>
                      <tbody>
                        @for (part of rec.recommendedParts; track part.name) {
                          <tr>
                            <td>{{ part.name }}</td>
                            <td>{{ part.quantity }}</td>
                            <td>{{ part.estimatedPrice | currency:'BRL' }}</td>
                          </tr>
                        }
                      </tbody>
                      <tfoot>
                        <tr>
                          <td colspan="2"><strong>Total Estimado</strong></td>
                          <td><strong>{{ rec.estimatedTotalPrice | currency:'BRL' }}</strong></td>
                        </tr>
                      </tfoot>
                    </table>
                  </div>
                }
              </div>
            }
          </div>

          <div class="total-summary">
            <h3>Resumo de Investimento</h3>
            <p class="total">
              Total estimado para todas as manutenções: 
              <strong>{{ getTotalPrice() | currency:'BRL' }}</strong>
            </p>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .container { padding: 24px; max-width: 900px; margin: 0 auto; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    h1 { margin: 0; }
    .btn { padding: 10px 20px; border-radius: 4px; text-decoration: none; font-size: 14px; cursor: pointer; border: none; }
    .btn-secondary { background: #757575; color: white; }
    .loading { text-align: center; padding: 48px; color: #666; }
    .vehicle-info { background: #f5f5f5; padding: 20px; border-radius: 8px; margin-bottom: 24px; }
    .vehicle-info h2 { margin: 0 0 8px; }
    .vehicle-info p { margin: 4px 0; color: #666; }
    .no-recommendations { text-align: center; padding: 32px; background: #e8f5e9; border-radius: 8px; color: #2e7d32; }
    .recommendations { display: flex; flex-direction: column; gap: 16px; }
    .recommendation-card { background: white; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; }
    .rec-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .rec-header h4 { margin: 0; }
    .urgency-badge { padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: 600; }
    .badge-critical { background: #ffcdd2; color: #c62828; }
    .badge-high { background: #ffe0b2; color: #ef6c00; }
    .badge-medium { background: #fff9c4; color: #f9a825; }
    .badge-low { background: #c8e6c9; color: #2e7d32; }
    .rec-details { margin-bottom: 16px; }
    .rec-details p { margin: 4px 0; font-size: 14px; }
    .parts-section h5 { margin: 0 0 8px; font-size: 14px; }
    .parts-table { width: 100%; font-size: 13px; border-collapse: collapse; }
    .parts-table th, .parts-table td { padding: 8px; text-align: left; border-bottom: 1px solid #eee; }
    .parts-table tfoot td { border-top: 2px solid #333; }
    .total-summary { margin-top: 24px; padding: 20px; background: #e3f2fd; border-radius: 8px; }
    .total-summary h3 { margin: 0 0 8px; }
    .total-summary .total { font-size: 18px; }
  `]
})
export class VehicleAnalysisComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(VehicleApiService);

  loading = signal(true);
  analysis = signal<MaintenanceAnalysis | null>(null);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.analyze({ vehicleId: id }).subscribe({
        next: (data) => {
          this.analysis.set(data);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    }
  }

  getTotalPrice(): number {
    const recs = this.analysis()?.recommendations;
    return recs ? recs.reduce((sum, r) => sum + r.estimatedTotalPrice, 0) : 0;
  }
}