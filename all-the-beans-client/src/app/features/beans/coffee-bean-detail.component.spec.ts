import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoffeeBeanDetailComponent } from './coffee-bean-detail.component';

describe('CoffeeBeanDetailComponent', () => {
  let component: CoffeeBeanDetailComponent;
  let fixture: ComponentFixture<CoffeeBeanDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoffeeBeanDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoffeeBeanDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
