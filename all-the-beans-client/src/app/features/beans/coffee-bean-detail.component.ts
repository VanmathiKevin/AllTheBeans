import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CoffeeBean } from '../../core/models/coffee-bean.model';
import { BeanService } from '../../core/services/bean.service';

@Component({
  standalone: true,
  selector: 'app-coffee-bean-detail',
  templateUrl: './coffee-bean-detail.component.html',
  styleUrls: ['./coffee-bean-detail.component.scss'],
  imports: [CommonModule, RouterModule]
})
export class CoffeeBeanDetailComponent implements OnInit {
  bean: CoffeeBean | null = null;
  error: string | null = null;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private beanService: BeanService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (isNaN(id)) {
      this.error = 'Invalid bean ID';
      return;
    }

    this.beanService.getById(id).subscribe({
      next: (data) => {
        this.bean = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Bean not found';
        this.loading = false;
      }
    });
  }
}
