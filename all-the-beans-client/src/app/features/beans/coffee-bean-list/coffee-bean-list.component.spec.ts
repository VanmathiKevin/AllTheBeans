import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoffeeBeanListComponent } from './coffee-bean-list.component';

describe('CoffeeBeanListComponent', () => {
  let component: CoffeeBeanListComponent;
  let fixture: ComponentFixture<CoffeeBeanListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoffeeBeanListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoffeeBeanListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
