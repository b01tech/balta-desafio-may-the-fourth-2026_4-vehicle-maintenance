import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { VehicleApiService } from '../../core/services/vehicle-api.service';
import { MaintenanceAnalysis } from '../../core/models/vehicle.model';

@Component({
  selector: 'app-vehicle-analysis',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DecimalPipe],
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
            <p>
              Nenhuma manutenção necessária no momento. Continue mantendo a quilometragem em dia!
            </p>
          </div>
        } @else {
          <div class="recommendations">
            <h3>Recomendações de Manutenção</h3>
            @for (rec of analysis()!.recommendations; track rec.serviceType) {
              <div
                class="recommendation-card"
                [class]="'urgency-' + rec.urgencyLevel.toLowerCase()"
              >
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
                            <td>{{ part.estimatedPrice | currency: 'BRL' }}</td>
                          </tr>
                        }
                      </tbody>
                      <tfoot>
                        <tr>
                          <td colspan="2"><strong>Total Estimado</strong></td>
                          <td>
                            <strong>{{ rec.estimatedTotalPrice | currency: 'BRL' }}</strong>
                          </td>
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
              <strong>{{ getTotalPrice() | currency: 'BRL' }}</strong>
            </p>
          </div>
        }
      }
    </div>
  `,
  styles: [
    `
      .container {
        padding: 32px;
        max-width: 900px;
        margin: 0 auto;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        min-height: 100vh;
        border-radius: 16px;
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
        text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
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
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      }
      .btn-secondary {
        background: white;
        color: #667eea;
      }
      .btn-secondary:hover {
        transform: translateY(-2px);
      }
      .loading {
        text-align: center;
        padding: 64px;
        color: white;
        font-size: 18px;
        background: rgba(255, 255, 255, 0.15);
        border-radius: 16px;
        backdrop-filter: blur(10px);
      }
      .vehicle-info {
        background: rgba(255, 255, 255, 0.98);
        padding: 24px;
        border-radius: 16px;
        margin-bottom: 24px;
        box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
      }
      .vehicle-info h2 {
        margin: 0 0 12px;
        color: #333;
        font-size: 24px;
      }
      .vehicle-info p {
        margin: 8px 0;
        color: #666;
        font-size: 15px;
      }
      .no-recommendations {
        text-align: center;
        padding: 40px;
        background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
        border-radius: 16px;
        color: white;
        font-size: 16px;
        box-shadow: 0 8px 32px rgba(56, 239, 125, 0.3);
      }
      .recommendations h3 {
        color: white;
        font-size: 22px;
        margin-bottom: 16px;
        text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
      }
      .recommendations {
        display: flex;
        flex-direction: column;
        gap: 20px;
      }
      .recommendation-card {
        background: white;
        border: none;
        border-radius: 16px;
        padding: 24px;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
        transition: all 0.3s ease;
      }
      .recommendation-card:hover {
        transform: translateY(-4px);
        box-shadow: 0 8px 30px rgba(0, 0, 0, 0.15);
      }
      .rec-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 16px;
        padding-bottom: 12px;
        border-bottom: 2px solid #f0f0f0;
      }
      .rec-header h4 {
        margin: 0;
        color: #333;
        font-size: 18px;
        font-weight: 600;
      }
      .urgency-badge {
        padding: 6px 16px;
        border-radius: 20px;
        font-size: 12px;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
      }
      .badge-critical {
        background: linear-gradient(135deg, #eb3349 0%, #f45c43 100%);
        color: white;
      }
      .badge-high {
        background: linear-gradient(135deg, #f2994a 0%, #f2c94c 100%);
        color: white;
      }
      .badge-medium {
        background: linear-gradient(135deg, #f2c94c 0%, #f2994a 100%);
        color: white;
      }
      .badge-low {
        background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
        color: white;
      }
      .rec-details {
        margin-bottom: 20px;
        display: flex;
        gap: 24px;
      }
      .rec-details p {
        margin: 0;
        font-size: 14px;
        color: #666;
      }
      .rec-details strong {
        color: #333;
      }
      .parts-section h5 {
        margin: 0 0 12px;
        font-size: 16px;
        color: #333;
        font-weight: 600;
      }
      .parts-table {
        width: 100%;
        font-size: 14px;
        border-collapse: collapse;
        background: #fafafa;
        border-radius: 8px;
        overflow: hidden;
      }
      .parts-table th,
      .parts-table td {
        padding: 12px 16px;
        text-align: left;
        border-bottom: 1px solid #eee;
      }
      .parts-table th {
        background: #f5f5f5;
        font-weight: 600;
        color: #333;
        font-size: 12px;
        text-transform: uppercase;
      }
      .parts-table tfoot td {
        border-top: 2px solid #667eea;
        background: #f0f4ff;
        font-weight: 600;
        color: #667eea;
      }
      .total-summary {
        margin-top: 24px;
        padding: 24px;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        border-radius: 16px;
        color: white;
        box-shadow: 0 8px 32px rgba(102, 126, 234, 0.4);
      }
      .total-summary h3 {
        margin: 0 0 12px;
        font-size: 20px;
      }
      .total-summary .total {
        font-size: 24px;
        font-weight: 700;
      }
    `,
  ],
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
        error: () => this.loading.set(false),
      });
    }
  }

  getTotalPrice(): number {
    const recs = this.analysis()?.recommendations;
    return recs ? recs.reduce((sum, r) => sum + r.estimatedTotalPrice, 0) : 0;
  }
}
