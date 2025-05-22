import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BeanOfTheDayComponent } from './bean-of-the-day.component';

describe('BeanOfTheDayComponent', () => {
  let component: BeanOfTheDayComponent;
  let fixture: ComponentFixture<BeanOfTheDayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BeanOfTheDayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BeanOfTheDayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
