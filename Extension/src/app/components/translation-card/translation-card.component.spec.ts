import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TranslationCardComponent } from './translation-card.component';

describe('TranslationCardComponent', () => {
  let component: TranslationCardComponent;
  let fixture: ComponentFixture<TranslationCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TranslationCardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TranslationCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
