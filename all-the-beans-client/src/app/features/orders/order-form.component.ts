import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoffeeBean } from '../../core/models/coffee-bean.model';
import { BeanService } from '../../core/services/bean.service';

@Component({
  standalone: true,
  selector: 'app-order-form',
  templateUrl: './order-form.component.html',
  styleUrls: ['./order-form.component.scss'],
  imports: [CommonModule, FormsModule]
})
export class OrderFormComponent implements OnInit {
  beans: CoffeeBean[] = [];
  order = {
    customerName: '',
    email: '',
    quantity: 1,
    beanId: 0
  };

  submitted = false;
  error = '';

  constructor(private beanService: BeanService) {}

  ngOnInit(): void {
    this.beanService.getAll().subscribe({
      next: (data) => {
        this.beans = data;
      },
      error: () => {
        this.error = 'Could not load coffee beans.';
      }
    });
  }

  submitOrder(): void {
    if (!this.order.customerName || !this.order.email || this.order.quantity <= 0 || !this.order.beanId) {
      this.error = 'Please fill out all fields correctly.';
      return;
    }

    this.submitted = true;
    this.error = '';
    //console.log('Order submitted:', this.order); - testing purpose as we don't have backend logic for this functionlaity.
  }
}
